using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using System.Linq;

/// AttackArea,BuffAndDebuffを統一した.


/// <summary>
/// Player/SkillArea.
/// </summary>
public class SkillColliderManager : MonoBehaviour, ISkillStart
{
    //Test用
    [SerializeField]
    private MyDebug myDebug;

    public IComboUI comboUI;        //Instant.
    private PhotonView view;
    private SkillInfoClass s_info = null;      //現在使っているskillの情報.

    //Debug用にpublic
    public skillEndDel skillEnd;
    private ICombinationBonus combo;
    private ICharaAnimation charaAnimation;
    private ICharaSkillAnimationSync charaAnimationSync;
    private ICharaDelayTime charaDelayTime;      //instat.

    [NonSerialized]
    public PvProtection pv;                 //Instant.
    [NonSerialized]
    public ReferenceInSarchArea reference;  //Instant.

    [SerializeField]
    private Collider myHitcollider;  //自身のHitAreaのCollider
    private HitArea myHitArea;

    private ManipulateSkillCollider manipuCollider_sc;
    private ShootRay shootRay = null;

    private List<int> sendMasterList;    //RPCでマスタに送る用
    private List<Collider> subList; //sendMessageで送る相手の参照の一時置き

    /// <summary>
    /// Hitタイミングを伝える対象のオブジェクト.
    /// </summary>
    [NonSerialized]
    public List<GameObject> sendObjects;

    private byte tempHitCount;   //そのs_info中にHitイベントが呼ばれた回数.

    void Start()
    {
        view = transform.root.GetComponent<PhotonView>();
        sendMasterList = new List<int>();
        subList = new List<Collider>();
        sendObjects = new List<GameObject>();
        myHitArea = myHitcollider.gameObject.GetComponent<HitArea>();
        combo = transform.root.GetComponent<CombinationBonus>();
        charaAnimation = transform.root.GetComponent<ICharaAnimation>();
        charaAnimationSync = transform.root.GetComponent<ICharaSkillAnimationSync>();
        shootRay = transform.root.GetComponent<ShootRay>();
        charaDelayTime = transform.root.GetComponent<ICharaDelayTime>();

        manipuCollider_sc = GetComponent<ManipulateSkillCollider>();
        manipuCollider_sc.EndEvent += CollisionEndHandler;
    }

    public bool CanStartSkill()
    {
        if (s_info != null)
        {
            return false;  //前のSkillが終わってなければ受け付けない
        }
        else return true;
    }

    //skillの発動.
    public async Task StartSkill(SkillInfoClass si, skillEndDel skillEndDele)
    {
        if (combo.IsWaitingTime)
        {
            combo.EndWaitingTimeSatisfyCombo();
            combo.IsSuccessCombo = true;
        }
        else
        {
            combo.IsSuccessCombo = false;
        }
        skillEnd = skillEndDele;
        s_info = si;     //OnTriggerEnterでの処理用に保管
        Debug.Log("<color=red><size=22> Start Skill " + si.absoluteSkillNum + " ! " + si.skillName + "    Skill Level:" + si.SkillLevel + " </size></color>");

        //コンボ成立か確認.成立ならDelay時間減少.
        float dtime = si.delayTime;
        if (combo.IsSuccessCombo)
        {
            dtime *= StaticMyClass.DelayTimeRateOfDecrease; //si.DelayTimeに直接かけるのは間違い.
            comboUI.ShowCombo();    //Combo表示
        }
        if ((si._skillType2 == SkillInfoClass.skillType2.baff || si._skillType2 == SkillInfoClass.skillType2.Revi) && !si.rangeAttack) MySelfBuffRevi(si);
        else manipuCollider_sc.StartAnimation(si);

        charaAnimationSync.SkillAnimationSync(si.absoluteSkillNum, si.SkillLevel);                //他端末自キャラとのSkillAnimation同期.

        await charaDelayTime.StartDelayTime(dtime);

        charaAnimation.PlaySkillAnimation(si.absoluteSkillNum, si.SkillLevel);        //SkillのAnimation開始
    }

    //自分自身Skill.
    private void MySelfBuffRevi(SkillInfoClass si)
    {
        sendMasterList.Add(view.ViewID);          //ここで自身を入れる処理.MasterHitSkillAssignmentのフローでai.sendEnemyList.Countを使っている.
        sendObjects.Add(this.transform.root.gameObject);
        myHitArea.MyHit(GetAttackInfoJson()); //buffReviは自分(skill使用者)が送る.
    }

    /// <summary>
    /// ManipulateSkillColliderの駆動によるOTE.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (s_info == null) return;
        if (other == myHitcollider) return;     //AttackAreaでの自分とのコリジョンは無視.

        if (CheckLayerEnemy(other))
        {
            //Enemyとのコリジョン
            TargetSelectPhase(other);
        }
        else
        {
            //他Playerとのコリジョン
            if (s_info._skillType2 == SkillInfoClass.skillType2.baff || s_info._skillType2 == SkillInfoClass.skillType2.Revi)
            {
                //Revi or Buff.
                if (pv.CheckPvProtectin(other)) TargetSelectPhase(other); //PVの確認.対象PlayerへBuffRevi使用可の場合True.

            }
            else
            {
                //Attack or Debuff.
                if (!pv.CheckPvProtectin(other)) TargetSelectPhase(other);       //PV可.
                                                                                 //PV不可能ならPlayerAttackとPlayerHitで発生するOnTriggerEnterはスルーする
            }


        }
    }


    protected void TargetSelectPhase(Collider other)
    {
        if (!s_info.rangeAttack)
        {
            if (shootRay.hitRayTarget == null)
            {
                manipuCollider_sc.ForceFinish();    //最初のOnTriggerEnterのみ処理
                AddSendList(other);                     //Listに追加
                SendMess(other);
                sendObjects.Add(other.transform.root.gameObject);
            }
            else     //Targeting中
            {
                //Targetingした相手とこのCollisionのColliderが同じ?
                if (shootRay.hitRayTarget == other)
                {
                    manipuCollider_sc.ForceFinish();
                    AddSendList(other);
                    SendMess(other);
                    sendObjects.Add(other.transform.root.gameObject);
                }
            }
        }
        else
        {
            if (!subList.Contains(other))
            {
                subList.Add(other);
                AddSendList(other);
                sendObjects.Add(other.transform.root.gameObject);
            }
        }
    }


    //EnemyとのコリジョンはTrue  他Playerとの場合はfalse
    protected bool CheckLayerEnemy(Collider other)
    {
        return LayerMask.NameToLayer("EnemyHit") == other.gameObject.layer ? true : false;
    }

    //skillColliderのCollision検知終了で呼ばれるイベントハンドラー.SkillStartから最大でも0.5sで呼ばれる. List処理へ.
    //範囲攻撃敵スキャン後このメソッドが呼ばれるとき敵の参照が既にない可能性あるかも  list検索して他に敵がいなければ攻撃キャンセル
    public void CollisionEndHandler()
    {
        Debug.Log("CollisionEndHandler");
        Debug.Log(s_info._skillType2);
        if (s_info._skillType2 == SkillInfoClass.skillType2.baff || s_info._skillType2 == SkillInfoClass.skillType2.Revi)
        {
            Debug.Log("baff or Revi");
            sendMasterList.Add(view.ViewID);            //ここで自身を入れる処理.MasterHitSkillAssignmentのフローでai.sendEnemyList.Countを使っている.
            sendObjects.Add(this.transform.root.gameObject); //ここで自身を入れる.
            myHitArea.MyHit(GetAttackInfoJson());       //buffReviは自分(skill使用者)が送る.
            return;
        }

        //Debug.Log("skillColliderのCollision検知終了で呼ばれました!");
        if (sendMasterList.Count == 0 || subList.Count == 0)
        {
            ClearData();
            return;
        }
        SendMess(); //範囲攻撃の場合、特に理由はないがAttack debuffの場合最初に当たった敵に送る.
    }

    //List登録
    private void AddSendList(Collider other)
    {
        //キーを指定してvalueを取り出し
        int colliderId = other.GetInstanceID();
        if (reference.enemyAndOtherPlayerViewIdTable.ContainsKey(colliderId))    //SearchAreaで捉えている場合のみ(捉えてないとむしろヘン)
        {
            int photonViewId = reference.enemyAndOtherPlayerViewIdTable[colliderId];
            sendMasterList.Add(photonViewId);       //送信用リストに追加
            Debug.Log("Add SendList!");
        }
    }


    private void SendMess(Collider other)
    {
        string attackerJsonData = GetAttackInfoJson();
        ExecuteEvents.Execute<RecieveInterface>(other.gameObject, null, (reciever, data) => reciever.OnRecieve(attackerJsonData));
    }


    //範囲攻撃の場合
    private void SendMess()
    {
        string attackerJsonData = GetAttackInfoJson();
        ExecuteEvents.Execute<RecieveInterface>(subList[0].gameObject, null, (reciever, data) => reciever.OnRecieve(attackerJsonData));
    }


    private void ClearData()
    {
        Debug.Log("ClearData()");
        this.sendMasterList.Clear();
        this.subList.Clear();
        sendObjects.Clear();
        tempHitCount = 0;
    }

    private string GetAttackInfoJson()
    {
        AttackInfoClass a = CalcAttackinfo();
        string StrJson = JsonUtility.ToJson(a);
        Debug.Log("Get Json:" + StrJson);
        return StrJson;
    }

    //SendMessageの引数にいれるものの処理　　SkillInfoClassのうち必要なものを加工・抜き出してRPCで送れるようにする
    private AttackInfoClass CalcAttackinfo()
    {
        AttackInfoClass ai = new AttackInfoClass();
        ai.AttackerBool = true;         //AttackerがPlayerの場合True.Enemyの場合False.
        ai.Attackerid = PhotonNetwork.LocalPlayer.ActorNumber;
        ai.absoluteSkillNum = s_info.absoluteSkillNum;      //Skillの絶対番号.
        ai.sendEnemyList = new List<int>(sendMasterList);   //命名が...範囲系のBuff&Reviの場合Playerが入る.
        return ai;
    }

    //SearchAreaのOnTriggerExitから呼ばれる　　
    public void EnemyLost(Collider other)
    {
        if (subList.Contains(other)) subList.Remove(other);
    }

    //Hit数が定義した回数に達した時SkillEnd()を呼ぶように修正.
    public void IncrementHitEvent()
    {
        if (StaticMyClass.OneHitOneDamageSkillAbs.Contains((byte)s_info.absoluteSkillNum))
        {
            ++tempHitCount;        //SkillAbs2,11,22,31は1Hit()で1つ.
            if (s_info.HitTime == tempHitCount) StartCoroutine(BackDelayTime());
        }
        else
        {
            StartCoroutine(BackDelayTime());
        }
    }

    /// <summary>
    /// 後Delay.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BackDelayTime()
    {
        float delayTime = StaticMyClass.AfterDelayTime;
        //コンボ成立か確認.成立ならDelay時間減少.
        if (combo.IsSuccessCombo) delayTime *= StaticMyClass.DelayTimeRateOfDecrease;

        Debug.Log("後Delay: " + delayTime);
        yield return new WaitForSeconds(delayTime);
        EndFire();
    }



    //SkillManagerにskillが終了したことを伝える    skillの当たった当たらなかったに関わらず送る
    private void EndFire()
    {
        this.s_info = null;
        ClearData();
        combo.StartingWaitingTimeSatisfyCombo();        //コンボ成立入力待ち時間開始.

        if (skillEnd != null)
        {
            Debug.Log("SkillManagerにskillが終了したことを伝える");
            skillEnd();
            // if(!(myDebug is null)) myDebug.SkillEnd();
        }
    }



    /// <summary>
    /// HitのタイミングでCharaAnimationEventからよばれる(Reviでもここが呼ばれる).OTEした相手にメッセージシステムで当たったことを送信.
    /// </summary>
    /// <param name="oneHitDequeueCount">1Hitしたときのデキュー回数.</param>
    public void SendHit()
    {
        Debug.Log("sendObjects.Count :" + sendObjects.Count);
        if (sendObjects.Count == 0) return;
        foreach (var target in sendObjects)
        {
            if (target == null) continue;
            Debug.Log("target.name:" + target.name);
            byte oneHitDequeueCount = (byte)s_info.HitTime;
            if (StaticMyClass.OneHitOneDamageSkillAbs.Contains((byte)s_info.absoluteSkillNum)) oneHitDequeueCount = 1;          //SkillAbs2,11,22,31は1Hit()で1つQueueを消費する.
            ExecuteEvents.Execute<IReceiveShowTiming>(target, null, (receiver, data) => receiver.OnReceiveHit(oneHitDequeueCount, SelectSendStatus(s_info)));
        }
    }

    /// <summary>
    /// 使用されたSkillのタイプを取得する.
    /// </summary>
    /// <param name="si"></param>
    /// <returns></returns>
    private StaticMyClass.SendStatus SelectSendStatus(SkillInfoClass si)
    {
        switch (si.condi)
        {
            case SkillInfoClass.Condition.None:
                if (si._skillType2 == SkillInfoClass.skillType2.Attack) return StaticMyClass.SendStatus.DamageOnly;
                else return StaticMyClass.SendStatus.Revi;

            case SkillInfoClass.Condition.StatusUpAll:
            case SkillInfoClass.Condition.StatusUp_Pwr:
            case SkillInfoClass.Condition.StatusUp_Dex:
            case SkillInfoClass.Condition.StatusUp_Def:
            case SkillInfoClass.Condition.StatusUp_Mat:
            case SkillInfoClass.Condition.StatusUp_Mde:
            case SkillInfoClass.Condition.StatusUp_Agi:
            case SkillInfoClass.Condition.VelocityUp:
                return StaticMyClass.SendStatus.UccOnly;

            case SkillInfoClass.Condition.StatusDownAll:
            case SkillInfoClass.Condition.StatusDown_Pwr:
            case SkillInfoClass.Condition.StatusDown_Dex:
            case SkillInfoClass.Condition.StatusDown_Def:
            case SkillInfoClass.Condition.StatusDown_Mat:
            case SkillInfoClass.Condition.StatusDown_Mde:
            case SkillInfoClass.Condition.StatusDown_Agi:
            case SkillInfoClass.Condition.VelocityDown:
            case SkillInfoClass.Condition.HpDown:
            case SkillInfoClass.Condition.paralyze:
                if (si._skillType2 == SkillInfoClass.skillType2.Attack) return StaticMyClass.SendStatus.DamagePlusUCC;
                else return StaticMyClass.SendStatus.UccOnly;

            default: return StaticMyClass.SendStatus.Revi;
        }
    }
}

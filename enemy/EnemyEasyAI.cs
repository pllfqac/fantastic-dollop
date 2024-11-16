using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Jobs;
using Unity.Collections;
using System.Threading.Tasks;
using System;

/// <summary>
/// debug用.
/// </summary>
public delegate void debugChangeStateHandler(string state);


/// <summary>
///Enemy. 
/// </summary>
public class EnemyEasyAI : MonoBehaviour
{

    //※Random使うときに必ずRandomManagerのCountUp()を呼ぶこと.

    //  public IMasterAllMemberTable mamt;      //Master用.高速アクセスの為.
    [NonSerialized]
    public IPlayerTable pTable;     //Mater用.
    private IRandomManager rm;

    public event debugChangeStateHandler easyAiChangeEvent;

    private EnemyAttackArea attackArea = null;
    private Transform AttackTarget = null;
    private EnemyMove enemyMove;                //HP0だった場合にEnemySpawnで呼ばれるとNullRefエラーが出るのでInspe.

    [System.NonSerialized]
    public MyPlayerDataRef playerDataRef;       //spawn

    [SerializeField]
    private float waitTime;
    [SerializeField]
    private float walkRange;            //小さすぎる値にはしないこと.

    [System.NonSerialized]
    public Vector3 basePosition;       //MasterからPositionをもらうのでNonSeri

    [SerializeField, Range(1, 100)]
    private float attackRange = 1;       //sqrMagnitudeと比較する.2乗済.

    private PhotonView view;
    private bool isRpcWait;         //falseでMasterへのRPC可能.
    private IEnumerator waitCoroutine;

    public State state;
    private State nextState;

    public enum State
    {
        walking,
        chasing,
        Attaking,
        died,
        paralyze,
    };


    //==============Editor用Mock==============  不要になったら消す.//Mock使用時はIMockEnemyEasyAIに処理が流れる.不要になれば消す.このスクの関連するifブロック等も消してOk.
    [Header("-----------Test用 EnemyAttackオンオフ--------------------")]
    [Tooltip("FalseでEnemyはAttackしてこない.")]
    public bool enableEnemyAttack;
    /// <summary>
    /// TestSceneで生成されるときにTrueになりMockRandomManagerを使用するようになる.
    /// </summary>
    [System.NonSerialized]
    public bool useMockRandomManager;
    private Vector3 debugEnemyPosition = new Vector3(26f, 0, 5f);

    //=========================================




    void Start()
    {
        isRpcWait = false;
        view = GetComponent<PhotonView>();
        AttackTarget = null;
        enemyMove = this.GetComponent<EnemyMove>();
        waitTime = StaticMyClass.WaitTime;
        //   basePosition = transform.position;//new Vector3(0, StaticMyClass.enemySpawnVectorY, 0);
        state = State.walking;
        nextState = State.walking;
        if (!useMockRandomManager) rm = GetComponent<RandomManager>();
        else rm = GetComponent<MockRandomManager>();

        attackArea = transform.Find("AttackArea").GetComponent<EnemyAttackArea>();

    }

    void Update()
    {

        switch (state)
        {
            case State.walking:
                Walking();
                break;
            case State.chasing:
                Chasing();
                break;
            case State.Attaking:
                Attacking();
                break;
            case State.paralyze:
                Paralyze();
                break;
        }

        if (state != nextState)
        {
            state = nextState;
            switch (state)
            {
                case State.walking: WalkStart(); if (easyAiChangeEvent != null) easyAiChangeEvent("walking"); break;
                case State.chasing: ChoseStart(); if (easyAiChangeEvent != null) easyAiChangeEvent("chasing"); break;
                case State.Attaking: AttackStart(); if (easyAiChangeEvent != null) easyAiChangeEvent("Attaking"); break;
                case State.died: Died(); if (easyAiChangeEvent != null) easyAiChangeEvent("died"); break;
                case State.paralyze: ParalyzeStart(); break;
            }
        }
    }



    public void WalkStart()
    {
        Debug.Log("WalkStart");
        if (state == State.paralyze)
        {
            ChangeState(State.walking);
            enemyMove.UpdateStop = false;        //もしparalyze状態で呼ばれたら解除.
            waitTime = 1.0f;
        }
        state = State.walking;
        //eAnimation.WalkingAnimationPlay();
    }

    private void Walking()
    {
        if (waitTime > 0.0f)
        {
            waitTime -= Time.deltaTime;

            if (waitTime <= 0.0)
            {
                if (rm.randomSyncWait)
                {
                    //目標 RC値に到達していたらTrue.RC増加無しでなんかする.
                    waitTime = StaticMyClass.syncWaitTime;          //仮　とりあえず待つ.
                                                                    // Debug.Log("とりあえず待つ");
                }
                else
                {
                    if (!enemyMove.Arrived()) return;
                    Vector2 randomValue = rm.GetVector2() * walkRange;
                    //    Debug.Log("basePosition"+ basePosition+"  v2:" + randomValue);
                    //	Vector3 destinationPosition = basePosition + new Vector3(randomValue.x, 0.0f, randomValue.y);

                    //目的地の指定.
                    enemyMove.SetDestination(basePosition + new Vector3(randomValue.x, StaticMyClass.enemySpawnVectorY, randomValue.y));
                }

            }
        }
        else
        {
            //目的地へ到達した場合,待機状態へ.
            if (enemyMove.Arrived())
            {
                // Debug.Log("待機状態()");
                if (rm.randomSyncWait) waitTime = StaticMyClass.syncWaitTime;   //目標 RC値に到達していたらTrue.RC増加無しで待つ.			
                else waitTime = rm.GetRangeNum(StaticMyClass.WaitTime, StaticMyClass.WaitTime * 1.5f);
            }

            //Walking中にTargetを発見したら追跡.
            if (AttackTarget) ChangeState(State.chasing);
        }
    }



    private void AttackStart()
    {
        //debug用
        if (!enableEnemyAttack)
        {
            ChangeState(State.chasing);
            return;
        }

        enemyMove.SetDirection(AttackTarget.position);
        if (attackArea != null) attackArea.Attack();     //Masterはdestroyしてる.
        state = State.Attaking;
    }

    private void Attacking()
    {
        if (!attackArea.Attacking()) ChangeState(State.chasing);
    }

    //uccから呼ばれる.
    public void ParalyzeStart()
    {
        state = State.paralyze;
        ChangeState(State.paralyze);
        enemyMove.UpdateStop = true;
        enemyMove.StopMove();
        //	eAnimation.WalkingAnimationStop();
    }

    private void Paralyze()
    {
        /*何も市内*/
    }


    private void ChoseStart()
    {
        state = State.chasing;
    }

    private void Chasing()
    {
        if (AttackTarget == null)
        {
            ChangeState(State.walking);
            return;
        }

        enemyMove.SetDestination(AttackTarget.position);        //移動先をPlayerに設定.

        //設定範囲内に近づいたら攻撃.
        //   Debug.Log((AttackTarget.position - transform.position).sqrMagnitude);         //最小で17くらいになる.
        if ((AttackTarget.position - transform.position).sqrMagnitude <= attackRange)
        {
            enemyMove.StopMove();           //範囲内に入ったら移動は終了して攻撃に移る.
            ChangeState(State.Attaking);
        }
    }

    public void DiadEnemy()
    {
        enemyMove.StopMove();
        enemyMove.UpdateStop = true;
        ChangeState(State.died);
    }

    private void Died()
    {
        AttackTarget = null;
    }

    public void Reborn()
    {
        AttackTarget = null;
        enemyMove.UpdateStop = false;
        waitTime = StaticMyClass.WaitTime;
        basePosition = transform.position;
        state = State.walking;
        nextState = State.walking;
    }

    private void ChangeState(State nextStae)
    {
        this.nextState = nextStae;
    }

    public void DeleteTarget(Collider other)    //PlayerのHitAreaのColl
    {
        AttackTarget = null;

        // if (myMock!=null) return;

        //自端末の自キャラの場合OTExitの同期RPC飛ばす.
        if (other.transform.gameObject.GetInstanceID() == playerDataRef.myHitAreaGameObjectInstanceID) view.RPC("DeleteAttackTarget", RpcTarget.Others);

        //Masterのみ.ATになっているPlayerがwarpしたとき用のEvent解除.
        if (PhotonNetwork.IsMasterClient) other.transform.root.GetComponent<SyncScene>().AfterOtherEvent -= DeleteAttackTarget;
    }

    [PunRPC]
    private void DeleteAttackTarget()
    {
        AttackTarget = null;
    }


    //攻撃対象を指定する.
    public void SetAttackTarget(Transform target)           //引数のTransformはHitArea
    {
        /*OnTriggerStay()でのsetAttackTarget()は自端末の自キャラのみ.自端末別キャラへのATはRPCのみで入る*/
        if (PhotonNetwork.IsMasterClient || target.gameObject.GetInstanceID() != playerDataRef.myHitAreaGameObjectInstanceID) return;
        //以下自端末自キャラのみ.すでにATになっているときはそれを優先.自キャラの場合はRPCの連続送信を避ける.
        if (AttackTarget == null && !isRpcWait)
        {
            SyncEnemyStateRPC(target);
        }
    }


    #region AttackTargetの同期

    /// <summary>
    /// EnemyのAttackTargetにPlayer参照が入った場合の他端末との同期RPC.
    /// RPCは自端末自キャラのみ(仮
    /// RPCの目的:間接的な位置の同期 + AttackTargetの同期.
    /// </summary>
    private void SyncEnemyStateRPC(Transform target)      //targetはPlayerのHitArea
    {
        PhotonView targetView = target.root.GetComponent<PhotonView>();
        if (targetView.IsMine)
        {
            waitCoroutine = RpcWaitCoroutine();
            StartCoroutine(waitCoroutine);
            //PhotonTargets.AllViaServerを使いたかったがinterestgroupではだめっぽい.viaの代わりにMasterで判断.
            view.RPC(nameof(SyncJudgeByMaster), RpcTarget.MasterClient, targetView.ViewID);    //Enemyではなく自PlayerのviewIDを送信.
        }
    }

    //RPC連続送信防止用コルーチン.
    private IEnumerator RpcWaitCoroutine()
    {
        isRpcWait = true;
        yield return new WaitForSeconds(StaticMyClass.rpcWaitTime);
        isRpcWait = false;
    }

    //僅差でRPCが来た場合,先のが入る仕様とする.
    [PunRPC]
    private void SyncJudgeByMaster(int playerViewId)
    {
        if (AttackTarget != null || isRpcWait) return;
        waitCoroutine = RpcWaitCoroutine();
        StartCoroutine(waitCoroutine);      //しばらく受け付けない.
        AttackTarget = pTable.FindUserObjectbyPhotonViewId(playerViewId).obj.transform; //mamt.MasterGetPlayerRef(playerViewId).transform;     //PhotonView.Findより速いと踏んだため.
        if (AttackTarget == null) return;   //対象のPlayerGameObjectが見つからない場合は何もしない.
                                            //      AttackTarget = PhotonView.Find(playerViewId).transform;
                                            //Masterのみ.ATになっているPlayerがwarpしたときEnemyはそのままではAT=NullにならないのでNullにする必要がある.
        if (PhotonNetwork.IsMasterClient) AttackTarget.GetComponent<SyncScene>().AfterOtherEvent += DeleteAttackTarget;
        view.RPC("SyncEnemyAttackTarget", RpcTarget.Others, playerViewId);
    }

    //Master以外.Masterが最初に受信したviewIDのPlayerキャラをこのEnemyのAttackTargetにする.
    [PunRPC]
    private void SyncEnemyAttackTarget(int targetPlayerViewID)        //(7byte*20)/h =8.4kbyte  5byte→6kbyteくらい.RCの同期が必要ならする.
    {
        waitCoroutine = RpcWaitCoroutine();
        StartCoroutine(waitCoroutine);
        AttackTarget = PhotonView.Find(targetPlayerViewID).transform;
    }

    #endregion
}

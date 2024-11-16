using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using HC.Debug;
using Photon.Pun;
using UnityEngine.EventSystems;


public delegate void MySkill();     //使用するskillの登録用.

/// <summary>
/// Enemy.
/// AttackAreaのColliderを制御する.
/// </summary>
public class EnemyAttackArea : MonoBehaviour
{

    [NonSerialized]
    public PhotonView view;         //EnemySpawnで生成時にもらう.
    private MySkill skill;
    private EnemySkill enemySkill;
    private bool skillRangeAttack;
    private bool attacking;         //コライダ動作中 or endDelayTime中は True.

    private AttackInfoClass aic;
    private IEnemyMyAnimation eAnimator;
    private EnemySkillClass skillClass;


    private void Awake()
    {
        enemySkill = this.GetComponent<EnemySkill>();
    }

    private void Start()
    {
        enemySkill.rangeEnd += AttackEndHandler;
        eAnimator = transform.root.GetComponent<IEnemyMyAnimation>();
        //skillの登録.
        skill = enemySkill.Skill;
        skillRangeAttack = enemySkill.eSkillClass.rangeAttack;

        aic = new AttackInfoClass();
        aic.AttackerBool = false;         //AttackerがPlayerの場合True.Enemyの場合False. PlayerAの端末上のPlayerBがEnemeyに攻撃されないようにするため.
        skillClass = transform.root.GetComponent<EnemyData>().EnemySkillClass;
    }

    public bool Attacking()
    {
        return attacking;
    }

    public void Attack()
    {
        eAnimator.AttackAnimationPlay();
        attacking = true;
        skill();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (skillRangeAttack)
        {
            SendHit(other);
        }
        else
        {
            enemySkill.ForceFinish();
            StartCoroutine(EndDelayEnumerator());
            SendHit(other);
        }
    }

    public void AttackEndHandler()
    {
        //後Delay開始.
        StartCoroutine(EndDelayEnumerator());
    }



    private string GetAttackInfoJson()
    {
        aic.Attackerid = view.ViewID;       //Enemyの場合viewID.
        string StrJson = JsonUtility.ToJson(aic);
        return StrJson;
    }



    public void SendHit(Collider other)
    {
        ExecuteEvents.Execute<RecieveInterface>(other.gameObject, null, (reciever, data) => reciever.OnRecieve(GetAttackInfoJson()));
    }


    //後ディレイ.
    private IEnumerator EndDelayEnumerator()
    {
        yield return new WaitForSeconds(skillClass.endDelayTime);
        attacking = false;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;


/// <summary>
/// Enemy.
/// Status,Skilの参照を持つ.
/// </summary>
public class EnemyData : MonoBehaviour, IEnemyStatusAddCompornent
{
    /// <summary>
    /// 使用するSkillの番号.
    /// Inspeで設定しておく.
    /// </summary>
    [SerializeField]
    private int _useSkillNumber;
    public int useSkillNumber { get { return _useSkillNumber; } }


    //今のところMasterOnly.
    public byte _sceneIndex { get; set; }

    public int enemyID { get; private set; }

    public EnemyStatus Status { get; private set; }


    public EnemySkillClass EnemySkillClass { get; private set; }

    /// <summary>
    /// 生成時に入る.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="eStatus"></param>
    /// <param name="esc"></param>
    public void EnemyDataInit(int id, EnemyStatus eStatus, EnemySkillClass esc)
    {
        this.enemyID = id;
        Status = eStatus;
        EnemySkillClass = esc;
        GetComponent<EnemyHP>().HP = Status.maxhp;
    }

    /// <summary>
    /// Statusの取得.
    /// 引数にデータ入れて返す.
    /// </summary>
    /// <param name="csc"></param>
    public void GetEnemyStatus(CommonStatusClass csc)
    {
        csc.maxHp = this.Status.maxhp;
        csc.maxMp = 0;          //EnemyはMPは関係なし.
        csc.Power = this.Status.pwr;
        csc.Dexerity = this.Status.dex;
        csc.Defense = this.Status.def;
        csc.MagicAttack = this.Status.mat;
        csc.MagicDefense = this.Status.mde;
        csc.Agility = this.Status.agi;
    }

    public EnemySkillClass GetEnemySkills()
    {
        return this.EnemySkillClass;
    }
}

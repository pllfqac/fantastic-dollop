using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


/// <summary>
/// EnemySkill定義用Class.
/// </summary>
[Serializable]
public class EnemySkillClass 
{

    [ReadOnly]
    /// <summary>
    /// Enemy版SkillAbs.
    /// インスペで設定したEnemy->AttackAreaのEnemySkill.useSkillNumberと同じ値のSkillが選択・実行される.
    /// </summary>
    public  int EnemySkillNumber;

    [ReadOnly]
    public bool rangeAttack;

    //skill拡大率.
    [ReadOnly,SerializeField, Range(1.0f, 10.0f)]
    private byte _collMagnificationRatioSkill;
    public byte CollMagnificationRatioSkill
    {
        get { return this._collMagnificationRatioSkill; }
        set { this._collMagnificationRatioSkill = value; }
    }

    //skill終了時間.
    [ReadOnly,SerializeField, Range(0.1f, 10.0f)]
    private float _skillEndTime;
    public float skillEndTime
    {
        get { return this._skillEndTime; }
        set { this._skillEndTime = value; }
    }

    /// <summary>
    /// DelayTime.
    /// </summary>
    [ReadOnly,SerializeField, Range(0.0f, 3.0f)]
    private float _skillDelayTime;
    public float skillDelayTime
    {
        get { return this._skillDelayTime; }
        set { this._skillDelayTime = value; }
    }

    /// <summary>
    /// Enemyの攻撃間隔調整用.
    /// Attackの後に呼ばれる.
    /// </summary>
    [ReadOnly]
    public float endDelayTime;

    [ReadOnly]
    public SkillInfoClass.Condition Condition;

    //Skill関連の値もこのスクに入れる. BuffAndDebuffスクで値をSetしている?
    [ReadOnly]
    public int HitTime;         //Hit回数. 未実装.
    [ReadOnly]
    public int uccDurationTime;    //Skill継続時間.
    [ReadOnly]
    public int skillAccuracy;   //skill命中率補正値.0～10(仮).0でも問題ない値.
}

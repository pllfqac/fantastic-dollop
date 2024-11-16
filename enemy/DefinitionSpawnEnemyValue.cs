using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// 1つのSceneのEnemy生成数定義用.
/// </summary>
[Serializable]
public class DefinitionSpawnEnemyValue {

    //生成対象のEnemyID.
    [SerializeField]
    private int _enemyId;
    public int EnemyId
    {
        get { return this._enemyId; }
        set { this._enemyId = value; }
    }

    //生成数.
    [SerializeField]
    private byte _spawnValue;
    public byte SpawnValue
    {
        get { return this._spawnValue; }
        set { this._spawnValue = value; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;


//廃止

/// <summary>
/// EnemyAllListのListの要素.
/// Enemy定義用Class.
/// </summary>
[Serializable]
public class EnemyAddressablesRef {


    [SerializeField]
    private int _enemyID;
    public int EnemyID
    {
        get { return this._enemyID; }
        set { this._enemyID = value; }
    }

    //Addressables参照
    [SerializeField]
    private AssetReference  _enemyAASRef;
    public AssetReference  EnemyAASRef
    {
        get { return this._enemyAASRef; }
        set { this._enemyAASRef = value; }
    }
    
}

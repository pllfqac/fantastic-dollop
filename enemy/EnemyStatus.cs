using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;

/// <summary>
/// EnemyのStatus定義.
/// </summary>
[Serializable]
public class EnemyStatus
{


    [SerializeField]
    private int _id;
    public int id { get { return _id; } }

    [SerializeField]
    private string _enemyName;
    public string EnemyName { get { return _enemyName; } }

    [SerializeField]
    private int _level;
    public int level { get { return _level; } }


    [SerializeField]
    private int _maxhp;
    public int maxhp { get { return _maxhp; } }


    [SerializeField]
    private int _pwr;
    public int pwr { get { return _pwr; } }

    [SerializeField]
    private int _dex;
    public int dex { get { return _dex; } }

    [SerializeField]
    private int _def;
    public int def { get { return _def; } }

    [SerializeField]
    private int _mat;
    public int mat { get { return _mat; } }

    [SerializeField]
    private int _mde;
    public int mde { get { return _mde; } }

    [SerializeField]
    private int _agi;
    public int agi { get { return _agi; } }


}

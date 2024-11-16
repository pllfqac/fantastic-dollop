using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy.
/// 実行時に使用する.
/// 定義はScriptableで.
/// </summary>
public class EnemyHP : MonoBehaviour
{

    private int hp;
    public int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp < 0) hp = 0;
        }
    }

}

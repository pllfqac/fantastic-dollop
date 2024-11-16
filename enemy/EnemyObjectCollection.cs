using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// Single2.
/// EnemyAllListを元にAASLoadしたEnemyObjの参照を保持する.
/// </summary>
public class EnemyObjectCollection : MonoBehaviour
{

    /// <summary>
    /// TKey:EnemyID.
    /// </summary>
    public ReadOnlyDictionary<int, GameObject> EnemyObjCollection;
}

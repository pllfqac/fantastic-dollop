using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Mock使用での処理をまとめた.
/// </summary>
public interface IMockEnemyEasyAI  {

    Transform SyncEnemyStateRPC(Transform target,Transform attackedTarget);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EnemyStartPositionオブジェ.
/// 生成するEnemyのIDを設定する.
/// </summary>
public class EnemyStartPosition : MonoBehaviour
{
    [SerializeField]
    private int sceneIndex;
    [SerializeField]
    private int spawnEnemyID;
    
    public int SpawnEnemyID { get { return spawnEnemyID; } }
    public int SpawnSceneIndex { get { return sceneIndex; } }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Masterが高速にアクセスするために持つ参照をまとめたClass.
/// Enemy生成するScene1つにこのスク1つ.
/// </summary>
public class MasterManagerEnemyDataRef  {

    public byte sceneIndex;
    public string sceneName;        //参考までに保持.

    

    //Scene別にEnemy1体ずつのdateを保存したTable.TKey:PhotonViewID
    public Dictionary<int, OneEnemyClass> oneEnemyClassTable = new Dictionary<int, OneEnemyClass>();

    //TKey:PhotonViewID
    public Dictionary<int, EnemyData> enemyDataTable = new Dictionary<int, EnemyData>();
    
    //RandomManagerのList
    public List<RandomManager> randomManagerList = new List<RandomManager>();
}

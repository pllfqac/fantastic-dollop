using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// Master用.
/// Scene別の生成EnemyのIDと生成数の定義.
/// MasterNumberOfEnemiesSpawnBySceneからこれに変更.
/// </summary>
[CreateAssetMenu(menuName = "MyGame/Create EnemySpawnDefineTable", fileName = "EnemySpawnDefineTable")]
public class EnemySpawnDefineTable : ScriptableObject {
    
    
    /// <summary>
    /// Scene別の生成EnemyのIDと生成数の定義.
    /// </summary>
    [SerializeField]
    private List<SpawnEnemyDefine> SpawnEnemyDefineTable = null;
    


    /// <summary>
    /// 対象シーンの生成定義を返す.
    /// </summary>
    /// <param name="sceneName">自作定義SceneDefinitionのSceneName</param>
    /// <returns>TKey:EnemyID. TValue:生成数.</returns>
    public Dictionary<int, byte> GetSpawnEnemyDefine(string sceneName)
    {
        SpawnEnemyDefine sed = SpawnEnemyDefineTable.First(x => x.SceneName == sceneName);      //Sceneの選択.
        return sed.SpawnEnemyData.ToDictionary(x => x.EnemyId, x => x.SpawnValue);
    }
    

	//Debug用.Enemyの生成数を変更する.
	public void ChangeEnemySpawnValue(byte changeValue)
	{
		Debug.Log("Debug用.Enemyの生成数を変更する");
		SpawnEnemyDefineTable.First(x => x.SceneName == "Stage3").SpawnEnemyData.First(id => id.EnemyId == 1).SpawnValue = changeValue;
	}
}

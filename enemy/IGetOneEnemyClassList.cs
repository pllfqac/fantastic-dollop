using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// シナリオシーンの同じPTのみ(ソロ可)のEnemyのリストを取得.
/// </summary>
public interface IGetOneEnemyClassList 
{
    List<OneEnemyClass> GetOneEnemyListforScenarioScene(int sceneIndex, int userId);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//廃止
public interface IMasterManager 
{
    void MasterEnemySpawnReqest(byte sceneIndex, int senderId);
    IEnemyStatusAddCompornent GetEnemyStatus(int sceneIndex, int viewID);
	IEnemyStatusAddCompornent GetEnemyStatus(int viewID);
	UnusualStatusClass GetEnemyUsc(int SceneIndex, int viewID);
}

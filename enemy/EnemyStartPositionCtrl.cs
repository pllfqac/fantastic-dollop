using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;


/// <summary>
/// Single2.
/// MasterOnly.
/// Enemy生成位置の保持と使用.
/// </summary>
public class EnemyStartPositionCtrl : MonoBehaviour,IEnemyStartPositionCtrl
{

    //Scene別にまとめたEnemy生成位置.
    //Tkey:SceneIndex.TValue:EnemyStartPositionオブジェ
    private Dictionary<int, List<GameObject>> spawnPos;


    private void Start()
    {
        if(!PhotonNetwork.IsMasterClient ) {
            Debug.Log("esp destroy");
            Destroy(this);
            return;
        }
        else
        {

            Debug.Log("ESP");
            spawnPos = new Dictionary<int, List<GameObject>>();

            //MainSceneに定義した全EnemyStartPositionオブジェをFind.Dictionaryを生成.
            Transform[] positions = GameObject.FindWithTag("EnemyStartPosition").GetComponentsInChildren<Transform>();

            Debug.Log("ESP.count:" + positions.Count());
            foreach(var p in positions)
            {
                Debug.Log("ESP Name:" + p.name);
            }

            foreach (var espObj in positions)
            {
                EnemyStartPosition esp = espObj.GetComponent<EnemyStartPosition>();
                if (esp == null) continue;
                if (spawnPos.ContainsKey(esp.SpawnSceneIndex)) spawnPos[esp.SpawnSceneIndex].Add(espObj.gameObject);
                else spawnPos.Add(esp.SpawnSceneIndex, new List<GameObject>() { espObj.gameObject });

            }

            foreach (var n in spawnPos)
            {
                Debug.Log("SceneIndex: " + n.Key + "  Count:" + n.Value.Count);
            }
        }   
    }

    /// <summary>
    /// StageとEnemyIDを指定して,対応するESPを返す.
    /// 複数のESPがあるときはランダムで選択する.
    /// 対応するESPがない場合はnull.
    /// </summary>
    /// <param name="enemyId"></param>
    /// <returns></returns>
    public Vector2 GetESP(int enemyId, int sceneIndex)
    {
        if (spawnPos.ContainsKey(sceneIndex))
        {
            List<GameObject> spawnLis = spawnPos[sceneIndex];
            if (spawnLis.Any(x => x.GetComponent<EnemyStartPosition>().SpawnEnemyID == enemyId))
            {
                var targetEnemys = spawnLis.Where(s => s.GetComponent<EnemyStartPosition>().SpawnEnemyID == enemyId).ToList();
                return GetTr(targetEnemys[Random.Range(0, targetEnemys.Count)]);
            }
            else
            {
                return StaticMyClass.defaultSpawnPos;
            }
        }
        else
        {
            return StaticMyClass.defaultSpawnPos;
        }
    }

    /// <summary>
    /// ESPを中心とした円の内からランダムな位置を返す.
    /// </summary>
    /// <param name="esp"></param>
    /// <returns></returns>
    private Vector2 GetTr(GameObject esp)
    {
        Vector2 v2 = Random.insideUnitCircle * StaticMyClass.enemySpawnPositionMaxAreaSize;     
        Vector2 result= new Vector2(esp.transform.position.x + v2.x, esp.transform.position.z + v2.y);   //2Dを3Dに変換.
        Debug.Log("元ESP:" + esp.transform.position + "  V2:" + result);
        return result;
    }
}

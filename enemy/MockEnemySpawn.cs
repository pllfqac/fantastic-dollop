using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Test用のEnemySpawn
public class MockEnemySpawn : MonoBehaviour
{
    [SerializeField]
    private int spawnValue;
    //生成したいEnemyのID.
    [SerializeField]
    private int spawnEenmyID;

    //Scriptable
    [SerializeField]
    private EnemyAllList enemyAllList;

    [SerializeField]
    private EnemyStatusDefinition enemyStatusDefine;     //Scriptableで定義したStatus.
    [SerializeField]
    private EnemySkillDefine enemySkillDefine = null;     //Inspe.


    public void Start()
    {
       // StartCoroutine(ManySpawn());
    }



  /*  private IEnumerator ManySpawn()
    {
        for (int i = 0; i < spawnValue; i++)
        {
            yield return null;
            GameObject obj = Instantiate(enemyAllList.GetEnemyObjRef(spawnEenmyID), Vector3.zero, Quaternion.identity) as GameObject;
            obj.GetComponent<EnemyEasyAI>().useMockRandomManager = true;
            EnemyData ed = obj.GetComponent<EnemyData>();
            Debug.Log("useSkillNumber:" + ed.useSkillNumber);
            ed.EnemyDataInit(1, enemyStatusDefine.GetEnemyDefineStatus(1), enemySkillDefine.GetEnemySkillDefinition(ed.useSkillNumber));
        }
    }*/
}

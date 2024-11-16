using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//廃止


[CreateAssetMenu(menuName = "MyGame/Create EnemyStatusList")]

public class EnemyStatusDefinition : ScriptableObject
{

    /// <summary>
    /// Enmey共通の値なのでScriptableにまとめたもの.
    /// </summary>
    [SerializeField]
    private List<EnemyStatus> enemyStatuses = new List<EnemyStatus>();




    /// <summary>
    /// 対応するIDのEnemyを取得.
    /// </summary>
    /// <param name="enemyId"></param>
    /// <returns></returns>
    public EnemyStatus GetEnemyDefineStatus(int enemyId)
    {
        return enemyStatuses.First(x => x.id == enemyId);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 全Enemyの参照.
/// </summary>
[CreateAssetMenu(menuName = "MyGame/Create EnemyAllList", fileName = "EnemyAllListSc")]
public class EnemyAllList : ScriptableObject
{
    /// <summary>
    /// Inspeで設定する.
    /// 個別Enemyオブジェの元になるGameObjectのリスト.
    /// </summary>
    [SerializeField]
    private List<EnemyDefinition> enemyDefinitions;
    public IReadOnlyList<EnemyDefinition> EnemyDefinitions { get { return enemyDefinitions; } }

    /// <summary>
    /// 指定したUnitTypeに当てはまるEnemy定義をすべて返す.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>全Typeあるはず.</returns>
    public IEnumerable<EnemyDefinition> GetEnemyDefinitionsbyUnitType(UnitStatus.UnitType type)
    {
        return enemyDefinitions.Where(x => x.UnitType == type);
    }

}

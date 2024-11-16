using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// EquipmentのEquipmentStatus.level2以上の定義.
/// </summary>
[CreateAssetMenu(menuName = "MyGame/Create EquipmentStatusDefinitionbyLevelTwoOver", fileName = "EquipmentStatusDefinitionbyLevelTwoOver")]
public class EquipmentStatusDefinitionbyLevelTwoOver : ScriptableObject, ICheckRequiredPlayerLevel, IGetEquipStatusDefinitionTwoOver
{

    [SerializeField]
    public List<EquipmentBaseStatusDefinition> equipmentBaseStatusesbyLevelTwoOverList = new List<EquipmentBaseStatusDefinition>();

    public IReadOnlyList<EquipmentBaseStatusDefinition> StatusDefineList { get { return equipmentBaseStatusesbyLevelTwoOverList; } }

    /// <summary>
    /// 引数で指定した(Equipの)AbsとeLevelからこのスクで定義したEquipを見つけ出し、装備可能Levelか否かを確認する.
    /// 装備可能ならTrue.不可ならFalse.
    /// このスクで定義していない場合はEquipmentLevel=1(=未合成.店の商品とか)とみなし常時True.
    /// </summary>
    /// <param name="abs">装備しようとするEquipのAbs.</param>
    /// <param name="equipmentLevel">装備しようとするEquipのEquipmentLevel.</param>
    /// <param name="playerLevel">playerの現在のLevel.</param>
    /// <returns></returns>
    public bool CanEquipbyNecessaryPlayerLevel(int abs, int equipmentLevel, int playerLevel)
    {
        if (equipmentLevel == 1) return true;
        if (!equipmentBaseStatusesbyLevelTwoOverList.Any(x => x.AbsoluteNumber == abs)) return true;
        //装備しようとするEquipの定義を取得.
        EquipmentBaseStatusDefinition eDef = equipmentBaseStatusesbyLevelTwoOverList.Where(x => x.AbsoluteNumber == abs).FirstOrDefault(x => x.equipmentLevel == equipmentLevel);
        Debug.Log("Equipment CanEquipLevel:" + eDef.CanEquipLevel);
        if (eDef == null) return true;  //このスクでは定義していない.
        if (eDef.CanEquipLevel <= playerLevel) return true;
        else return false;
    }

    /// <summary>
    /// 指定した引数に当てはまるEquipStatus定義を返す.
    /// 無ければ例外.
    /// </summary>
    /// <param name="abs"></param>
    /// <param name="equipLevel"></param>
    /// <returns></returns>
    public EquipmentBaseStatusDefinition GetStatusDefinitionTwoOver(int abs, int equipLevel)
    {
        if (equipLevel == 1 || !equipmentBaseStatusesbyLevelTwoOverList.Any(x => x.AbsoluteNumber == abs)) throw new System.Exception();
        return equipmentBaseStatusesbyLevelTwoOverList.Where(x => x.AbsoluteNumber == abs).First(x => x.equipmentLevel == equipLevel);
    }
}

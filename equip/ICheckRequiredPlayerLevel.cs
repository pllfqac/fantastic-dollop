using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 装備可能Levelの確認.
/// </summary>
public interface ICheckRequiredPlayerLevel
{
    /// <summary>
	/// 装備可能Levelかの確認.
	/// 装備可能ならTrue.不可ならFalse.
    /// </summary>
    /// <param name="abs">装備しようとするEquipのAbs.</param>
    /// <param name="equipmentLevel">装備しようとするEquipのEquipmentLevel.</param>
    /// <param name="playerLevel">playerの現在のLevel.</param>
    /// <returns></returns>
    bool CanEquipbyNecessaryPlayerLevel(int abs, int equipmentLevel, int playerLevel);
}
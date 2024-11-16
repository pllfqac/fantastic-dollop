using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single2.
/// 装備のStatusをEquipmentBaseStatusを元に決定する.
/// </summary>
public interface IEquipmentStatuGenerator  {

    /// <summary>
    /// MasterOnly.
    /// αを算出して定義したBase値に加えるVer.
    /// EquipmentLevelを省略した場合は常にeLevel=1で処理.
    /// </summary>
    /// <param name="absNum"></param>
    /// <returns></returns>
    EquipmentStatus GenerateEquipStatus(byte absNum, byte equipmentLevel = 1);


    /// <summary>
    /// 定義した値をそのままEquipmentStatusにコピーして取得するVer.
    /// EquipmentLevelを省略した場合は常にeLevel=1で処理.
    /// </summary>
    /// <param name="abNum"></param>
    /// <returns></returns>
    EquipmentStatus GetbaseEquipmentStatus(byte absNum, byte equipmentLevel = 1);
}

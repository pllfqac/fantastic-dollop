using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 装備中の装備のEquipmentStatusを取得する.
/// </summary>
public interface IEquipmentStatusAccess  {

    /// <summary>
    /// 装備中の全ての装備のEquipmentStatusをCommonStatusClassにまとめて取得する
    /// </summary>
    /// <returns></returns>
    void GetEquipmentAllStatus(CommonStatusClass csc);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRandomManager  {

    bool randomSyncWait { get; set; }

    /// <summary>
    /// 半径 1 の円の内部のランダムな点を返します.
    /// </summary>
    /// <returns></returns>
    Vector2 GetVector2();

    /// <summary>
    /// 指定された範囲の乱数を返却.
    /// </summary>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <returns></returns>
    float GetRangeNum(float num1, float num2);
}

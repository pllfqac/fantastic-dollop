using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// EnemyEasyAI.useMockRandomManager=True ならRandomManagerがついていても無視する.
/// </summary>
public class MockRandomManager : MonoBehaviour, IRandomManager
{

    [SerializeField]
    private bool randomWaitBool;

    /// <summary>
    /// 目標値に到達したらTrue.
    /// </summary>
     public bool randomSyncWait
    {
        get { return randomWaitBool; }
        set { }
    }

    /// <summary>
    /// 指定された範囲の乱数を返却.
    /// </summary>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <returns></returns>
    public float GetRangeNum(float num1=0.2f, float num2=1.0f)
    {
        float r= UnityEngine.Random.Range(num1, num2);
 //       Debug.Log("Mock Random:" + r);
        return r;
    }


    /// <summary>
    /// 半径 1 の円の内部のランダムな点を返します.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetVector2()
    {
        Vector2 v2 = UnityEngine.Random.insideUnitCircle;
    //    Debug.Log("Mock Random:" + v2);
        return v2;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Threading;

/// <summary>
/// Single2.
/// Field Item用再生タイマーの保存リスト.
/// Scene切替での再生時間保存のためSingleで持つ.
/// </summary>
public class FieldItemSingleton : MonoBehaviour
{
    /// <summary>
    /// 再生待ちFieldItemのタイマーリスト.
    /// </summary>
    private List<int> waitLocationList = new List<int>();


    /// <summary>
    /// 再生待ちタイマーListに入っているかの確認.
    /// </summary>
    /// <param name="locationNum"></param>
    /// <returns>Listに入っていればTrue.</returns>
    public bool ExistWaitList(int locationNum)
    {
        return waitLocationList.Contains(locationNum);
    }


    /// <summary>
    /// 再生待ちタイマー起動.
    /// </summary>
    /// <param name="locationNum"></param>
    /// <param name="waitTime">[s]</param>
    /// <returns></returns>
    public async Task FieldItemWait(int locationNum, int waitTime)
    {
        waitLocationList.Add(locationNum);          //Listに追加して
        await Task.Delay(waitTime * 1000);             //再生待ち.ミリ秒に変換.
        waitLocationList.Remove(locationNum);       //Listから取り除く.
    }


}

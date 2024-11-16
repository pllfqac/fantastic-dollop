using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listクラスの拡張.
/// </summary>
public static class ListExtension
{
    //ttps://takap-tech.com/entry/2017/09/15/083300

    /// <summary>
    /// 指定した位置の要素を指定したインデックス位置に変更します。
    /// </summary>
    /// <param name="newIndex">移動させたい要素のIndex.</param>
    /// <param name="oldIndex">移動後のIndex.</param>
    public static void ChangeOrder<T>(this List<T> list, int oldIndex, int newIndex)
    {
        if (newIndex > list.Count - 1)
            throw new ArgumentOutOfRangeException(nameof(newIndex));

        if (oldIndex == newIndex) return;

        T item = list[oldIndex];
        list.RemoveAt(oldIndex);

        if (newIndex > list.Count)
        {
            list.Add(item);
        }
        else
        {
            list.Insert(newIndex, item);
        }
    }

    /*
     シンプルな List<int> の場合、以下のように使用します。
        var list = new List<int>() { 0, 1, 2, 3, 4 };
        list.ChangeOrder(0, 4); // 0番目の要素を4番目に移動
        // > 1 2 3 4 0
        list.ChangeOrder(4, 1); // 4場番目の要素を1番目に移動
        // > 1 0 2 3 4
     */


    //=================================================================================
    //ランダム取得
    //=================================================================================

    /// <summary>
    /// ランダムに取得する
    /// </summary>
    public static T GetAtRandom<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            Debug.LogError("リストが空です！");
        }

        return list[UnityEngine.Random.Range(0, list.Count)];
    }



    /// <summary>
    /// ランダムに取得し、リストから消す
    /// </summary>
    public static T GetAndRemoveAtRandom<T>(this List<T> list)
    {
        T target = list.GetAtRandom();
        list.Remove(target);
        return target;
    }
}

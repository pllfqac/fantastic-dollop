using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Fieldアイテムオブジェクト.
/// </summary>
public interface IFieldItemProperty
{

    /// <summary>
    /// このFieldItemObjectがTapされたとき.
    /// 設定されたFieldItemのデータを取得する.
    /// </summary>
    /// <returns>isWait:待ち時間がある時はTrue.</returns>
    (bool isWait, int installationLocationNum, int abs,int requiredItem) GetFieldItemProp();
}
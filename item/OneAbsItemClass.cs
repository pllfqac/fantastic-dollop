using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Userの持つ１種類のアイテムの実態.
/// ItemAbsoluteNumber毎に作成される.
/// </summary>
[Serializable]
public class OneAbsItemClass
{

    /// <summary>
    /// Item絶対番号.旧itemAbsoluteNumber.
    /// </summary>
    public int iAbs;
    /// <summary>
    /// 所持アイテム数.旧itemCount.
    /// </summary>
    public int iCt;
    /// <summary>
    /// 個々のItemのデータ.HashなしItemの場合でも!=nullなので注意.
    /// </summary>
    public List<OneItemClass> oil;
    /// <summary>
    /// このItemの定義.
    /// </summary>
    [NonSerialized]
    public ItemDefinition definition;
}

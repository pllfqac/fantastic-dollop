using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// CPU商店で売っているアイテムの定義用Class.
/// </summary>
[Serializable]
public class MerchandiseClass
{


    /// <summary>
    /// Item番号.
    /// </summary>
    [SerializeField]
    private byte _itemAbsNum;
    public byte itemAbsNum
    {
        get { return this._itemAbsNum; }
        set { this._itemAbsNum = value; }
    }



    /// <summary>
    /// このアイテムの定義.
    /// ItemNode生成時にAbsNumで検索され参照が入る.
    /// </summary>
    private ItemDefinition _itemDef;
    public ItemDefinition itemDef
    {
        get { return this._itemDef; }
        set { this._itemDef = value; }
    }


}

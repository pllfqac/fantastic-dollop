using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// HP,MP回復量定義.
/// ItemDefinition.ItemType.ReviItemに属する.
/// </summary>
[Serializable]
public class ReviItemDefinition  {


	public ReviItemDefinition(int abs,itemTypeSecondClassification itemType)
	{
		this.absNum = abs;
		this.recoveryType = itemType;
	}

    /// <summary>
    /// 対応するItem絶対番号.
    /// </summary>
    [SerializeField]
    private int _absNum;
    public int absNum
    {
        get { return this._absNum; }
        set { this._absNum = value; }
    }

    /// <summary>
    /// 回復Item小分類.
    /// </summary>
    public enum itemTypeSecondClassification
    {
        /// <summary>
        /// HP回復Item.
        /// </summary>
        HPRecovery,
        /// <summary>
        /// MP回復Item.
        /// </summary>
        MPRecovery,
        /// <summary>
        /// SP回復Item.
        /// </summary>
        SPRecovery,

        /// <summary>
        /// 状態異常回復.
        /// </summary>
        ButCondiRecovery,
    }



    [SerializeField]
    private itemTypeSecondClassification _recoveryType;
    public itemTypeSecondClassification recoveryType
    {
        get { return this._recoveryType; }
        set { this._recoveryType = value; }
    }



    /// <summary>
    /// 定義した回復量.
    /// </summary>
    [SerializeField]
    private int _recoveryValue;
    public int recoveryValue
    {
        get { return this._recoveryValue; }
        set { this._recoveryValue = value; }
    }



}

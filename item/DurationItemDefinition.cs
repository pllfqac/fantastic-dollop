using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 効果持続系アイテムの定義.
/// </summary>
[Serializable]
public class DurationItemDefinition  {

	public DurationItemDefinition(int abs,itemTypeSecondClassification itemType)
	{
		this.AbsNum = abs;
		this.itemType = itemType;
	}

    [SerializeField]
    private int _absNum;
    public int AbsNum
    {
        get { return this._absNum; }
        set { this._absNum = value; }
    }



    public enum itemTypeSecondClassification
    {
        PwrUp,
        DexUp,
        DefUp,
        MatUp,
        MdeUp,
        AgiUp,

        /// <summary>
        /// 経験値Up.
        /// 効果持続時間をServerに記録?.
        /// </summary>
        ExpUp,
    }



    [SerializeField]
    private itemTypeSecondClassification _itemType;
    public itemTypeSecondClassification itemType
    {
        get { return this._itemType; }
        set { this._itemType = value; }
    }



    /// <summary>
    /// 同Typeの効果の弱,強などLevel.
    /// SkillのSkillLevelに相当.
    /// </summary>
    [SerializeField][Range(1,5)]                        //←Skill定義のSkillLevelに合わせる.仮の値now.
    private byte _itemBenefitLevel;
    public byte ItemBenefitLevel
    {
        get { return this._itemBenefitLevel; }
        set { this._itemBenefitLevel = value; }
    }



    /// <summary>
    /// 効果持続時間.
    /// </summary>
    [SerializeField]
    private int _durationTime;
    public int durationTime
    {
        get { return this._durationTime; }
        set { this._durationTime = value; }
    }


}

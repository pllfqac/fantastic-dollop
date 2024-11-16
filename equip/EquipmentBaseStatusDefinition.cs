using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// 装備のステ―タスの基準定義.
/// </summary>
[Serializable]
public class EquipmentBaseStatusDefinition  {


	public EquipmentBaseStatusDefinition(byte abs)
	{
		this.AbsoluteNumber = abs;
	}

    /// <summary>
    /// ItemDefinitionに合わせること.
    /// </summary>
    [SerializeField,Tooltip("ItemDefinitionに合わせること.")]
    private byte _absoluteNumber;
    public byte AbsoluteNumber
    {
        get { return this._absoluteNumber; }
        set { this._absoluteNumber = value; }
    }

    /// <summary>
    /// Scriptable「EquipmentStatusDefinitionbyLevelTwoOver」では2以上の値を指定すること.
    /// </summary>    
    [SerializeField]
    private byte _equipmentLevel;
    public byte equipmentLevel
    {
        get { return this._equipmentLevel; }
        set { _equipmentLevel = value; }
    }

    [SerializeField]
    private byte _eqBasePwr;
    public byte eqBasePwr
    {
        get { return this._eqBasePwr; }
        set { this._eqBasePwr = value; }
    }

    [SerializeField]
    private byte _eqBaseDex;
    public byte eqBaseDex
    {
        get { return this._eqBaseDex; }
        set { this._eqBaseDex = value; }
    }


    [SerializeField]
    private byte _eqBaseDef;
    public byte eqBaseDef
    {
        get { return this._eqBaseDef; }
        set { this._eqBaseDef = value; }
    }



    [SerializeField]
    private byte _eqBaseMat;
    public byte eqBaseMat
    {
        get { return this._eqBaseMat; }
        set { this._eqBaseMat = value; }
    }



    [SerializeField]
    private byte _eqBaseMde;
    public byte eqBaseMde
    {
        get { return this._eqBaseMde; }
        set { this._eqBaseMde = value; }
    }



    [SerializeField]
    private byte _eqBaseAgi;
    public byte eqBaseAgi
    {
        get { return this._eqBaseAgi; }
        set { this._eqBaseAgi = value; }
    }

    /// <summary>
    /// 現在の強化回数.
    /// equipmentMaxIncrement以下の値にすること.
    /// </summary>
    [SerializeField]
    private byte _eqIncrementValue;
    public byte eqIncrementValue
    {
        get { return this._eqIncrementValue; }
        set { this._eqIncrementValue = value; }
    }

    /// <summary>
    /// 装備の最大強化回数.
    /// </summary>
    [SerializeField]
    private byte _equipmentMaxIncrement;
    public byte equipmentMaxIncrement
    {
        get { return this._equipmentMaxIncrement; }
        set { this._equipmentMaxIncrement = value; }
    }

    /// <summary>
    /// 装備できるLevel.
    /// </summary>
    [SerializeField]
    private byte _canEquipLevel;
    public byte CanEquipLevel
    {
        get { return _canEquipLevel; }
        set { this._canEquipLevel = value; }
    }
}

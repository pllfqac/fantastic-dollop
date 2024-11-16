using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 装備の定義.
/// ItemDefinitionに包括される.
/// </summary>
[Serializable]
public class EquipmentDefinition
{

	public EquipmentDefinition(byte abs, CharacterEquipmentPlace.EquipPlaceType equipPlaceType, StaticMyClass.EquipmentType equipmentType, StaticMyClass.WeaponEquipmentType weapon, int weight)
	{
		this.absoluteNumber = abs;
		this.ePlaceType = equipPlaceType;
		this.equipmentType = equipmentType;
		this.WeaponEquipmentType = weapon;
		this.EquipmentWeight = weight;
	}


	[SerializeField]
	private byte _absoluteNumber;
	public byte absoluteNumber
	{
		get { return this._absoluteNumber; }
		set { this._absoluteNumber = value; }
	}


	[SerializeField]
	private CharacterEquipmentPlace.EquipPlaceType _ePlaceType;
	public CharacterEquipmentPlace.EquipPlaceType ePlaceType
	{
		get { return this._ePlaceType; }
		set { this._ePlaceType = value; }
	}

	[SerializeField]
	private StaticMyClass.EquipmentType _equipmentType;
	public StaticMyClass.EquipmentType equipmentType
	{
		get { return this._equipmentType; }
		set { this._equipmentType = value; }
	}


	/// <summary>
	/// 武器でなければnone.
	/// </summary>
	[SerializeField]
	private StaticMyClass.WeaponEquipmentType weaponEquipmentType;
	public StaticMyClass.WeaponEquipmentType WeaponEquipmentType
	{
		get { return weaponEquipmentType; }
		set { this.weaponEquipmentType = value; }
	}

	/// <summary>
	/// 装備につく属性.
	/// (現状武器のみ.武器でなければNothing.
	/// </summary>
	[SerializeField]
	private MySpace.Attribute.Element attribute;
	public MySpace.Attribute.Element Attribute
	{
		get { return attribute; }
		set { this.attribute = value; }
	}

	[SerializeField]
	private int equipmentWeight;
	public int EquipmentWeight
	{
		get { return equipmentWeight; }
		set { this.equipmentWeight = value; }
	}



}

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


/// <summary>
/// NullObject
/// </summary>
public class NullUserEquip : ICheckEquiped
{
	public ReadOnlyDictionary<CharacterEquipmentPlace.EquipPlaceType, OneItemClass> CheckEquiped()
	{
		throw new System.NotImplementedException();
	}

	public Dictionary<CharacterEquipmentPlace.EquipPlaceType, byte> GetEquippingItemAbsTable()
	{
		throw new System.NotImplementedException();
	}

	public StaticMyClass.WeaponEquipmentType GetEquippingWeaponType()
	{
		return StaticMyClass.WeaponEquipmentType.none;
	}

	public bool IsCheckEquiped(string GUID)
	{
		throw new System.NotImplementedException();
	}

	ReadOnlyDictionary<CharacterEquipmentPlace.EquipPlaceType, byte> ICheckEquiped.GetEquippingItemAbsTable()
	{
		throw new System.NotImplementedException();
	}
}

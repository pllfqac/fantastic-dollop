using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

public interface ICheckEquiped  {

    /// <summary>
    /// 指定したGUIDのアイテムが装備されているかどうかの確認.
    /// </summary>
    /// <param name="GUID">調べたいGUID.</param>
    /// <returns>装備されている場合True.</returns>
    bool IsCheckEquiped(string GUID);

	//今どの武器を装備しているか
	StaticMyClass.WeaponEquipmentType GetEquippingWeaponType();

	/// <summary>
	/// Playerの現在の装備状態を返す.
	/// TValue:itemAbs.装備していなければ0が入るので注意.
	/// </summary>
	/// <returns></returns>
	ReadOnlyDictionary<CharacterEquipmentPlace.EquipPlaceType, byte> GetEquippingItemAbsTable();

	/// <summary>
	/// Playerの現在の装備状態を返す.
	/// </summary>
	/// <returns></returns>
	//ReadOnlyDictionary<CharacterEquipmentPlace.equipPlaceType, OneItemClass> CheckEquiped();
}

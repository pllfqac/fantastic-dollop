using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipmentObjectManager
{
	//装備オブジェを外す.
	void UnEquipObject(EquipmentDefinition eDef);
	void UnEquipObject(CharacterEquipmentPlace.EquipPlaceType placeType);

	//装備オブジェを付ける.
	void EquipObject(byte itemAbs);


	//Scene切り替え時に呼ばれる.
	void ChangeEquipObjectStateInTown();
	void ChangeEquipObjectStateInField();


	/// <summary>
	/// FadeでEquipの表示・非表示をするために取得.
	/// </summary>
	/// <returns>Rendererがあるもののみ返す.１つも無い場合はnullを返す.</returns>
	List<Renderer> GetEquippingObjRenderer();

	//ArrowオブジェのTrail Particle制御.Arrow参照渡してSkillParticleManagerでいじる.
	//GameObject GetArrowRef();
}
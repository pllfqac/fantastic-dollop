using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ItemDefinitionTableからItemの情報を取得する.
/// </summary>
public interface ILoadItemDefinitionByAbs
{
	ItemDefinition GetItemDefinition(int ItemAbsoluteNum);
	EquipmentDefinition GetEquipmentItemDefinition(int ItemAbsoluteNum);

	/// <summary>
	/// 対象のAbsのBookの定義を取得する.
	/// </summary>
	/// <param name="abs"></param>
	/// <returns>取得できない場合はnull.</returns>
	BookProperty GetBook(int abs);


	/// <summary>
	///  Item定義の中から ItemType.Seedと定義したitemのAbsと成長時間のClassListを取得する.
	/// </summary>
	/// <returns></returns>
	List<PlantGrowthTimeDefinition> GetGrowthList();
	/// <summary>
	/// 作物育成定義Listから対応するAbsの定義を取得する.
	/// </summary>
	/// <param name="abs"></param>
	/// <returns>なければNull.</returns>
	PlantGrowthTimeDefinition GetGrowthDefinition(int abs);
}

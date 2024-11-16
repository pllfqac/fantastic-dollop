using UnityEngine;
using System.Collections.ObjectModel;

/// <summary>
/// Single2.
/// S3からLoadしたEquipmentObjectの保持クラス
/// </summary>
public class EquipmentObjectDataTable : MonoBehaviour,IEquipmentObjectDataTable
{
	/// <summary>
	/// S3からLoadしてきたデータの保持Table.
	/// Tkey:ItemAbs.
	/// </summary>
	public ReadOnlyDictionary<byte, GameObject> equipmentObjectTable=null;

	/// <summary>
	/// TableからのEquipmentオブジェクトの取得.
	/// </summary>
	/// <param name="itemAbs"></param>
	/// <returns>オブジェクトの取得が出来なければnullを返す.</returns>
	public GameObject GetGameObject(byte itemAbs)
	{
        if (equipmentObjectTable!=null && equipmentObjectTable.ContainsKey(itemAbs)) return equipmentObjectTable[itemAbs];
        else return null;       
	}
}

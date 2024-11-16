using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;


public interface IAddressableEquipmentLoad
{
	/// <summary>
	/// 「Equipment」LabelをつけたオブジェクトをLoadして専用Tableに保持する.
	/// </summary>
	/// <returns></returns>
	Task AllEquipmentLoadAsync();
}

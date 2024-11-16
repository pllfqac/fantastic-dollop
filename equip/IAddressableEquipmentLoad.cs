using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;


public interface IAddressableEquipmentLoad
{
	/// <summary>
	/// �uEquipment�vLabel�������I�u�W�F�N�g��Load���Đ�pTable�ɕێ�����.
	/// </summary>
	/// <returns></returns>
	Task AllEquipmentLoadAsync();
}

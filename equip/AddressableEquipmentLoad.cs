using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.ObjectModel;

/// <summary>
/// 表示用EquipmentオブジェクトのLoad.
/// Title画面.Photon接続直前にロードして,EquipmentObjectDataTableにデータを移すまでこのスクに保存しておく.
/// </summary>
public class AddressableEquipmentLoad : MonoBehaviour, IAddressableEquipmentLoad
{
	//temp.TKey:ItemAbs.
	public Dictionary<byte, GameObject> keyValuePairs;

	/// <summary>
	/// Loadしたものを使いやすい形にして保持する.
	/// </summary>
	/// <returns></returns>
	public async Task AllEquipmentLoadAsync()
	{
		keyValuePairs = new Dictionary<byte, GameObject>();

		var handle = Addressables.LoadAssetsAsync<GameObject>("Equipment", null);       // ラベルを指定して一括ロード
		await handle.Task;      //Async awaitは、AsyncOperationHandle.Taskプロパティを通じてもサポートされます

		if (handle.Status == AsyncOperationStatus.Succeeded)
		{

			foreach (var p in handle.Result)
			{
				byte itemAbs = byte.Parse(p.name.Substring((p.name.IndexOf("@") + 1)));     //@の次の文字からなので+1
				keyValuePairs[itemAbs] = p;
			}

			Addressables.Release(handle);
			StoreEquipmentObjectData();     //Single2に取得したデータ入れるまで繰り返す.
		}
		else
		{
			throw new MyUniException(StaticError.ErrorType.EquipmentLoadError);
		}
	}


	/// <summary>
	/// Single2にある保持用クラスを探してそこに入れる.
	/// MainScene切替後に呼ばれる
	/// </summary>
	public void StoreEquipmentObjectData()
	{
		GetComponent<EquipmentObjectDataTable>().equipmentObjectTable = new ReadOnlyDictionary<byte, GameObject>(keyValuePairs);
	}

}

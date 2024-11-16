using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;


/// <summary>
/// single2.
/// Enemyオブジェクトのロード.
/// </summary>
public class AddressableEnemyLoad : MonoBehaviour, IAddressableEnemyLoad
{

	/// <summary>
	/// Scriptableで定義したEnemyのコレクション.
	/// </summary>
	[SerializeField]
	public EnemyAllList EnemyAllList = null;



	/// <summary>
	/// AddressablesでbuildしS3に置いたEnemyをLoadして,Scriptable#EnemyAllListにGameObject型で参照をセットする.
	/// </summary>
	/// <returns></returns>
	public async Task EnemyLoadAndSetScriptable()
	{
		if (EnemyAllList == null || EnemyAllList.EnemyDefinitions == null)
		{
			return;
		}

		Dictionary<int, GameObject> temp = new Dictionary<int, GameObject>();//Gameobject一時保存先.TKey:EnemyID.

		//ScriptableでAssetReferenceを定義しているのでそれを使う.
		foreach (EnemyDefinition oneEnemyDef in EnemyAllList.EnemyDefinitions)
		{
			var e = await OneEnemyLoadAsync(oneEnemyDef);
			temp[e.enemyId] = e.enemyObj;
		}
		Debug.Log("EnemyObj AllDL完了!!");
		GetComponent<EnemyObjectCollection>().EnemyObjCollection = new System.Collections.ObjectModel.ReadOnlyDictionary<int, GameObject>(temp);
	}


	//戻り値のintはEnemyID.
	private async Task<(int enemyId, GameObject enemyObj)> OneEnemyLoadAsync(EnemyDefinition oneEnemy)
	{
		AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(oneEnemy.EnemyAASRef);
		await handle.Task;      //Async awaitは、AsyncOperationHandle.Taskプロパティを通じてもサポートされます

		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			GameObject enemyObj = handle.Result;
			Addressables.Release(handle);
			return (oneEnemy.EnemyID, enemyObj);
		}
		else
		{
			throw new MyUniException(StaticError.ErrorType.EnemyLoadError);
		}
	}

}

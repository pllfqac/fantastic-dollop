using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Security.Policy;
using System.Linq;

/// <summary>
/// Single1.
/// AASでのTexureLoadと,LoadしたTexture(Utage除く)の保持
/// </summary>
public class AddressableTextureLoad : MonoBehaviour, IAddressableTextureLoad
{
	//キャラ立ち絵Sprite.
	[SerializeField]
	private AssetReference spriteRefCharaA;
	[SerializeField]
	private AssetReference spriteRefCharaB;
	[SerializeField]
	private AssetReference spriteRefCharaC;
	[SerializeField]
	private AssetReference spriteRefCharaD;

	/// <summary>
	/// Load後,各キャラ立ち絵が入る.
	/// TKey:"ABCD"のどれか.
	/// </summary>
	public IReadOnlyDictionary<string, Sprite> LoadResult { get; private set; }



	/// <summary>
	/// 「Texture」Labelを付けたAASのload.
	/// </summary>
	/// <returns></returns>
	public async Task TextureLoadAsync()
	{
		var result = new Dictionary<string, Sprite>();

		var handle = Addressables.LoadAssetsAsync<Sprite>("Texture", null);       // ラベルを指定して一括ロード
		await handle.Task;      //Async awaitは、AsyncOperationHandle.Taskプロパティを通じてもサポートされます

		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			Debug.Log("Texture Load Success!");

			foreach (var p in handle.Result)
			{
				string charaType = p.name.Substring((p.name.IndexOf("@") + 1));     //@の次の文字検索
				Debug.Log("AASダウンロードした  Texture → " + charaType);
				result[charaType] = p;
			}

			Debug.Log("LoadしたTexture Count : " + result.Count);
			Addressables.Release(handle);
			LoadResult = new Dictionary<string, Sprite>(result);

		}
		else
		{
			Debug.LogWarning("Text AAS Load Error!!!!!   Status:" + handle.Status.ToString());
			throw new MyUniException(StaticError.ErrorType.TextureError);
		}
	}

	/// <summary>
	/// AssetReferenceにセットしているSpriteを1つ指定して取得するVer.
	/// </summary>
	/// <param name="assetReference"></param>
	/// <returns></returns>
	public async Task<Sprite> TextureLoadbyAssetReferenceAsync(AssetReference assetReference)
	{
#if UNITY_STANDALONE_LINUX
       return null;
#else
		var handle = Addressables.LoadAssetAsync<Sprite>(assetReference);       //ロード
		await handle.Task;      //Async awaitは、AsyncOperationHandle.Taskプロパティを通じてもサポートされます

		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			Debug.Log("Texture Load Success!  " + handle.Result.name);
			//Addressables.Release(handle);                                         
			return handle.Result;
		}
		else
		{
			Debug.LogWarning("Text AAS Load Error!!!!!   Status:" + handle.Status.ToString());
			throw new MyUniException(StaticError.ErrorType.TextureError);
		}
#endif
	}

	/// <summary>
	/// キャラ別の立ち絵のAASLoad.
	/// 取得不能で例外発生してもゲームの進行は止めない(仮).
	/// </summary>
	/// <param name="charaType">取得したいcharaType</param>
	/// <returns>取得不能ならnull.</returns>
	public async Task<Sprite> TextureLoadAsync(StaticMyClass.CharaType charaType)
	{
		try
		{
			switch (charaType)
			{
				case StaticMyClass.CharaType.A: return await TextureLoadbyAssetReferenceAsync(spriteRefCharaA);
				case StaticMyClass.CharaType.B: return await TextureLoadbyAssetReferenceAsync(spriteRefCharaB);
				case StaticMyClass.CharaType.C: return await TextureLoadbyAssetReferenceAsync(spriteRefCharaC);
				case StaticMyClass.CharaType.D: return await TextureLoadbyAssetReferenceAsync(spriteRefCharaD);
				default: return null;
			}
		}
		catch (MyUniException)
		{
			return null;
		}


	}

	/// <summary>
	/// Label「CharaFaceIcon」を全て取得する.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="System.NotImplementedException"></exception>
	public async Task<List<Sprite>> AllCharaFaceIconLoadAsync()
	{
		var handle = Addressables.LoadAssetsAsync<Sprite>("CharaFaceIcon", null);         // ラベルを指定して一括ロード
		await handle.Task;      //Async awaitは、AsyncOperationHandle.Taskプロパティを通じてもサポートされます
		if (handle.Status == AsyncOperationStatus.Succeeded)
		{
			Debug.Log("FaceIcon AAS Load Success!");
			// Addressables.Release(handle);
			return (List<Sprite>)handle.Result;
		}
		else
		{
			Debug.LogWarning("FaceText AAS Load Error!!!!!   Status:" + handle.Status.ToString());
			throw new MyUniException(StaticError.ErrorType.TextureError);       //別に取れなくても落とすほどではないが...
		}
	}
}

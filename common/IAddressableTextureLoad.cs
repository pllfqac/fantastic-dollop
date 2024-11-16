using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

public interface IAddressableTextureLoad {

    Task TextureLoadAsync();

    IReadOnlyDictionary<string ,Sprite> LoadResult { get; }

	/// <summary>
	/// AssetReferenceにセットしているSpriteを1つ指定して取得するVer.
	/// </summary>
	/// <param name="assetReference"></param>
	/// <returns></returns>
	Task<Sprite> TextureLoadbyAssetReferenceAsync(AssetReference assetReference);


	/// <summary>
	/// キャラ別の立ち絵のAASLoad.
	/// "絵"なのでとりま取得不能で例外発生してもゲームの進行は止めない(仮).
	/// </summary>
	/// <param name="charaType">取得したいcharaType</param>
	/// <returns>取得不能ならnull.</returns>
	Task<Sprite> TextureLoadAsync(StaticMyClass.CharaType charaType);

    /// <summary>
    /// Sprite「Label:CharaFaceIcon」を全て AASでLoadする.
    /// </summary>
    /// <returns></returns>
    Task<List<Sprite>> AllCharaFaceIconLoadAsync();
}

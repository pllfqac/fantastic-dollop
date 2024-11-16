using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// MasterOnly.
/// Master専用のUser所持Item,Seedデータベースへのアクセス.
/// </summary>
public interface IMasterAccessUserOwndataDB  {

    /// <summary>
    /// UserLogin時のDBからの所持Item,Seedの取得.
    /// </summary>
    /// <param name="sendJson">暗号化したShaMDN.IV.</param>
    /// <param name="senderId">対象とするUserのPhotonNetwork.Player.ID</param>	
    Task<string> AccessLoginUserOwnData(string sendJson, int senderId);


    //DBからのSeedのRead.
    Task ReadUserOwnSeed(int userId);



    /// <summary>
    /// userownseedtableへAccessして,所持SeedのUpdateをする.つまりUnity側でMasterが持つ対象UserのSeed値(Total.伝われ)をDBにコピーする.
    /// </summary>
    /// <param name="userId">対象とするUserのPhotonNetwork.Player.ID</param>
    /// <param name="ownSeed">対象とするUserの所持Seed.</param>
    Task UpdateSeed(int userId, int ownSeed);



    /// <summary>
    /// MockからItemの追加,削除をしたとき.
    /// </summary>
    /// <param name="userId">対象UserのPhotonNetwork.Player.ID</param>
    /// <param name="DBJson">DBへ渡すItem更新用Json</param>
    /// <param name="uniJson">Userへ渡すItem更新用Json</param>
    /// <param name="updateMap">Master上のPlayerのItem更新用</param>
    Task UpdateUserOwnItem(int userId, string DBJson,string uniJson,Dictionary<int,OneAbsItemClass> updateMap);

    /// <summary>
    /// ItemのDBへのSave.
    /// </summary>
    /// <param name="userId">対応するUserのID.</param>
    /// <param name="DBJson">DBへ渡すItem更新用Json</param>
    /// <returns></returns>
    Task UpdateUserOwnItem(int userId,string DBJson);
    /// <summary>
    /// UserのItem購入時DB処理.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ownSeed"></param>
    /// <param name="addItem"></param>
    Task UpdateDBSeedAndItembyShopBuy(int userId, int ownSeed, Dictionary<int, OneAbsItemClass> addItem);


    /// <summary>
    /// UserのItem売却時DB処理.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="ownTotalSeed">(この売却処理での)売却アイテム総額を加算した全所持Seed値</param>
    /// <param name="addSeed">(この売却処理での)売却アイテム総額</param>
    /// <param name="deleteItem">売却アイテムのリスト</param>
    Task UpdateDBSeedAndItembyShopSell(int userId, int ownTotalSeed, int addSeed, Dictionary<int, OneAbsItemClass> deleteItem);



    /// <summary>
    /// 装備変更時の装備状態のDB save.
    /// </summary>
    /// <param name="userId">PhotonNetwork.Player.ID</param>
    /// <param name="equipmentStateJson">AbsAndGUIDClassのJson</param>
    Task UpdateEquipmentPlaceState(int userId, string equipmentStateJson);




	/// <summary>
	/// User Logout時のDB Saveをまとめたメソッド.
	/// </summary>
	/// <param name="userId">PhotonView.Owner.ActorNumber</param>
	/// <param name="equipmentStateJson">AbsAndGUIDClassのJsonのDB save</param>
	/// <param name="weaponTypeExpJson"></param>
	/// <returns></returns>
	Task SaveLogoutUserData(int userId, string equipmentStateJson, string weaponTypeExpJson);

    /// <summary>
    /// 合成でのEquipLevelUp or Enhancementした時のDBSave.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="DBJson"></param>
    /// <returns></returns>
     Task UpdateEquipbyComposition(int userId, string DBJson, Dictionary<int, OneAbsItemClass> updateEquip,StaticMyClass.CompositionResultStatus status,int cost);
}

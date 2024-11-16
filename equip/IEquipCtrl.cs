using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Characterへ,装備可能アイテムを「装備」する or 装備を「外す」処理.
/// </summary>
public interface IEquipCtrl  {

    /// <summary>
    /// 「装備」する or 装備を「外す」.
    /// </summary>
    /// <param name="ItemNodeProp">「装備」する or 装備を「外す」アイテムを指定.</param>
    void EquipOrUnequip(ItemNodeProp node);

    /// <summary>
    /// Master用.
    /// UserからのEquipment変更時同期Raise受信後の処理.
    /// </summary>
    /// <param name="json"></param>
    /// <param name="uoi">(Equipする場合)Userがそのアイテムを所持しているかの確認用.</param>
 //   void SyncEquipforMaster(int senderId,string json,IUserOwnItemTable uoi);


    /// <summary>
    /// Login時,DBのequipmentplacestatetableから取得した装備save情報を元に装備する.
    /// 新規User or 何も装備していない状態なら何もしない.
    /// </summary>
    /// <param name="equipmentPlaceStateArrayJson">equipPlaceType順にGUID or null が入った配列のJson.</param>
    /// <param name="uoi">Master:OneItemClass取得用.Userはnull</param>
    ///<param name="selecter">User用.</param>
    IEnumerator LoginEquip(string equipmentPlaceStateArrayJson, IUserOwnItemTable uoi,ISelectable selecter);
}

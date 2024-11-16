using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IUserOwnItemTable
{
  event UpdateItemCountDelegate UpdateItemCountEvent;
  IGetEquipStatusDefinitionTwoOver twoOver { get; set; }

  /// <summary>
  /// アイテムの出入りで呼ばれる.
  /// </summary>
  bool UpdateItem(Dictionary<int, OneAbsItemClass> Itemtable);

  /// <summary>
  /// アイテムの出入りで呼ばれる.
  /// Dictionaryだと面倒な場合.
  /// </summary>
  /// <param name="addItem"></param>
  public void UpdateItem(OneAbsItemClass oai);

  Dictionary<int, OneAbsItemClass> GetUserOwnItemTable();
  void SetOwnItemTable(string json);   //Login時の所持ItemSet.

  void SetEvent(UpdateItemDelegate newAdd, UpdateItemDelegate deleteItem, UpdateItemDelegate updateItem);        //Eventのセット.

  /// <summary>
  /// 指定したアイテムが指定した個数以上所持しているかの確認用.
  /// GUID付は同Absなら複数対応(GUIDの確認もする).
  /// </summary>
  /// <param name="checkItemTable">Raise受信後デシリアライズした売却アイテムTable</param>
  /// <returns>所持しているならTrue</returns>
  bool ContainItem(Dictionary<int, OneAbsItemClass> checkItemTable);



  /// <summary>
  /// 指定したGUIDのアイテムを所持している場合そのOneItemClassを取得する.
  /// </summary>
  /// <param name="guid">GUIDを指定.</param>
  /// <returns></returns>
  OneItemClass GetOneItemClass(string guid);


  /// <summary>
  /// 所持アイテムの中から指定したAbsのOneAbsItemClassを取得する.
  /// </summary>
  /// <param name="abs">取得したいItemのAbs.</param>
  /// <returns>取得できなければNull.</returns>
  OneAbsItemClass GetOneAbs(int abs);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemIn
{
    /// <summary>
    /// 最大所持可能数を取得.
    /// </summary>
    /// <returns></returns>
    int CanOwnItemMaxCount();

    /// <summary>
    /// 指定したアイテムが"取得できるか"の確認.
    /// </summary>
    /// <returns>取得できるならTrue.</returns>
    bool ExistFreeSpace(int Abs,int addCount=1);

    /// <summary>
    /// ガチャ前にイベントリに10以上の空きがあるときTrue
    /// </summary>
    /// <returns>ガチャ前にイベントリに10以上の空きがあるときTrue</returns>
    bool CanBilling();


    /// <summary>
    /// Item取得権発生時の1Absの所持数OverFlowCheck.
    /// OverしたらTrue.
    /// 取得予定値と現在の所持数の合計が最大所持可能数を超えなければfalseを返す.
    /// </summary>
    /// <param name="itemAbsoluteNum"></param>
    /// <param name="addCount"></param>
    /// <returns></returns>
    bool IsItemOverFlow(int itemAbsoluteNum, int addCount);

    /// <summary>
    /// MasterからのItemShop購入成功Raiseの受信後.
    /// </summary>
    /// <param name="hashJson"></param>
    void SuccessedRise(object hashJson);

    //==============MasterOnly===========
    /// <summary>
    /// MasterOnly.
    /// ガチャ,レアドロでのアイテムの取得確定.
    /// DB保存,Raiseも.
    /// </summary>
    /// <param name="getItem"></param>
    void GetItemforMaster(OneAbsItemClass getItem);
    /// <summary>
    /// MasterOnly.
    /// </summary>
    /// <param name="getItems"></param>
    void GetItemforMaster(Dictionary<int, OneAbsItemClass> getItems);

    //========UserOnly===========
    /// <summary>
    /// User.
    /// Itemの取得.
    /// </summary>
    /// <param name="json"></param>
    void GetItemForUser(string json);
    /// <summary>
    /// Itemの取得.UserOnly.Hash付き不可.
    /// </summary>
    /// <param name="abs"></param>
    void GetItemForUser(int abs, int addCount);

    //===========================
    /// <summary>
    /// シナリオ用.Masterへのアイテム生成要請.
    /// </summary>
    /// <param name="createItemAbs">生成してほしいItemAbs.</param>
  //  void RequestCreateScenarioItem(int createItemAbs);

    /// <summary>
    /// FieldItemにTapした時.
    /// </summary>
    /// <param name="locationNum">FieldItem設置個所番号.</param>
    /// <param name="abs"></param>
    /// <param name="requiredItem">FI取得に必要なItem.</param>
    void FieldItemTapped(bool isWait, int locationNum, int itemAbs,int requiredItem);

}
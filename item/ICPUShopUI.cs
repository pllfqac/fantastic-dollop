using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void EndBuyAndSell();

public interface ICPUShopUI  {

    event EndBuyAndSell endEvent;

    //CPU商店の売っているアイテムのUI表示.
    void DisplayCpuShopUI(CPUItemShopMerchandise cpuShopMerchanDef, Dictionary<int, ItemDefinition> itemDef,int userOwnSeed);
    //CPU商店の表示終了.
    void EndDisplayCpuShopUI();

    //所持金Overの時のUI表示.
    void ShowSeedOverText();

    void ShowCountOverText();

    //購入後にCPUShopPanelは残したままでSelectPanel内のNodeは消去する&Seed表示更新.
    void EndBuy(int restSeed);

    void EndSell();

    //売却時のUser所持アイテムの表示変更.第2引数はUser所持数から総選択数を引いた残りの数.
    void ChangeUserOwnItemNodeCount(GameObject userOwnItemNode,byte remaining);

    /// <summary>
    /// SeedのUI更新.
    /// </summary>
    /// <param name="nowSeed">現在の所持Seed</param>
    void UpdateSeedText(int nowSeed);
}

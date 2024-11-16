using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Single2.
/// 定義したShopのScriptableObjectをまとめたリストとそのリストの使用.
/// </summary>
public class CPUShopList : MonoBehaviour {

    //ScriptableObjectで作ったCPU商店の定義の各参照.CPU商店を新規作成したらこのListを増やしてそのScriptableObjectをセットする.
    [SerializeField]
    private List<CPUItemShopMerchandise> shopList=null;


    /// <summary>
    /// shopオブジェにつけたTagから対応するShopの定義(商品&Valueリスト)を返す.
    /// </summary>
    /// <param name="shopNameOrTag">shopオブジェにつけたTag</param>
    /// <returns></returns>
    public CPUItemShopMerchandise SelectShop(string shopNameOrTag)
    {
        if (shopList.Any(x => x.shopName == shopNameOrTag)) return shopList.First(x => x.shopName == shopNameOrTag);
        else return new CPUItemShopMerchandise();       //もしなければ.
    }
}

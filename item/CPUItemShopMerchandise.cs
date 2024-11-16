using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CPU商店アイテム定義リスト用のScriptableObject作成用.
/// Shop毎に1つのScriptableObject作成.
/// </summary>
[CreateAssetMenu(menuName = "MyGame/Create CPUShopMerchandiseList", fileName = "MerchandiseList_CPUShop")]
public class CPUItemShopMerchandise : ScriptableObject
{



    private string _shopName;
    public string shopName
    {
        get { return this._shopName; }
        set { this._shopName = value; }
    }

    [SerializeField]
    public List<MerchandiseClass> shopItemList = new List<MerchandiseClass>();

}

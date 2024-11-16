using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Canvas-Configrations-CPUShopPanel.
/// Canvas-ItemPanelにあるScrollView(Uesr所持アイテムリスト)を,ItemShopの「売却」に流用するためにUIの位置を変更する.
/// </summary>
public class MoveItemScrollView : MonoBehaviour {   	 

	///Canvas-ItemPanel.
	[SerializeField]
    private GameObject itemPanel=null;

	/// <summary>
	/// Canvas-ItemPanel-ScrollView.
	/// Userの所持アイテムを表示するオブジェ.
	/// これの位置を動かす.
	/// </summary>
	[SerializeField]
	private Transform itemPanelScrollView = null;

	//ItemPanelの最初の位置(のanchor)
	private Vector2 tempScrollViewAnchorPos;
	//元に戻すとき用
	private Vector2 originOffsetMax;
	private Vector2 originOffsetMin;

	//ItemShopでの「売却」時のUserOwnItemの表示位置となるPanel.
	[SerializeField]
	private Transform userItemScrollPositionPanel = null;


	[SerializeField]
    private GameObject marchandisePanel=null;    //「買う」Panel
	[SerializeField]
	private GameObject userItemSellPanel = null;  //「売る」Panel 

	[SerializeField]
    private ItemScrollController itemScrollCtrl=null;



	//スクロールバーの切り替えについて
	//ItemPanelの場合				→ ItemPanel-Scrollbarを使う.
	//CPUShopBuy,CPUShopSellの場合	→ 共通のスクロールバーを使う.Canvas-Configrations-CPUShopPanel-CommonScrollbarを使用.

	//ItemPanel-Scrollbar
	[SerializeField]
	private Scrollbar ItemPanelScrollbar = null;
	//共通のスクロールバー
	[SerializeField]
	private Scrollbar CommonScrollbar = null;





	private void Awake()
    {
        userItemSellPanel.SetActive(false);  //「買う」Panelがデフォで表示されるように.Start()でないのはCPUShopUIのStart()で親を非Activeにしてるため
    }

	void Start()
	{
		tempScrollViewAnchorPos = itemPanelScrollView.GetComponent<RectTransform>().anchoredPosition;
		originOffsetMax = itemPanelScrollView.GetComponent<RectTransform>().offsetMax;
		originOffsetMin = itemPanelScrollView.GetComponent<RectTransform>().offsetMin;
	}



    public void SelectedBuy()
    {
        itemPanelScrollView.transform.SetParent(itemPanel.transform);    //元の親子関係に戻す.
		itemPanelScrollView.GetComponent<RectTransform>().anchoredPosition = tempScrollViewAnchorPos;           //ScrollView(の位置)を,元のCanvas-ItemPanelの位置に戻す.
		itemPanelScrollView.GetComponent<ScrollRect>().verticalScrollbar = CommonScrollbar; //スクロールバー参照をShop用に変更する.

		itemPanelScrollView.gameObject.SetActive(false);
        if (userItemSellPanel.activeSelf) userItemSellPanel.SetActive(false);
        marchandisePanel.SetActive(true);
    }

    public void SelectedSell()
    {
        userItemSellPanel.SetActive(true);
        itemPanelScrollView.gameObject.SetActive(true);
        itemPanelScrollView.transform.SetParent(userItemScrollPositionPanel.transform);          //ItemPanel-ScrollViewをuserItemSellPanelの子に設定する.
		itemPanelScrollView.GetComponent<Image>().enabled = false;							//SellのときはItemPanelのimageを消す.

		RectTransform rectTr = itemPanelScrollView.GetComponent<RectTransform>();
		rectTr.anchoredPosition = userItemScrollPositionPanel.GetComponent<RectTransform>().anchoredPosition;//所持アイテムListをItemShop用の位置に変える.
		itemPanelScrollView.GetComponent<ScrollRect>().verticalScrollbar = CommonScrollbar;  //スクロールバー参照をShop用に変更する.

		itemScrollCtrl.ShowUserOwnItemList();
		rectTr.offsetMax = new Vector2(0, 0);     //位置調整.
		rectTr.offsetMin = new Vector2(0, 0);
		
		if (marchandisePanel.activeSelf) marchandisePanel.SetActive(false);
    }


    //Shop利用終了時,userOwnItemListObjをactiveにきり変える.非activeのままだとActiveにならないため.
    public void ActiveUserOwnItemListObj()
    {
		itemPanelScrollView.GetComponent<ScrollRect>().verticalScrollbar = ItemPanelScrollbar; //スクロールバー参照をItemPanel用に戻す.
		RectTransform rt = itemPanelScrollView.GetComponent<RectTransform>();
		rt.anchoredPosition = tempScrollViewAnchorPos;       //位置戻す.
		itemPanelScrollView.GetComponent<Image>().enabled = true;                               //ItemPanelのimageを戻す.
		rt.offsetMax = originOffsetMax;
		rt.offsetMin = originOffsetMin;
		itemPanelScrollView.gameObject.SetActive(true);
    }
}

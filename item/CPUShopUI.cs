using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Canvas.
/// </summary>
public class CPUShopUI : MonoBehaviour, ICPUShopUI
{

    //==================================Debug用==================================
    /*  [Tooltip("デバッグ用.自キャラへの参照")]
      public GameObject debugMyObj;
      private int debugAddSeed = 1000;
  */
    //====================================================================

    //連打防止
    private PushManager push;

    /// <summary>
    /// 「購入」「売却」確定ボタン.
    /// </summary>
    [SerializeField]
    private Button decisionButton = null;

    //CPU商店UIをまとめたPanel.
    [SerializeField]
    private GameObject shopUiPanel = null;

    [SerializeField]
    private CPUShopMerchandiseListScrollCtrl shopListScrollCtrl = null;
    [SerializeField]
    private DropObjectForSelectPanel dosp = null;
    [SerializeField]
    private CPUShopBuyAndSell BaS = null;

    [SerializeField]
    private TextMeshProUGUI overText = null;      //所持金Over or 個数Overのときに出すText.
    [SerializeField]
    private TextMeshProUGUI userOwnSeed = null;
    [SerializeField]
    private ItemScrollController itemScCtrl = null;


    //どちらが選択されているかの判断にはCPUShopBuyAndSellのEnumを利用
    [SerializeField]
    private Button selectBuyButton = null;     //左上の「買う」ボタン.
    [SerializeField]
    private Button selectSellButton = null;    //左上の「売る」ボタン.

    //「買う」「売る」ボタンのアクティブ・非アクティブ状態用UIテクスチャの参照
    [SerializeField]
    private Sprite activeImage = null;
    [SerializeField]
    private Sprite inActiveImage = null;


    [SerializeField]
    private MoveItemScrollView moveUserOwnItemList = null;

    public event EndBuyAndSell endEvent;

    [SerializeField]
    private Text totalSelectedPrice = null;    //SelectPanelの選択されたアイテムの合計値表示用Text.


    [SerializeField]
    private GameObject itemDataPanel = null;

    //所持個数と最大所持可能数の表示用Text
    [SerializeField]
    private TextMeshProUGUI ownItemCountText;
    [SerializeField]
    private TextMeshProUGUI canMaxOwnItemCountText;
    //上記Text用の参照.Instat.
    [NonSerialized]
    public IItemIn itemIn;
    [NonSerialized]
    public IGetUserOwnItemCount ownItemCount;


    private void Start()
    {
        push = GetComponent<PushManager>();
        shopUiPanel.SetActive(false);
        overText.enabled = false;
        dosp.changeTotalValueAction += UpdateSelectedTotalValue;
        //ICCPanel表示での、左上の「買う」「売る」ボタンのenable or disableの制御.
        dosp.ShowICCPanelDisableAction += DisableOtherShopUIButton;
        dosp.ShowICCPanelEnableAction += EnableOtherShopUIButton;
    }




    /// <summary>
    /// CPU商店をTapした時のUI表示.
    /// </summary>
    /// <param name="cpuShopMerchanDef">Shop別の売ってるItemの定義</param>
    /// <param name="itemDef">Itemの定義.TKey:Item absoluteNumber</param>
    public void DisplayCpuShopUI(CPUItemShopMerchandise cpuShopMerchanDef, Dictionary<int, ItemDefinition> itemDef, int userOwnSeed)
    {
        UpdateSelectedTotalValue(0);
        GetComponent<CanvasManager>().DisplayCtrlCpuMerchandise(false);
        shopUiPanel.SetActive(true);
        BaS.BorS = CPUShopBuyAndSell.BorSSelect.buy;
        selectBuyButton.enabled = false;
        selectSellButton.enabled = true;
        selectBuyButton.image.sprite = inActiveImage;           //選択されているほうを白めにする.
        selectSellButton.image.sprite = activeImage;
        selectBuyButton.gameObject.transform.GetChild(0).GetComponent<Text>().color = GetColor(true);
        selectSellButton.gameObject.transform.GetChild(0).GetComponent<Text>().color = GetColor(false);     //選択されていないほうのButtonのTextの色を薄暗く
        this.userOwnSeed.text = userOwnSeed.ToString();     //Userの所持Seedの表示.
                                                            //Panelに定義から各商品Node生成.
        shopListScrollCtrl.CreateMerchandiseList(cpuShopMerchanDef, itemDef);
    }


    /// <summary>
    /// shopUiPanelの取引終了ボタンイベント.
    /// </summary>
    public void EndDisplayCpuShopUI()
    {
        if (selectSellButton.gameObject.activeSelf) OnSelectedBuyButton(); //「売る」が選択されていた場合,「買う」に戻して処理を終了する.
        itemDataPanel.SetActive(false);
        moveUserOwnItemList.ActiveUserOwnItemListObj();
        shopListScrollCtrl.DeleteShopDisplayNodes();        //MerchandiseListPanel側のItemNodeの削除.
        dosp.DeleteCreatedNode();                           //SelectPanel側のNode削除.
        shopUiPanel.SetActive(false);
        GetComponent<CanvasManager>().DisplayCtrlCpuMerchandise(true);
        dosp.HideCCPanel();
        //取引終了をすることで再び利用することが出来る(多重利用の禁止).
        if (endEvent != null) endEvent();
    }

    /// <summary>
    /// 「決定」ボタン.
    /// 「購入」「売却」共用.どちらが選択されているかの判断にはCPUShopBuyAndSellのEnumを利用.
    /// </summary>
    public void OnBuyButton()
    {
        if (!push.CanPush()) return;          //入力禁止でfalseが返ってくる.
        itemDataPanel.SetActive(false);
        if (BaS.BorS == CPUShopBuyAndSell.BorSSelect.buy) BaS.ItemBuy(dosp.createdNodes);
        else if (BaS.BorS == CPUShopBuyAndSell.BorSSelect.sell) BaS.ItemSell(dosp.createdNodes);
        else return;    //調合とか??
        UpdateSelectedTotalValue(0);
        UpdateOwnItemCount();
    }

    /// <summary>
    /// 所持金Overでの表示.
    /// </summary>
    public void ShowSeedOverText()
    {
        StartCoroutine(ShowAndHideOverText(StaticMyClass.ownSeedOverMessage));
    }

    private IEnumerator ShowAndHideOverText(string message)
    {
        overText.text = message;
        overText.enabled = true;
        yield return new WaitForSeconds(StaticMyClass.TextShowTime);
        overText.enabled = false;
    }

    /// <summary>
    /// 購入後CPUShopPanelは残すのでそれ関係.
    /// </summary>
    public void EndBuy(int restSeed)
    {
        userOwnSeed.text = restSeed.ToString();
        dosp.DeleteCreatedNode();   //Nodeは消す.
    }

    public void EndSell()
    {
        dosp.DeleteCreatedNode();   //Nodeは消す.
    }



    /// <summary>
    /// 最大所持可能個数overでの表示.
    /// </summary>
    public void ShowCountOverText()
    {
        StartCoroutine(ShowAndHideOverText(StaticMyClass.itemCountOverMessage));
    }

    //左上の「買う」ボタン Event.
    public void OnSelectedBuyButton()
    {
        if (BaS.BorS == CPUShopBuyAndSell.BorSSelect.sell) UpdateTextSelectedNode(dosp.createdNodes);
        itemDataPanel.SetActive(false);
        dosp.DeleteCreatedNode();
        BaS.BorS = CPUShopBuyAndSell.BorSSelect.buy;
        selectBuyButton.enabled = false;
        selectSellButton.enabled = true;       //「売る」ボタン を有効にする.
        selectBuyButton.image.sprite = inActiveImage;
        selectSellButton.image.sprite = activeImage;
        selectBuyButton.gameObject.transform.GetChild(0).GetComponent<Text>().color = GetColor(true);
        selectSellButton.gameObject.transform.GetChild(0).GetComponent<Text>().color = GetColor(false);     //選択されていないほうのButtonのTextの色を薄暗く

        moveUserOwnItemList.SelectedBuy();
        UpdateSelectedTotalValue(0);
    }

    //左上の「売る」ボタン Event.
    public void OnSelectedSellButton()
    {
        itemDataPanel.SetActive(false);
        dosp.DeleteCreatedNode();
        BaS.BorS = CPUShopBuyAndSell.BorSSelect.sell;
        selectSellButton.enabled = false;
        selectBuyButton.enabled = true;        //「買う」ボタンを有効にする.
        selectBuyButton.image.sprite = activeImage;
        selectSellButton.image.sprite = inActiveImage;
        selectBuyButton.gameObject.transform.GetChild(0).GetComponent<Text>().color = GetColor(false);  //選択されていないほうのButtonのTextの色を薄暗く
        selectSellButton.gameObject.transform.GetChild(0).GetComponent<Text>().color = GetColor(true);

        moveUserOwnItemList.SelectedSell();         //表示制御等はまるなげ
        UpdateSelectedTotalValue(0);
    }

    //「売却」時のみ.売却するPanelに入ったときの処理.DropObjectForSelectPanelスクでのUser所持アイテム数のUIのみの変更&UI表示OnOff.
    public void ChangeUserOwnItemNodeCount(GameObject userOwnItemNode, byte remaining)
    {
        userOwnItemNode.transform.Find("ItemCount").GetComponent<Text>().text = remaining.ToString();
        if (remaining == 0) userOwnItemNode.SetActive(false);

    }

    //「売却」時のみ.[buy]に一度切り替えて再び[Sell]を選択したとき用.左のPanel(User所持アイテム)のCount表示を戻す.OneAbsのitemCountはいじってないのでそれで復元.
    private void UpdateTextSelectedNode(List<ItemNodeProp> selectedItemNodeList)
    {
        itemScCtrl.ResetItemCountUI(selectedItemNodeList);      //SelectPanelにあるNodeで左PanelのNodeをけんさく&UI更新.
    }

    //Seed Uiの更新.
    public void UpdateSeedText(int nowSeed)
    {
        userOwnSeed.text = nowSeed.ToString();
    }

    //Selectされたアイテムの総額UIの更新.D&Dされるたびに呼ばれる.
    private void UpdateSelectedTotalValue(int totalValue)
    {
        totalSelectedPrice.text = totalValue.ToString();
    }


    /// <summary>
    /// ItemCountChangePanel表示中にItemCountChangePanel以外のボタン(選択されていた方(enabled = false)と逆のボタン & 「確定」ボタン)を有効にする.
    /// </summary>
    public void EnableOtherShopUIButton(CPUShopBuyAndSell.BorSSelect selected)
    {
        if (selected == CPUShopBuyAndSell.BorSSelect.buy)
        {
            selectBuyButton.enabled = false;
            selectSellButton.enabled = true;
            selectBuyButton.image.sprite = inActiveImage;
            selectSellButton.image.sprite = activeImage;
        }
        else
        {
            selectBuyButton.enabled = true;
            selectSellButton.enabled = false;
            selectBuyButton.image.sprite = activeImage;
            selectSellButton.image.sprite = inActiveImage;
        }
        decisionButton.enabled = true;
    }

    /// <summary>
    /// ItemCountChangePanel表示中にItemCountChangePanel以外のボタンを無効にする.
    /// </summary>
    public void DisableOtherShopUIButton()
    {
        selectBuyButton.enabled = false;
        selectSellButton.enabled = false;
        decisionButton.enabled = false;
    }


    /// <summary>
    /// 16進数で定義したカラーコードをColor型に変換して取得する.
    /// </summary>
    /// <param name="isSelect">選択された時のColorが欲しい場合はTrue.非選択の場合はfalseを指定する.</param>
    private Color GetColor(bool isSelect)
    {
        string targetStrColorCode = isSelect ? StaticMyClass.WhiteColorCode : StaticMyClass.CPUShopSelectTabUnselectedColor;
        return StaticMyClass.GetSelectedColor(targetStrColorCode);
    }

    /// <summary>
    /// 所持個数と最大所持可能数の表示用Textの更新.
    /// </summary>
    private void UpdateOwnItemCount()
    {
        ownItemCountText.text = ownItemCount.GetOwnItemCount().ToString();
        canMaxOwnItemCountText.text = "/" + itemIn.CanOwnItemMaxCount().ToString();
    }
}

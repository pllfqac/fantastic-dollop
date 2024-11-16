using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using System.Linq;
using TMPro;

/// <summary>
/// MainScene-Canvas-Configrations-CompositionPanel.
/// 合成のUI.
/// </summary>
public class CompositionUI : MonoBehaviour
{
    /// <summary>
    /// Userが選択した合成の原料.
    /// </summary>
    public Action<List<ItemNodeProp>> selectedItems;

    ///Canvas-ItemPanel.
    [SerializeField]
    private GameObject itemPanel = null;
    [SerializeField]
    private TextMeshProUGUI showText = null;       //OverText.
    //ItemPanelの最初の位置(のanchor)
    private Vector2 tempScrollViewAnchorPos;
    //元に戻すとき用
    private Vector2 originOffsetMax;
    private Vector2 originOffsetMin;

    /// <summary>
    /// Canvas-ItemPanel-ScrollView.
    /// Userの所持アイテムを表示するオブジェ.
    /// </summary>
    [SerializeField]
    private Transform itemPanelScrollView = null;
    [SerializeField]
    private CanvasManager canvasManager = null;

    //UserOwnItemの表示位置となるPanel.
    [SerializeField]
    private Transform userItemScrollPositionPanel = null;
    [SerializeField]
    private ItemScrollController itemScrollCtrl = null;
    [SerializeField]
    private Scrollbar CommonScrollbar = null;
    //ItemPanel-Scrollbar
    [SerializeField]
    private Scrollbar ItemPanelScrollbar = null;
    [SerializeField]
    private Button okButton = null;     //合成決定ボタン.
    [SerializeField]
    DropObjectForCompositionMaterialSelectPanel docm = null;
    [SerializeField]
    private PushManager push = null;
    [SerializeField]
    private GameObject itemDataPanel = null;

    [SerializeField]
    private Text userOwnSeedText = null;
    [NonSerialized]
    public IOwnSeed seed;
    [SerializeField]
    private GameObject itemCountChangePanel = null;

    /// <summary>
    /// 表示OnでもSeedの表示更新
    /// </summary>
    private void OnEnable()
    {
        UpdateOwnSeedText();
    }



    private void Start()
    {
        tempScrollViewAnchorPos = itemPanelScrollView.GetComponent<RectTransform>().anchoredPosition;
        originOffsetMax = itemPanelScrollView.GetComponent<RectTransform>().offsetMax;
        originOffsetMin = itemPanelScrollView.GetComponent<RectTransform>().offsetMin;

    }

    /// <summary>
    /// 合成画面表示.
    /// </summary>
    public void ShowCompositionPanel()
    {
        Debug.Log("Show Composition");
        itemPanelScrollView.gameObject.SetActive(true);
        itemPanelScrollView.transform.SetParent(userItemScrollPositionPanel.transform);          //ItemPanel-ScrollViewをCompositionPanelの子に設定する.
        itemScrollCtrl.ShowUserOwnItemList();
        itemPanelScrollView.GetComponent<ScrollRect>().verticalScrollbar = CommonScrollbar; //スクロールバー参照をShop用に変更する.
        RectTransform rectTr = itemPanelScrollView.GetComponent<RectTransform>();
        rectTr.anchoredPosition = userItemScrollPositionPanel.GetComponent<RectTransform>().anchoredPosition;//所持アイテムListをItemShop用の位置に変える.
        rectTr.offsetMax = new Vector2(0, 0);     //位置調整.
        rectTr.offsetMin = new Vector2(0, 0);
    }

    /// <summary>
    /// 合成ボタンが押されたとき.
    /// </summary>
    public void OnCompositionButton()
    {
        if (!push.CanPush()) return;
        Debug.Log("合成ボタンOn");

        itemDataPanel.SetActive(false);
        if (selectedItems != null) selectedItems(docm.createdNodes);
    }

    /// <summary>
    /// Returnボタン
    /// </summary>
    public void OnClose()
    {
        if (showText.IsActive()) return;
        Debug.Log("Close");
        if (itemDataPanel.activeSelf) itemDataPanel.SetActive(false);
        itemScrollCtrl.ResetItemCountUI(docm.createdNodes);
        docm.DeleteCreatedNode();
        itemPanelScrollView.transform.SetParent(itemPanel.transform);    //元の親子関係に戻す.
        itemPanelScrollView.GetComponent<RectTransform>().anchoredPosition = tempScrollViewAnchorPos;           //ScrollView(の位置)を,元のCanvas-ItemPanelの位置に戻す.
        itemPanelScrollView.GetComponent<ScrollRect>().verticalScrollbar = ItemPanelScrollbar; //スクロールバー参照をItemPanel用に戻す.

        RectTransform rt = itemPanelScrollView.GetComponent<RectTransform>();
        rt.anchoredPosition = tempScrollViewAnchorPos;       //位置戻す.
        rt.offsetMax = originOffsetMax;
        rt.offsetMin = originOffsetMin;
        itemPanelScrollView.gameObject.SetActive(true);
        if (itemCountChangePanel.activeSelf) itemCountChangePanel.SetActive(false);
        canvasManager.CloseCompositionPanel();
    }


    /// <summary>
    /// Materialの個数を入力中(ICC Panel)は他のボタンを押せないようにする.
    /// </summary>
    public void SelecttingItemCount()
    {
        okButton.enabled = false;
    }

    public void EndSelectting()
    {
        okButton.enabled = true;
    }




    public void ChangeUserOwnItemNodeCount(GameObject userOwnItemNode, byte remaining)
    {
        userOwnItemNode.transform.Find("ItemCount").GetComponent<Text>().text = remaining.ToString();
        if (remaining == 0) userOwnItemNode.SetActive(false);
    }



    //合成成功or失敗or中断でのSelectされたItemの個数を正しい値に戻す.
    public void UpdateUI(StaticMyClass.CompositionResultStatus state)
    {
        switch (state)
        {
            case StaticMyClass.CompositionResultStatus.EquipLevelUpSuccess:
                //Equip以外は消滅.
                itemScrollCtrl.ResetItemCountUI(docm.createdNodes.Where(x => x.oneAbs.definition.itemType == ItemDefinition.ItemType.Equipment).ToList());  //Equのみ戻す.
                docm.DeleteCreatedNode();
                StartCoroutine(ShowMessageEnumerator(StaticMyClass.compositionSuccess));
                break;
            case StaticMyClass.CompositionResultStatus.EquipEnhancementSuccess:
                //Equip以外は消滅.
                itemScrollCtrl.ResetItemCountUI(docm.createdNodes.Where(x => x.oneAbs.definition.itemType == ItemDefinition.ItemType.Equipment).ToList());  //Equのみ戻す.
                docm.DeleteCreatedNode();
                StartCoroutine(ShowMessageEnumerator(StaticMyClass.compositionSuccess));
                break;
            case StaticMyClass.CompositionResultStatus.OtherCompositionSuccess:
                docm.DeleteCreatedNode();
                StartCoroutine(ShowMessageEnumerator(StaticMyClass.compositionSuccess));
                break;
            case StaticMyClass.CompositionResultStatus.EquipCountOver:
                //すべて元に戻す.
                itemScrollCtrl.ResetItemCountUI(docm.createdNodes);
                docm.DeleteCreatedNode();
                itemScrollCtrl.ShowUserOwnItemList();
                StartCoroutine(ShowMessageEnumerator(StaticMyClass.compositionProprietyNG));
                break;
            case StaticMyClass.CompositionResultStatus.EquipIncrementRemainingCountZero:
                //すべて元に戻す.
                itemScrollCtrl.ResetItemCountUI(docm.createdNodes);
                docm.DeleteCreatedNode();
                itemScrollCtrl.ShowUserOwnItemList();
                StartCoroutine(ShowMessageEnumerator(StaticMyClass.compositionProprietyNG3));
                break;
            case StaticMyClass.CompositionResultStatus.RecipeNull:
                //すべて元に戻す.
                itemScrollCtrl.ResetItemCountUI(docm.createdNodes);
                docm.DeleteCreatedNode();
                itemScrollCtrl.ShowUserOwnItemList();
                StartCoroutine(ShowMessageEnumerator(StaticMyClass.compositionProprietyNG));
                break;
            case StaticMyClass.CompositionResultStatus.MaterialLack:
                //すべて元に戻す.
                itemScrollCtrl.ResetItemCountUI(docm.createdNodes);
                docm.DeleteCreatedNode();
                itemScrollCtrl.ShowUserOwnItemList();
                StartCoroutine(ShowMessageEnumerator(StaticMyClass.compositionProprietyNG2));
                break;
            case StaticMyClass.CompositionResultStatus.SeedLack:
                //すべて元に戻す.
                itemScrollCtrl.ResetItemCountUI(docm.createdNodes);
                docm.DeleteCreatedNode();
                itemScrollCtrl.ShowUserOwnItemList();
                StartCoroutine(ShowMessageEnumerator(StaticMyClass.ownSeedOverMessage));
                break;
            case StaticMyClass.CompositionResultStatus.Failure:
                //Equip以外は消滅.合成回数増加
                itemScrollCtrl.ResetItemCountUI(docm.createdNodes.Where(x => x.oneAbs.definition.itemType == ItemDefinition.ItemType.Equipment).ToList());  //Equのみ戻す.
                docm.DeleteCreatedNode();
                StartCoroutine(ShowMessageEnumerator(StaticMyClass.compositionFailure));
                break;
            case StaticMyClass.CompositionResultStatus.NonLevel:
                //すべて元に戻す.
                itemScrollCtrl.ResetItemCountUI(docm.createdNodes);
                docm.DeleteCreatedNode();
                itemScrollCtrl.ShowUserOwnItemList();
                StartCoroutine(ShowMessageEnumerator(StaticMyClass.compositionProprietyNG));
                break;
            default:
                break;
        }

        UpdateOwnSeedText();
    }

    /// <summary>
    /// Conposition成功orFailureでのSeedの表示更新
    /// </summary>
    private void UpdateOwnSeedText()
    {
        if (seed == null) return;
        userOwnSeedText.text = seed.ReadOwnSeed().ToString();
        Debug.Log("Seed更新" + seed.ReadOwnSeed());
    }


    /// <summary>
    /// Textの表示OnOff.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowMessageEnumerator(string message)
    {
        showText.text = message;
        showText.enabled = true;
        yield return new WaitForSeconds(2.5f);
        showText.enabled = false;
    }
}

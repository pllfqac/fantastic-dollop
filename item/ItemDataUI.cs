using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
/// <summary>
/// Canvas-ItemDataPanel.
/// ItemNodeをTapしたとき.
/// アイテムの説明・装備ステータス等のUI.
/// ItemNodeを生成するスクにこのクラスへの参照を持たせること.
/// </summary>
public class ItemDataUI : MonoBehaviour, IItemDataUI
{

    private readonly string Plus = "+";

    [NonSerialized]
    public IEquipCtrl equipCtrl;        //Instant.
    [NonSerialized]
    public ItemBoxManager ibm;          //isn.
    [NonSerialized]
    public IItemOut itemOut;            //ins.

    /// <summary>
    /// 画面右下の「装備する」or「外す」Button.
    /// EquipをTapしたときのみ表示.
    /// </summary>
    [SerializeField]
    private Button equipButton = null;
    /// <summary>
    /// 画面右下の「使う」Button.
    /// 使用出来るアイテムをTapした時のみ表示.
    /// </summary>
    [SerializeField]
    private Button itemUseButton = null;



    private readonly int rectTrXPos = 573;       //CPUShop利用時の ItemDataPanelの表示位置変更用.しょうさいは別紙参照.



    //EquipmentConfigPanelの上のText&Image.
    [SerializeField]
    private Image ItemImage = null;
    [SerializeField]
    private TextMeshProUGUI ItemName = null;

    //Item説明文
    [SerializeField]
    private TextMeshProUGUI directionText = null;

    //装備箇所表示用.
    [SerializeField]
    private TextMeshProUGUI equipablePlaceText = null;

    //装備Item用のステータスをまとめたPanel.
    [SerializeField]
    private GameObject equipmentStatusPanel = null;

    //装備のステータス値の表示用.
    [SerializeField]
    private TextMeshProUGUI pwrValueText = null;
    [SerializeField]
    private TextMeshProUGUI dexValueText = null;
    [SerializeField]
    private TextMeshProUGUI defValueText = null;
    [SerializeField]
    private TextMeshProUGUI matValueText = null;
    [SerializeField]
    private TextMeshProUGUI mdeValueText = null;
    [SerializeField]
    private TextMeshProUGUI agiValueText = null;
    [SerializeField]
    private TextMeshProUGUI increValueText = null;
    [SerializeField]
    private TextMeshProUGUI increMaxValueText = null;
    [SerializeField]
    private TextMeshProUGUI canEquipLevel = null;   //装備可能Level.
    //Item(Equipのみだけど)Imageの右下のEquipLevel表示用.
    [SerializeField]
    private EquipLevelStarUI levelStarUI;

    //TapされたItemNodeを区別するため.
    private int tempTapedItemNodeInstanceId;

    //TapされたEquipmentの一時沖.
    private ItemNodeProp tempNode = null;



    private void OnDisable()
    {
        tempTapedItemNodeInstanceId = 0;
        tempNode = null;
        equipButton.gameObject.SetActive(false);        //非表示.
        itemUseButton.gameObject.SetActive(false);
    }



    /// <summary>
    /// ItemNodeがTapされたとき.
    /// Equipment用(=GUID付).
    /// </summary>
    /// <param name="nodeProp">Tapしたnode</param>
    /// <param name="instanceId"></param>
    /// <param name="tapPosition"></param>
    public void ShowItemData(ItemNodeProp nodeProp, int instanceId, int tapPosition, string tapNodeTagName)
    {
        if (CheckRetap(instanceId)) return;
        Init(nodeProp.oneAbs.definition, tapPosition, tapNodeTagName);
        tempNode = nodeProp;             //temp保存して「装備する」or「外す」Buttonが押されたときに使用.
        equipmentStatusPanel.SetActive(true);
        // equipmentStatusPanel.SetActive(true);
        ShowEquipmentDefinition(nodeProp.oneItem.eDefinition);
        SetEquipmentStatus(nodeProp.oneItem.eStatus);
    }

    /// <summary>
    /// ItemNodeがTapされたとき.
    /// GUID付かないアイテム.
    /// </summary>
    /// <param name="definition"></param>
    public void ShowItemData(ItemNodeProp nodeProp, ItemDefinition definition, int instanceId, int tapPosition, string tapNodeTagName)
    {
        if (CheckRetap(instanceId)) return;
        Init(definition, tapPosition, tapNodeTagName);
        tempNode = nodeProp;
        equipmentStatusPanel.SetActive(false);
        levelStarUI.ShowEquipLevelStart(0);     //一応消しておく.
    }

    //表示するときの共通処理.
    private void Init(ItemDefinition defi, int tapPosition, string tapNodeTag)
    {
        if (!this.gameObject.activeSelf) this.gameObject.SetActive(true);           //ItemDataPanelが表示されていなければ表示.
        SetShowPosition(tapPosition);                                           //表示の為にTapされたNodeの位置を元に,ItemDataPanelの表示位置を決定する.
        ItemName.text = defi.itemName;
        ItemImage.sprite = defi.sprite;
        //  if (!string.IsNullOrEmpty(defi.directions))
        directionText.text = defi.directions;

        //Equipmentの場合,「装備する」or「外す」Buttonを表示する(ただしShopItemの場合は除く).
        if (tapNodeTag == "ItemNodeforShop")
        {
            itemUseButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(false);
            Debug.Log("<color=orange> ShowItemData Init! Return</color>");
            return;
        }
        if (defi.itemType == ItemDefinition.ItemType.Equipment)
        {
            equipButton.gameObject.SetActive(true);
            itemUseButton.gameObject.SetActive(false);
        }
        else
        {
            itemUseButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
            Debug.Log("<color=orange> ShowItemData Init!</color>");

        }
    }


    /// <summary>
    /// ItemDataPanelの表示位置の設定.
    /// </summary>
    /// <param name="tapPosition"></param>
    private void SetShowPosition(int tapPosition)
    {
        Vector2 vector2;
        if ((Screen.width / 2) < tapPosition) vector2 = new Vector2(0, 0);    //ItemDataPanelを画面左側に表示.
        else vector2 = new Vector2(rectTrXPos, 0);                           //右に表示.		

        GetComponent<RectTransform>().anchoredPosition = vector2;   //anchoredPosition:アンカー基準点に対する RectTransform の相対的なピボットの位置
    }




    /// <summary>
    /// 同じNodeがTapされた場合このオブジェを非activeにする.
    /// 同じNodeがTapされた場合True.
    /// </summary>
    private bool CheckRetap(int tapInstanceId)
    {
        if (tempTapedItemNodeInstanceId == tapInstanceId)
        {
            tempTapedItemNodeInstanceId = 0;
            this.gameObject.SetActive(false);
            Debug.Log("CheckRetap  true");
            return true;
        }
        else
        {
            tempTapedItemNodeInstanceId = tapInstanceId;
            Debug.Log("CheckRetap  false");
            return false;
        }
    }

    //EquipmentのときEquipmentDefinitionの情報を取得して表示
    private void ShowEquipmentDefinition(EquipmentDefinition eD)
    {
        equipablePlaceText.text = eD.ePlaceType.ToString();
        //equipableLevelText.text = eD.canEquipLevel.ToString();		Equip可能Level
    }


    /// <summary>
    /// EquipmentStatusPanelに装備のStatusをセット.
    /// </summary>
    private void SetEquipmentStatus(EquipmentStatus eStatus)
    {
        pwrValueText.text = Plus + eStatus.epwr.ToString();
        dexValueText.text = Plus + eStatus.edex.ToString();
        defValueText.text = Plus + eStatus.edef.ToString();
        matValueText.text = Plus + eStatus.emat.ToString();
        mdeValueText.text = Plus + eStatus.emde.ToString();
        agiValueText.text = Plus + eStatus.eagi.ToString();
        increValueText.text = eStatus.increvalue.ToString();
        increMaxValueText.text = eStatus.incremaxvalue.ToString();
        // equipLevelText.text = "Lv." + eStatus.level.ToString();       
        canEquipLevel.text = eStatus.canEquipLevel.ToString();

        levelStarUI.ShowEquipLevelStart(eStatus.level);
    }

    //「装備する」or「外す」ButtonEvent.
    public void OnEquipActionButton()
    {
        if (tempNode == null) return;
        equipCtrl.EquipOrUnequip(tempNode);
        tempNode = null;
    }

    //「使う」ボタンEvent.
    public void OnUseItem()
    {
        if (tempNode == null) return;
        Debug.Log("tempNode!=null");
        if (!ibm.CanInputAccept) return;
        Debug.Log("ibm.CanInputAccept");

        itemOut.UseItem(tempNode);
        //SE?

        //表示消す?
        this.gameObject.SetActive(false);
    }

}

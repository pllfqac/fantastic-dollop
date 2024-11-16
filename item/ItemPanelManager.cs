using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using TMPro;

/// <summary>
/// Canvas-Configrations-ItemPanel.
/// Seedも扱う.
/// </summary>
public class ItemPanelManager : MonoBehaviour {

    [NonSerialized]
    public IItemDelete itemDelete;      //instant.

    [SerializeField]
    private ItemScrollController itemScCtrl=null;        //Inspe.

	///Canvas-Configrations-ItemPanel-OwnSeedText.
	[SerializeField]
	private TextMeshProUGUI ownSeedText = null;
//	public IOwnSeed ownSeed = null;	//Instant.

    /// <summary>
    /// EquipmentPanleと共用するItemDataPanel.
    /// </summary>
    [SerializeField]
    private GameObject itemDataPanel=null;


    //Itemを捨てるときの確認Panel.汎用YesNoPanel
    [SerializeField]
    private GameObject itemThrowAwayVerifPanel = null;
  //  private YesNoPanel yesNo;
    //Itemを捨てるときの確認Panelが表示されたときのみ使用するTemp
    private INodeData tempINode;

    /// <summary>
    /// ItemCountChangePanel.
    /// 捨てるアイテムがhash無しの場合,捨てる数を決めるようのオブジェ.CPUShopのを使いまわす.
    /// </summary>
    [SerializeField]
    private GameObject ItemCountChangePanel = null;

    /// <summary>
    /// Item削除可能ならTrue.
    /// </summary>
    private bool canItemDelete = false;

    /// <summary>
    /// 現在のアイテム所持数(Abs数)
    /// </summary>
    [NonSerialized]
    public IGetUserOwnItemCount itemOwnCount;          //Instant.
    /// <summary>
    /// 最大所持可能数.
    /// </summary>
    [NonSerialized]
    public IItemIn itemOwnMaxCount;                 //Instat.
    /// <summary>
    /// Userが現在所持しているアイテム数(Abs数)の表示用Text.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI userOwnItemCountText;
    /// <summary>
    /// Userが所持できるアイテム数(Abs数)の最大値の表示用Text.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI userOwnItemMaxText;


    //CanvasManagerのItemPanel Onで呼ばれる.このスクから表示するときのみItem削除可能とする
    public void ShowItemList()
    {
        canItemDelete = true;
        itemScCtrl.ShowUserOwnItemList();
        ownSeedText.text = GameObject.FindWithTag("single2").GetComponent<IMyPlayerDataRef>().OwnSeed.ReadOwnSeed().ToString();  //ItemPanelのOnでSeed表示更新
        ShowItemCountText();
	} 
 
    public void OnDisable()
    {
        itemDataPanel.SetActive(false);
        itemThrowAwayVerifPanel.SetActive(false);
        canItemDelete = false;
    }

    /// <summary>
    /// Item削除確認Panel表示. 
    /// </summary>
    /// <param name="nodeData">削除対象のNodeData</param>
    public void ShowItemThrowAwayVerificationPanel(INodeData nodeData)
    {
        if (!canItemDelete) return;
     //   itemThrowAwayVerifPanel.SetActive(true);
     //   if (yesNo == null) yesNo = itemThrowAwayVerifPanel.GetComponent<YesNoPanel>();

        tempINode = nodeData;       //選択されたNodeを一時保管.
        //YesNoPanelのボタンが押されたときに発生するイベントを登録.
        //YesButtonは,Hash付きならそのまま削除処理へ進む処理.Hash無しなら個数選択の処理へ進む処理のどちらかを選択.
        itemThrowAwayVerifPanel.GetComponent<YesNoPanel>().Init(nodeData.GetOneAbsItemClass().definition.itemName+ "を捨てますか?", YesButtonHandler, NoButtonHandler);
    }

    //================汎用ボタンイベントの実装的な.Actionに登録してbuttonイベントで呼ばれる処理================
    public void YesButtonHandler()
    {
        if (tempINode == null) return;
        if (itemDataPanel.activeSelf) itemDataPanel.SetActive(false);
        if (tempINode.GetGUID() != null)
        {
            Debug.Log("Hash有る時　YesButton");     //Hashある時用.  
            itemDelete.Delete(tempINode,1);
        }
        else
        {
            Debug.Log("Hash無い時　YesButton");                                //Hash無しアイテムのとき.
            //個数が１つのみのときは個数選択無し.
            if (tempINode.GetOneAbsItemClass().iCt == 1) SelectDeleteItemCount(1);
            else
            {
                //個数が2個以上
                ItemCountChangePanel.SetActive(true);
                ItemCountChangePanelCtrl iccpc = ItemCountChangePanel.GetComponent<ItemCountChangePanelCtrl>();
                iccpc.itemCountEntered += SelectDeleteItemCount;
                iccpc.Init((byte)tempINode.GetOneAbsItemClass().iCt);
            }
        }
        ShowItemCountText();
    }

    /// <summary>
    /// ItemCountChange終了で呼ばれる.
    /// </summary>
    /// <param name="deleteCount"></param>
    private void SelectDeleteItemCount(byte deleteCount)
    {
        itemDataPanel.SetActive(false);
        ItemCountChangePanel.SetActive(false);
        if (deleteCount == 0) return;

        itemDelete.Delete(tempINode, deleteCount);
    }


    public void NoButtonHandler()
    {
        Debug.Log("No Button");
        //SE?
        tempINode = null;
    }

    /// <summary>
    /// 右上のItem所持数/最大所持可能数　のText表示.
    /// </summary>
    private void ShowItemCountText()
    {
        userOwnItemCountText.text = itemOwnCount.GetOwnItemCount().ToString();
        userOwnItemMaxText.text = "/"+itemOwnMaxCount.CanOwnItemMaxCount().ToString();
    }
}

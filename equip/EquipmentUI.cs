using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Canvas-EquipmentPanel.
/// 装備用UI.
/// </summary>
public class EquipmentUI : MonoBehaviour ,IEquipmentUI{

    [SerializeField]
    private GameObject itemDataPanel=null;

    /// <summary>
    /// ItemNode.
    /// 装備UI用に生成後に不要なコンポーネントを削除する.
    /// </summary>
    [SerializeField]
    private RectTransform node = null;

    //装備した場合にNodeを貼り付ける各Panel.各Panelの子に出来るItemNodeプレファブ(RectTransform)は一つのみ.
    [SerializeField]
    private RectTransform headPanel = null;
    [SerializeField]
    private RectTransform bodyPanel = null;
    [SerializeField]
    private RectTransform backPanel = null;
    [SerializeField]
    private RectTransform arm1Panel = null;
    [SerializeField]
    private RectTransform arm2Panel = null;
    [SerializeField]
    private RectTransform legsPanel = null;



    private void OnDisable()
    {
        itemDataPanel.SetActive(false);
    }


    /// <summary>
    /// 「装備する」場合.
    /// EquipmentPanel上の6つの装備箇所のうち対応するPanelを選択してItemNode(の一部コピーしたもの)を張り付ける.
    /// Nodeから不要なコンポーネントを削除する.
    /// </summary>
    /// <param name="eNodeProp">装備するアイテムのNodeのNodeProp(つまりTapされたアイテム)</param>
    public void EquipSetUI(ItemNodeProp eNodeProp)
    {        
        RectTransform eNode = Instantiate(node) as RectTransform;       //EquipmentUi上のnodeをPrefabから生成.
        EquipmentNodeInit(eNode, eNodeProp);

        UnequipSetUI(eNodeProp.oneItem.eDefinition.ePlaceType);                  //すでに存在する場合は消してから.

        switch (eNodeProp.oneItem.eDefinition.ePlaceType)
        {
            case CharacterEquipmentPlace.EquipPlaceType.head: eNode.SetParent(headPanel, false); break;
            case CharacterEquipmentPlace.EquipPlaceType.body: eNode.SetParent(bodyPanel, false); break;
            case CharacterEquipmentPlace.EquipPlaceType.back: eNode.SetParent(backPanel, false); break;
            case CharacterEquipmentPlace.EquipPlaceType.arm1: eNode.SetParent(arm1Panel, false); break;
            case CharacterEquipmentPlace.EquipPlaceType.arm2: eNode.SetParent(arm2Panel, false); break;
            case CharacterEquipmentPlace.EquipPlaceType.legs: eNode.SetParent(legsPanel, false); break;
        }
        //test
        var val = eNode.GetComponent<ItemNodeProp>().oneItem.GUID;
        Debug.Log("GUID:" + val);
    }

    /// <summary>
    /// 生成したNodeの共通処理.
    /// </summary>
    /// <param name="newCreateNode">EquipmentUi上に表示する用に生成したNode</param>
    /// <param name="eqNode">TapされたNode</param>
    private void  EquipmentNodeInit(RectTransform newCreateNode,ItemNodeProp eqNode)
    {
        //生成したNodeのサイズ変更.
     //   LayoutElement nodeEle=newCreateNode.GetComponent<LayoutElement>();
     //   nodeEle.minHeight = 1;      //でかい値が入ってるのでテキトウ.
     //   nodeEle.preferredHeight = 70;
     //   nodeEle.preferredWidth = 500;

        ItemNodeProp newCreateNodeProp = newCreateNode.GetComponent<ItemNodeProp>();
       newCreateNodeProp.itemDataUi = itemDataPanel.GetComponent<IItemDataUI>(); //TapしたときのItemDataPanelの表示用.

		/*参照で渡しても内部のメンバ(OneAbsなど)は渡せないっぽい*/
        newCreateNodeProp.oneAbs = eqNode.oneAbs;
        newCreateNodeProp.oneItem = eqNode.oneItem;
        newCreateNode.transform.Find("NodeImage").GetComponent<Image>().sprite = eqNode.GetNodeImage().sprite;
        newCreateNode.transform.Find("FrameImage").gameObject.SetActive(false);
        //不要なコンポーネントの消去等.
        Destroy(newCreateNode.transform.Find("ItemCount").gameObject);
        Destroy(newCreateNode.GetComponent<DragMoveObject>());
    }




    /// <summary>
    /// 装備を「外す」.
    /// </summary>
    /// <param name="placeType"></param>
    public void UnequipSetUI(CharacterEquipmentPlace.EquipPlaceType placeType)
    {
        //各Panelの子として存在する(Prefabから生成した)Nodeオブジェを削除する.
        switch (placeType)
        {
            case CharacterEquipmentPlace.EquipPlaceType.head: UnEquipOne(headPanel); break;
            case CharacterEquipmentPlace.EquipPlaceType.body: UnEquipOne(bodyPanel); break;
            case CharacterEquipmentPlace.EquipPlaceType.back: UnEquipOne(backPanel); break;
            case CharacterEquipmentPlace.EquipPlaceType.arm1: UnEquipOne(arm1Panel); break;
            case CharacterEquipmentPlace.EquipPlaceType.arm2: UnEquipOne(arm2Panel); break;
            case CharacterEquipmentPlace.EquipPlaceType.legs: UnEquipOne(legsPanel); break;
        }
        itemDataPanel.SetActive(false);
    }

    private void UnEquipOne(RectTransform targetRectTr)
    {
        foreach (Transform child in targetRectTr)
        {
			if (child.tag == "Image") continue;		 //"Image"タグをつけているアイコンは除く.
			Destroy(child.gameObject);
        }
    }
    
}

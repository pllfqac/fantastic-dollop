using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

/// <summary>
/// MainScene-Canvas-ItemPanel-ScrollView-Content.
/// Userの所持するItemを表示する.Item毎にNodeを生成してScrollViewにセットする.
/// (2021.10 追)StaticMyClass.maxItemOwnCountより所持数が少ない場合は空のNodeで埋める仕様に変更.
/// </summary>
public class ItemScrollController : MonoBehaviour, ISelectable
{

    //このClassのメソッドの引数にとるほとんどのOneAbsItemClassについて.Update値のみで構成された変数である点に注意する.UserOwnItemTableの全所持アイテムと区別して考える.
    //prefsにデータがなくてもitemを所持している場合はItemPanelへのNode生成はする.

    //Inspe.
    [SerializeField]
    private PoolBody poolBody = null;
    [SerializeField]
    private SkillSetManager skillSetManager = null;
    [SerializeField]
    private FlickSetManager flickSet = null;
    [SerializeField]
    private BoxSetManager boxSet = null;
    [SerializeField]
    private ParameterTable ItemDefinitionTable = null;  //Itemの定義ScriptableObject.

    [SerializeField]
    private ItemDataUI itemDataUi = null;
    [SerializeField]
    private ItemPanelManager ipm = null;


    /// <summary>
    /// 所持アイテムリストのNodeのリスト.
    /// 空のNodeは除く
    /// </summary>
    private List<INodeData> itemNodeList = new List<INodeData>();

    /// <summary>
    /// 空のNodeList.
    /// </summary>
    //private List<INodeData> emptyNodeList = new List<INodeData>();

    /// <summary>
    /// PrefsからReadしたItemのtemp.
    /// </summary>
    private List<PrefsSaveDataClass> tempList = null;



    /// <summary>
    /// Canvas-ItemPanelのUser所持アイテムUI表示.
    /// </summary>
    public void ShowUserOwnItemList()
    {
        StartCoroutine(ShowAllNode());
    }


    //Read時のPlayerPrefsから読込んだItem設定の一時置きをする.この処理がRaiseより遅い場合はPrefsデータ展開を諦める仕様.
    public void TempReadItemData(List<PrefsSaveDataClass> prefsSaveLis)
    {
        tempList = prefsSaveLis;
    }

    //UserOwnItemTableから呼ばれる.Login時EEventType.UserRequestResultのフロー.所持アイテムがなければ呼ばれない.
    public void SetUserItemDataUseTempdata(IUserOwnItemTable uoiTable)
    {
        CreateUserOwnItemListByUserOwnItemTable(uoiTable);
        HideAllNode();
        if (tempList != null) SetBoxforItem(CreateBoxNumAndINodedataTable());   //Prefsがあれば,対応するBoxにset.
    }

    //User所持ItemからUI用INodeDataのリストを作成.
    private void CreateUserOwnItemListByUserOwnItemTable(IUserOwnItemTable uoi)
    {
        Dictionary<int, OneAbsItemClass> itemTable = uoi.GetUserOwnItemTable();
        if (itemTable == null) return;
        foreach (OneAbsItemClass oai in itemTable.Values.ToList())
        {
            InstantiateItemNodes(oai, uoi);
        }
        SetEvent(uoi);
    }

    //Login直後のItemNode生成終了(新規などItemなければ即)でEvent登録.
    public void SetEvent(IUserOwnItemTable uoi)
    {
        uoi.SetEvent(InstantiateItemNodes, SelectDeleteNode, NodeTextUpdateCount);
    }



    //OneAbsItemClassずつ(個数分)Nodeの生成.Login後はeventでUpdateItem()が呼ばれたときにこのメソッドも呼ばれる.
    private void InstantiateItemNodes(OneAbsItemClass oneAbs, IUserOwnItemTable uoi)//Dictionary<int,OneAbsItemClass> uoiTable)
    {
        //       ItemDefinition iDefi = oneAbs.definition;           //Login後のAddは,引数にはDefinitionの参照を取得していない.
        oneAbs.definition = ItemDefinitionTable.GetItemDefinition(oneAbs.iAbs);
        if (oneAbs.definition.itemRank == 1)
        {
            //Hash付きアイテムの場合個数分Nodeの生成.現状まとめない仕様.
            foreach (OneItemClass oneItem in oneAbs.oil)
            {
                InstantiateOneItemNode(oneAbs, oneItem, uoi);
            }
        }
        else
        {
            InstantiateOneItemNode(oneAbs, null, uoi);
        }
    }


    /// <summary>
    /// 個々のアイテムのNode生成.
    /// GUID付はItemNodePropのItemCountが1になるので注意.
    /// </summary>
    /// <param name="oneAbs">更新値のみ含んだOneAbsItemClass</param>
    /// <param name="oneItem">Hash無しItemの場合はnull</param>
    private void InstantiateOneItemNode(OneAbsItemClass oneAbs, OneItemClass oneItem, IUserOwnItemTable uoi)//Dictionary<int,OneAbsItemClass> uoiTable)
    {
        RectTransform itemNode = poolBody.ItemNodePlace();
        itemNode.SetParent(transform, false);
        ItemNodeProp itemNodePr = itemNode.GetComponent<ItemNodeProp>();
        itemNodeList.Add(itemNodePr);

        itemNodePr.oneAbs = uoi.GetUserOwnItemTable()[oneAbs.iAbs];               //UserOwnItemTableのDictionaryへの参照を入れる.           

        itemNodePr.oneItem = oneItem;       //ここでhashとかの参照ゲットしてる.

        itemNode.GetComponent<DragMoveObject>().Init(itemNodePr);
        itemNodePr.countText = itemNode.Find("ItemCount").GetComponent<Text>();         //個数表示Textへの参照は取得してoku.
        itemNodePr.countText.text = oneItem == null ? oneAbs.iCt.ToString() : 1.ToString();

        itemNode.Find("NodeImage").GetComponent<Image>().sprite = oneAbs.definition.sprite;
        itemNodePr.itemDataUi = itemDataUi;
        itemNode.GetComponent<NodeTap>().ipm = this.ipm;    //長押しのとき処理用.  
    }


    //全Nodeの非表示.
    private void HideAllNode()
    {
        foreach (INodeData inode in itemNodeList)
        {
            inode.SetActive(false);
        }
    }

    private IEnumerator ShowAllNode()
    {
        foreach (INodeData inode in itemNodeList)
        {
            yield return null;
            inode.SetActive(true);
        }
    }


    /// <summary>
    /// 削除対象のINodeDataを取得.
    /// </summary>
    /// <param name="oai">削除対象の1つのOneAbsItemClass.</param>
    /// <param name="uoiTable">userOwnItemTable更新後のuserOwnItemTable参照</param>
    private void SelectDeleteNode(OneAbsItemClass oai, IUserOwnItemTable uoi)
    {
        if (oai.oil != null)
        {
            //hash付の場合INodeは複数取得の可能性ある.Hashで検索.
            List<INodeData> deleteChoiceINodeList = new List<INodeData>();      //temp.
            //引数から対応するINodeDataを探す.
            var nodes = itemNodeList.Where(x => x.GetAboluteNum() == oai.iAbs);
            foreach (OneItemClass oic in oai.oil)
            {
                INodeData node = nodes.First(i => i.GetGUID() == oic.GUID);         //1つのみの取得のはず.なければ例外発生.                      
                deleteChoiceINodeList.Add(node);
            }
            DeleteNodeList(deleteChoiceINodeList);
        }
        else
        {
            DeleteNode(itemNodeList.First(x => x.GetAboluteNum() == oai.iAbs));
        }
    }

    /// <summary>
    /// Item Nodeの削除.
    /// itemNodeListから除外.UIの削除等etc.
    /// </summary>
    /// <param name="DeleteNodeLis">SelectDeleteNode()で作成した削除対象INodeのリスト.</param>
    private void DeleteNodeList(List<INodeData> DeleteNodeLis)
    {
        foreach (INodeData node in DeleteNodeLis)
        {
            DeleteNode(node);
        }
    }

    private void DeleteNode(INodeData node)
    {
        itemNodeList.Remove(node);
        node.ReturnPool();
        skillSetManager.ConsumeResultCount0(node);
    }

    //PrefsデータとServerから取得した所持ItemDictionaryから，Dictionary<BoxNumber,INodeData>を作成する.
    private Dictionary<int, INodeData> CreateBoxNumAndINodedataTable()
    {
        Dictionary<int, INodeData> boxTable = new Dictionary<int, INodeData>(); //TKey:BoxNum.
        foreach (PrefsSaveDataClass psd in tempList)
        {

            if (string.IsNullOrEmpty(psd.itemHash))
            {   //Hashなし
                var iNode = itemNodeList.FirstOrDefault(absNum => absNum.GetAboluteNum() == psd.AbsoluteNumber);
                if (iNode == null) continue;
                boxTable[psd.BoxNumber] = iNode;
            }
            else
            {
                //HashありならそれのINodeDataを.
                var iNode = itemNodeList.FirstOrDefault(node => node.GetGUID() == psd.itemHash);
                if (iNode == null) continue;
                boxTable[psd.BoxNumber] = iNode;
            }
        }
        return boxTable;
    }


    //Dictionary<BoxNumber,INodeData>を使ってBoxにSetする.
    private void SetBoxforItem(Dictionary<int, INodeData> iNodeMap)
    {
        if (iNodeMap == null) return;
        //DropItemBufferまでの登録
        flickSet.setPrefsFlick(iNodeMap);
        boxSet.PrefsSkillSetCall(iNodeMap);
        skillSetManager.SetUpFinishSkillSetPrefs(iNodeMap);
    }

    /// <summary>
    /// ItemNodeの個数の表示変更.Hash付はこのメソッドに入ってはならない.
    /// </summary>
    /// <param name="node">対象のItemNode</param>
    /// <param name="updateCount">更新したい値.上書き.</param>
    private void NodeTextUpdateCount(OneAbsItemClass oneAbs, IUserOwnItemTable uoi)//Dictionary<int,OneAbsItemClass> updatedTable)
    {
        //INodeDataの検索.
        if (itemNodeList.Any(x => x.GetAboluteNum() == oneAbs.iAbs))
        {
            INodeData node = itemNodeList.First(x => x.GetAboluteNum() == oneAbs.iAbs);
            node.countText.text = uoi.GetUserOwnItemTable()[oneAbs.iAbs].iCt.ToString();
        }
        else
        {
        }
    }

    /// <summary>
    /// Shopで「売却」がキャンセルされたときにCountのUIを元に戻す.
    /// </summary>
    /// <param name="selectedNodes">Shopでの「売却」で売る選択をされたアイテムリスト</param>
    public void ResetItemCountUI(List<ItemNodeProp> selectedNodes)
    {
        if (selectedNodes == null || selectedNodes.Count == 0) return;
        foreach (ItemNodeProp node in selectedNodes)
        {
            ItemNodeProp ownItemNode = null;

            //GUID付は1NodeでCount=1(参照しているuserOwnItemAbsClassのCountは1以上の可能性がある)ので1を入力.
            var ownNodes = itemNodeList.Where(x => x.GetAboluteNum() == node.GetAboluteNum());      //所持リストからまず同Absを取得.GUID付の場合複数の可能性.
            if (ownNodes.Count() >= 2)
            {
                foreach (ItemNodeProp ownNode in ownNodes)
                {
                    if (ownNode.GetGUID() == node.GetGUID())
                    {
                        ownNode.transform.Find("ItemCount").GetComponent<Text>().text = 1.ToString();
                        break;                      //このforeachから抜け出す.
                    }
                }
            }
            else
            {
                ownItemNode = (ItemNodeProp)ownNodes.First();
                ownItemNode.transform.Find("ItemCount").GetComponent<Text>().text = ownItemNode.oneAbs.iCt.ToString();

            }
        }
    }

    /// <summary>
    /// 指定したGUIDを元に,itemNodeListから対応するItemNodePropを取得する.
    /// </summary>
    /// <param name="GUID">検索に使うGUID.</param>
    /// <returns>指定したGUIDのItemNodePropがなければ,Null</returns>
    public ItemNodeProp SelectNodeProp(string GUID)
    {
        if (!itemNodeList.Any(x => x.GetGUID() == GUID)) return null;
        return (ItemNodeProp)itemNodeList.First(x => x.GetGUID() == GUID);
    }

}

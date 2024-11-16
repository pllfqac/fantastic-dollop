using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using System.Threading.Tasks;
//using UtageExtensions;

/// <summary>
/// Player.
/// Userから所持itemが離れるときに,必ずこのClassを経由する.
/// </summary>
public class ItemOut : MonoBehaviour, IItemOut, IItemDelete
{
    /// <summary>
    /// Canvas-ItemBoxPanel-各Button-各NumberOfPossessionsTextにて,消費系アイテムの保有数を表示する.
    /// </summary>
    [NonSerialized]
    public ItemBoxNumberTextManager ibnt = null;        //ins.
    [NonSerialized]
    public DBItemSave dbItemSave;   //UniMasOnly.callback.
    private Billing billing;        //UniMasOnly.


    [SerializeField]
    private ParameterTable prmTable = null;     //Inspe.

    private PhotonView view;
    private IRevi revi;
    private IUserOwnItemTable userOwnItemTable;
    //   private IMP mp;
    //状態異常回復
    private IEndUccCondition endUccCondition;
    private IBuff buff;                 //MasterOnly.
    private ICharaDelayTime charaDelayTime;
    private IItemIn checkOwnItem;
    private ICheckEquiped checkEquiped;
    private GameObject single2;

    private Dictionary<int, OneAbsItemClass> outDictionary = new Dictionary<int, OneAbsItemClass>();
    private OneAbsItemClass outOneAbs = new OneAbsItemClass();


    private void Start()
    {
        view = GetComponent<PhotonView>();
        revi = GetComponent<Revival>();
        single2 = GameObject.FindWithTag("single2");
        userOwnItemTable = single2.GetComponent<IMyPlayerDataRef>().MyItemTable;
        charaDelayTime = GetComponent<ICharaDelayTime>();
        checkOwnItem = GetComponent<IItemIn>();
        checkEquiped = GetComponent<ICheckEquiped>();

        if (PhotonNetwork.IsMasterClient && GameObject.FindWithTag("single1").GetComponent<PUNController>().TargetRoom == PUNController.Room.CommonRoom)
        {
            buff = GetComponent<IBuff>();
            endUccCondition = GetComponent<IEndUccCondition>();
            billing = single2.GetComponent<Billing>();
        }
    }

    #region UserOnly.


    /// <summary>
    /// ItemCount更新時にUOIから呼ばれる.
    /// </summary>
    /// <param name="oai"></param>
    /// <param name="Uoi"></param>
    public void ChangeCount(int itemAbs, int itemCount)
    {
        //更新があったItemのAbsと更新後の個数を取得してUIを更新する.
        ibnt.UpdatePossNum(itemAbs, itemCount);
    }



    /// <summary>
    /// ItemCountの確認.
    /// そもそもCount0ではメソッドに入らないと思うが一応.
    /// </summary>
    /// <param name="node"></param>
    private void CheckCount(INodeData node)
    {
        if (node.GetOneAbsItemClass().iCt <= 0) throw new MyUniException(StaticError.ErrorType.CountOver);
    }

    /// <summary>
    /// Flick,Boxにセットされた「使用」可Itemの使用 or ItemUseButtonの「使う」の押下.
    /// </summary>
    /// <param name="ItemNode"></param>
    public void UseItem(INodeData ItemNode)
    {
        Debug.Log("Use Item.  AbsNum:" + ItemNode.GetAboluteNum());
        CheckCount(ItemNode);
        //このメソッドでは回復系か持続系アイテムしか以下の処理しない(仮).分類分けした後に使用する.大分類→小分類.
        ItemClassification(ItemNode, ItemNode.GetOneAbsItemClass().definition.itemType);

        charaDelayTime.StartDelayTime(StaticMyClass.useItemDelayTime);
    }

    /// <summary>
    /// UserからMasterへの「使用」可Itemの使用RPC送信.
    /// </summary>
    /// <param name="ItemAbsNum"></param>
    private void SendItemUseRPCToMaster(INodeData node)
    {
        Debug.Log("masterへRPC!");
        //Hash有無で分岐.
        if (node.GetOneItemClass() == null)
        {
            view.RPC(nameof(ItemOut.ItemUseRPC)/*"ItemUseRPC"*/, RpcTarget.MasterClient, (byte)node.GetAboluteNum());       //AbsNumはByteに変換して送る.
        }
        else
        {
            if (string.IsNullOrEmpty(node.GetOneItemClass().GUID)) throw new MyUniException(StaticError.ErrorType.HashError); //OneItemClass!=nullでhashなしは不正とする.
            else view.RPC(nameof(ItemOut.HashItemUseRPC)/*"HashItemUseRPC"*/, RpcTarget.MasterClient, (byte)node.GetAboluteNum(), node.GetOneItemClass().GUID);   //Hash付の場合、hashもMasterへ送る.
        }
    }

    /// <summary>
    /// Item大分類.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="type"></param>
    private void ItemClassification(INodeData node, ItemDefinition.ItemType type)
    {
        switch (type)
        {
            case ItemDefinition.ItemType.ReviItem:
                if (!PhotonNetwork.IsMasterClient) SendItemUseRPCToMaster(node);           //MasterへRPC.Testようにif
                UseReviItem(node);
                break;
            case ItemDefinition.ItemType.DurationItem:
                if (!PhotonNetwork.IsMasterClient) SendItemUseRPCToMaster(node);
                UseDurationItem(node);
                break;
            case ItemDefinition.ItemType.Equipment:     //ここでは何もしない.
                break;
            case ItemDefinition.ItemType.NoUseItem:     //ここでは何もしない.
                break;
            case ItemDefinition.ItemType.ResusItem:
                if (!PhotonNetwork.IsMasterClient) SendItemUseRPCToMaster(node);           //MasterへRPC.Testようにif
                UseResusItem(node);
                break;
            case ItemDefinition.ItemType.other:     //ここでは何もしない.
                break;
            case ItemDefinition.ItemType.Gacha:
                if (!checkOwnItem.CanBilling()) return;  //User側Check.空きがない
                if (!PhotonNetwork.IsMasterClient) SendItemUseRPCToMaster(node);           //MasterへRPC.Testようにif
                ItemOutUpdateItemTable(node);
                break;
            case ItemDefinition.ItemType.book:
                BookProperty book = prmTable.GetBook(node.GetAboluteNum());

                //if (knowledge == null) throw new Exception();
                IKnowledge knowledge = single2.GetComponent<IKnowledge>();
                if (!knowledge.CanReadBook(book)) return;
                if (!PhotonNetwork.IsMasterClient) SendItemUseRPCToMaster(node);           //MasterへRPC.
                ItemOutUpdateItemTable(node);
                knowledge.StartReading(book);
                break;
            default: break;
        }
    }

    //回復系Itemの小分類.
    private void UseReviItem(INodeData node)
    {
        ItemOutUpdateItemTable(node);           //UserOwnItemTable.UpdateItem()呼出し.個数変更&UI個数変更.

        ReviItemDefinition recoDefi = prmTable.GetDefiRecovery(node.GetAboluteNum());
        switch (recoDefi.recoveryType)
        {
            case ReviItemDefinition.itemTypeSecondClassification.HPRecovery:
                revi.ItemUseHPRevi(recoDefi.recoveryValue);                      //HP回復.現状ではHPMaxでも使用できてしまう.この場合個数減少のみ.
                break;
            case ReviItemDefinition.itemTypeSecondClassification.MPRecovery:
                // mp.ReviMP(recoDefi.recoveryValue);
                revi.ItemUseMPRevi(recoDefi.recoveryValue);
                break;
            case ReviItemDefinition.itemTypeSecondClassification.SPRecovery:
                break;
            case ReviItemDefinition.itemTypeSecondClassification.ButCondiRecovery:
                endUccCondition.ForceEndUCondition();
                break;
        }
    }

    private void UseDurationItem(INodeData node)
    {
        ItemOutUpdateItemTable(node);
    }


    /// <summary>
    /// UserOwnItemTableを更新します.
    /// </summary>
    private void ItemOutUpdateItemTable(INodeData node)
    {
        //更新用のDictionaryの作成.Hash付もこのメソッドに入る.
        if (outOneAbs.oil != null) outOneAbs.oil = null;

        outOneAbs.iAbs = node.GetAboluteNum();
        outOneAbs.iCt = -1;                               //使用は基本「１」づつ.
        if (node.GetOneItemClass() != null)
        {
            outOneAbs.oil = new List<OneItemClass>() { node.GetOneItemClass() };
            string hash = node.GetOneItemClass().GUID;
        }
        outDictionary.Clear();
        outDictionary[node.GetAboluteNum()] = outOneAbs;
        //UserOwnItemTableを更新.
        userOwnItemTable.UpdateItem(outDictionary);
    }


    //=====================Shopでの売却=====================

    /// <summary>
    /// userOwnItemTableから売却アイテムを取り除く.
    /// </summary>
    /// <param name="sellItemLis"></param>
    /// <returns></returns>
    public Dictionary<int, OneAbsItemClass> DeleteSelectedSellItems(List<ItemNodeProp> sellItemLis)
    {
        //ItemNodePropをDictionary<int,OneAbs>に変換.
        //GUID付が複数ある場合まとめる.itemCountはマイナスにする.
        Dictionary<int, OneAbsItemClass> deleteTable = new Dictionary<int, OneAbsItemClass>();      //削除用Table.

        foreach (ItemNodeProp node in sellItemLis)
        {
            if (deleteTable.ContainsKey(node.GetOneAbsItemClass().iAbs))
            {
                //削除用Tableにすでに同Absが含まれている場合,GUID付ItemであるのでoneItemClassに追加する.
                deleteTable[node.oneAbs.iAbs].oil.Add(node.oneItem);      //OneItemClassのついか  
                deleteTable[node.oneAbs.iAbs].iCt = deleteTable[node.oneAbs.iAbs].oil.Count;      //Count数更新.GUID数=Count.
            }
            else
            {
                OneAbsItemClass aoi = new OneAbsItemClass();
                aoi.iAbs = node.oneAbs.iAbs;
                aoi.iCt = node.oneAbs.iCt;

                // if(node.oneItem!=null && node.oneItem.GUID!=null)
                if (node.oneAbs.oil != null)    /*node生成時GUID付でなければ明示的にnull入れてる.  条件判断でList<OneItemClass> = null とOneItemClass=null　がごっちゃになってる可能性ある.*/
                {
                    aoi.oil = new List<OneItemClass>() { new OneItemClass(node.GetGUID()) };        //NodeのGUIDを入れる.
                }
                else
                {
                    aoi.oil = null;

                }
                deleteTable[aoi.iAbs] = aoi;
            }
        }

        ChangeCountMinus(deleteTable);      //UserOwnItemTable用にItemCountをマイナスに変える.
                                            //   Hoge(deleteTable);                  //Test用.

        //User端末のみ処理.PCdebugはraiseの後ItemUpdate()でアイテムを削除したい為ここではUpdateItem()しない.
        if (!PhotonNetwork.IsMasterClient) userOwnItemTable.UpdateItem(deleteTable);

        return deleteTable;
    }



    //UserOwnItemTable用にItemCountをマイナスに変える.
    private void ChangeCountMinus(Dictionary<int, OneAbsItemClass> deleteTable)
    {
        foreach (var oai in deleteTable)
        {
            oai.Value.iCt = -oai.Value.iCt;
        }
    }

    //===================== Item Delete =====================

    public void Delete(INodeData nodeData, byte deleteCount)
    {
        //装備中のアイテムは消去しない
        if (nodeData.GetOneAbsItemClass().definition.itemType == ItemDefinition.ItemType.Equipment && checkEquiped.IsCheckEquiped(nodeData.GetGUID())) return;

        var itemMap = CreateDeleteItemData(nodeData, deleteCount);
        ChangeCountMinus(itemMap);
        userOwnItemTable.UpdateItem(itemMap);

        SerializationOneAbsItemClass seri = new SerializationOneAbsItemClass();
        seri.i = nodeData.GetAboluteNum();
        seri.i2 = -deleteCount;        //マイナス値
        if (nodeData.GetGUID() != null)
        {
            // Debug.Log("GUID:" + nodeData.GetGUID());
            seri.stLis = new List<string>();
            seri.stLis.Add(nodeData.GetGUID());
        }
        string sendJson = JsonUtility.ToJson(seri);
        Debug.Log("Json:" + sendJson);

        view.RPC("MasterDeleteItem", RpcTarget.MasterClient, sendJson);
    }

    /// <summary>
    /// Itemの消去.1つのみの場合Ver.
    /// </summary>
    /// <param name="oneAbs"></param>
    public void Delete(OneAbsItemClass oneAbs)
    {
        //装備中のアイテムは消去しない
        if (oneAbs.definition.itemType == ItemDefinition.ItemType.Equipment && checkEquiped.IsCheckEquiped(oneAbs.oil.First().GUID)) return;
        Dictionary<int, OneAbsItemClass> itemMap = new Dictionary<int, OneAbsItemClass>();
        itemMap[oneAbs.iAbs] = oneAbs;
        ChangeCountMinus(itemMap);
        userOwnItemTable.UpdateItem(itemMap);

        SerializationOneAbsItemClass seri = new SerializationOneAbsItemClass();
        seri.i = oneAbs.iAbs;
        seri.i2 = -Math.Abs(oneAbs.iCt);        //マイナス値
        if (oneAbs.oil != null && oneAbs.oil.Count != 0)
        {
            Debug.Log("GUID:" + oneAbs.oil.First().GUID);
            seri.stLis = new List<string>();
            seri.stLis.Add(oneAbs.oil.First().GUID);
        }
        string sendJson = JsonUtility.ToJson(seri);
        Debug.Log("Json:" + sendJson);

        view.RPC("MasterDeleteItem", RpcTarget.MasterClient, sendJson);
    }

    //Dictionaryだけど1AbsのみのItemを返す仕様.
    private Dictionary<int, OneAbsItemClass> CreateDeleteItemData(INodeData node, byte deleteCount)
    {
        //消去用Dictionary.
        Dictionary<int, OneAbsItemClass> deleteItemTable = new Dictionary<int, OneAbsItemClass>();
        int targetAbs = node.GetAboluteNum();

        deleteItemTable[targetAbs] = new OneAbsItemClass();
        deleteItemTable[targetAbs].iAbs = targetAbs;
        deleteItemTable[targetAbs].iCt = deleteCount;
        deleteItemTable[targetAbs].definition = node.GetOneAbsItemClass().definition;
        Debug.Log("Abs:" + targetAbs);
        Debug.Log("Delete Count:" + deleteCount);
        //  if (deleteItemTable[targetAbs].oil != null && deleteItemTable[targetAbs].oil.Count != 0)
        if (node.GetGUID() != null)
        {
            deleteItemTable[targetAbs].oil = new List<OneItemClass>();
            deleteItemTable[targetAbs].oil.Add(new OneItemClass(node.GetGUID()));
            Debug.Log("Hash:" + deleteItemTable[targetAbs].oil.First().GUID);

        }
        return deleteItemTable;
    }


    #endregion

}
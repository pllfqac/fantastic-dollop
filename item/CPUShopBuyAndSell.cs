using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using System;

/// <summary>
/// Single2.
/// ShootRayでCPU商人をTapした後&購入したいものを選択して「購入」ボタンを押した後のロジック.
/// </summary>
public class CPUShopBuyAndSell : MonoBehaviour
{

    //cpuShopのEquipmentはBaseDefintionを使用.


    public IOwnSeed ownSeed;        //User→Instant.MasterはIMasterAllMemberTable経由でアクセス.
    [System.NonSerialized]
    public ItemIn itemIn;           //Instant.Master上ではIMAMTから
    [System.NonSerialized]
    public IItemOut itemOut;        //Instant.

    [SerializeField]
    private CPUShopList shopList = null;
    [SerializeField]
    private ParameterTable itemDefinition = null;
    private ICPUShopUI ui;
    private string tempShopTag;

    private RaiseEventClass raise;



    /// <summary>
    /// 利用中のShop定義.Masterは使用不可.
    /// </summary>
    private CPUItemShopMerchandise shopDef = null;

    public enum BorSSelect
    {
        buy,
        sell,
    }
    /// <summary>
    /// 左上の「買う」「売る」ボタン と同期してこの値も変える.どちらが選択されているかの判定に利用.
    /// </summary>
    [System.NonSerialized]
    public BorSSelect BorS = BorSSelect.buy;


    //MasterOnly.
    private IMasterAccessUserOwndataDB mauo;
    //   private IGUIDGenerator guidGene;            //Hashとしてたけど正確にはGUID.
    private IAllocationItemStatus allocation;   //OneItemClassの各メンバにデータの割り当て(GUID,eStatus,eDefinition等).
    private IMasterAllMemberTable mamt;
    private IPlayerTable pTable;

    /// <summary>
    /// 商店利用中か否か.一度にひとつのShopのみ利用できる.
    /// True:利用中.False:利用していない.
    /// </summary>
    private bool isBuyOrSell = false;

    private void Start()
    {
        ui = GameObject.Find("Canvas").GetComponent<CPUShopUI>();
        ui.endEvent += EndCpuShopBuyAndSell;       //商店利用終了でのコールバックを登録しておく.
        raise = GetComponent<RaiseEventClass>();

        if (!PhotonNetwork.IsMasterClient) return;
        mauo = GetComponent<MasterAccessUserOwndataDB>();
        //     guidGene = GetComponent<GUIDGenerator>();
        allocation = GetComponent<IAllocationItemStatus>();
        mamt = GetComponent<MasterAllMemberTable>();
        pTable = GetComponent<IPlayerTable>();
    }

    /// <summary>
    /// ShootRayで呼ばれる.
    /// </summary>
    /// <param name="cpuTag">CPU商人オブジェクトに設定したTag</param>
    public void TapCPUTradesman(string cpuTag)
    {
        //複数呼ばれても一度のみ受け付ける.
        if (isBuyOrSell) return;
        isBuyOrSell = true;
        tempShopTag = cpuTag;
        //TagでShop定義ScriptableObjectを選択.
        shopDef = shopList.SelectShop(cpuTag);

        ui.DisplayCpuShopUI(shopDef, itemDefinition.itemDefinitionList.ToDictionary(x => x.absoluteNumber), ownSeed.ReadOwnSeed());        //UI表示.
    }


    public void EndCpuShopBuyAndSell()
    {
        shopDef = null;
        isBuyOrSell = false;
        tempShopTag = null;
    }




    /// <summary>
    /// 「決定」が押された後(Buy).
    /// </summary>
    /// <param name="buyItemNodeList">SelectPanel-Contentに生成されたNode(Userが購入したいアイテム)のリスト</param>
    public void ItemBuy(List<ItemNodeProp> buyItemNodeList)
    {
        if (buyItemNodeList == null || buyItemNodeList.Count == 0) return;
        //所持数オーバー確認.
        if (CheckAllOwnItemCountOver(buyItemNodeList.Select(x => x.GetAboluteNum()).ToList()))
        {
            ui.ShowCountOverText();
            return;
        }

        //User側合計金額計算.
        // int totalValue = buyItemNodeList.Sum(x => x.mc.price * x.oneAbs.iCt);
        int totalValue = buyItemNodeList.Sum(x => x.oneAbs.definition.value * StaticMyClass.CPUShopBuyCoefficient * x.oneAbs.iCt);
        Debug.Log("Total value:" + totalValue);

        if (IsValueOver(totalValue)) ui.ShowSeedOverText();
        else
        {
            //Debug.Log("アイテムの購入が出来ました!");
            //購入したItemはItemInのキューに追加.この語のMasterからのRaiseでそのキューの要素をUserOwnItemTableへ追加(&有ればhashの)処理をする.
            itemIn.AddBuyItemQueue(buyItemNodeList);
            CheckMasterFlow(buyItemNodeList);       //OKならMasterにraise.
            ownSeed.SeedOut(totalValue);            //User端末のSeed更新はRaise返却を待たずに実施.
            ui.EndBuy(ownSeed.ReadOwnSeed());
        }
    }

    /// <summary>
    /// 「決定」が押された後(Sell).
    /// </summary>
    /// <param name="sellItemNodeList">SelectPanelに生成した売却予定のItemNodeList.</param>
    public void ItemSell(List<ItemNodeProp> sellItemNodeList)
    {
        if (sellItemNodeList == null || sellItemNodeList.Count == 0) return;
        Dictionary<int, OneAbsItemClass> sellItemTable = itemOut.DeleteSelectedSellItems(sellItemNodeList);      //SellはQueueに追加せずにすぐ所持リストから削除.


        string json = SerializeSellTable(sellItemTable);      //OneAbsとItemCount,GUIDのListのみの軽量Classに変換したListのJson.


        //    string json = JsonUtility.ToJson(new Serialization<int, OneAbsItemClass>(sellItemTable));      //売却アイテムTableをSerialize.
        ItemSellRaise(json);
        ui.EndSell();
    }


    /// <summary>
    /// 売却アイテムリストをMasterへRaise.
    /// </summary>
    /// <param name="sellItemJson"></param>
    private void ItemSellRaise(string sellItemJson)
    {
        //ShopTagも送る.
        SimpleClass sc = new SimpleClass();
        sc.data1 = tempShopTag;
        sc.data2 = sellItemJson;
        string json = JsonUtility.ToJson(sc);

        RaiseEventOptions option = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.MasterClient,
        };
        raise.StartRaise((byte)RaiseEventClass.EEventType.itemSell, json, true, option);
    }



    /// <summary>
    /// 購入金額OverCheck.
    /// </summary>
    /// <param name="totalValue">購入予定アイテムの総額</param>
    /// <returns>所持金Overしていなければfalse.Overしていればtrue</returns>
    private bool IsValueOver(int totalValue)
    {
        int MySeed = ownSeed.ReadOwnSeed();
        return (MySeed - totalValue) < 0 ? true : false;
    }

    /// <summary>
    /// 購入予定アイテムと所持アイテムの合計が256(仮)を超えないようにする.超えたらTrue.
    /// </summary>
    /// <param name="nodeProp"></param>
    /// <returns>取得予定値と現在の所持数の合計が最大所持可能数を超えなければfalseを返す.超えたらTrue</returns>
    public bool IsAllBuyItemCountOver(byte absNum, int buyItemCount)
    {
        return itemIn.IsItemOverFlow(absNum, buyItemCount);
    }

    /// <summary>
    /// 購入予定アイテムと所持アイテムの種類(Abs)の合計が50(デフォ)を超えたらTrue.
    /// </summary>
    /// <param name="addItemAbs">購入予定のItemのAbs</param>
    /// <returns>超えたらTrue.</returns>
    public bool CheckAllOwnItemCountOver(IReadOnlyList<int> addItemAbs)
    {
        //すでに所持しているAbsはスルー.255制限についてはここでは考えない.
        int emptyCount = itemIn.CheckUserOwnItemCapacity();
        foreach (var p in addItemAbs)
        {
            if (itemIn.haveOwnItem(p))
            {
                emptyCount++;
                continue;
            }
            else
            {
                if (!itemIn.IsTotalItemCountOver()) emptyCount--; //超えなければfalse.
                else return true;
            }
        }

        return emptyCount < 0 ? true : false;
    }



    private void SeedUpdate(int totalValue)
    {
        ownSeed.SeedOut(totalValue);
    }


    private void CheckMasterFlow(List<ItemNodeProp> nodes)
    {
        string json = SerializeItemNode(nodes);
        Debug.Log("ItemNodeProp JsonSerialize:" + json);
        RaiseEventOptions option = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.MasterClient,
        };
        raise.StartRaise((byte)RaiseEventClass.EEventType.ItemBuy, json, true, option);
    }


    //User購入予定のItemのjsonシリアライズ.
    private string SerializeItemNode(List<ItemNodeProp> nodes)
    {
        //List<ItemNodeProp>をList<SimpleInt2Class>に変換してJson
        List<SimpleInt2Class> sicLis = new List<SimpleInt2Class>();

        foreach (ItemNodeProp node in nodes)
        {
            SimpleInt2Class si2c = new SimpleInt2Class();
            si2c.d = node.oneAbs.iAbs;
            si2c.d2 = node.oneAbs.iCt;                        //※Rank=1の同Abs Itemも纏められるので注意.
            //equipは考慮してない.
            sicLis.Add(si2c);
        }
        Serialization<SimpleInt2Class> siSerialize = new Serialization<SimpleInt2Class>(sicLis);
        //SimpleClassに利用した商店TagとJsonをまとめてさらにJson.
        SimpleClass sc = new SimpleClass();
        sc.data1 = tempShopTag;                             //data1には利用したshopのtagを入力.
        sc.data2 = JsonUtility.ToJson(siSerialize);
        return JsonUtility.ToJson(sc);
    }


    /// <summary>
    /// Item Sell時のserialize. eStatus等Masterへ送る必要のないシリアライズを避ける.
    /// </summary>
    /// <param name="sellTable">売却アイテムのTable</param>
    /// <returns>Json文字列</returns>
    private string SerializeSellTable(Dictionary<int, OneAbsItemClass> sellTable)
    {
        List<SerializationOneAbsItemClass> soaiLis = new List<SerializationOneAbsItemClass>();
        foreach (var oai in sellTable.Values)
        {
            SerializationOneAbsItemClass soai = new SerializationOneAbsItemClass();
            soai.i = oai.iAbs;
            soai.i2 = oai.iCt;
            if (oai.oil != null && oai.oil.Any(a => a.GUID != null))
            {
                soai.stLis = oai.oil.Select(x => x.GUID).ToList();
            }
            else
            {
                soai.stLis = null;            //GUIDないアイテムの場合
            }
            soaiLis.Add(soai);
        }
        string json = JsonUtility.ToJson(new Serialization<SerializationOneAbsItemClass>(soaiLis));
        Debug.Log("SerializationOneAbsItemClass Json:" + json);
        return json;
    }


    //MasterからのSell成功返答Raise.Masterからは売値を加算した合計Seed値が送られてくる.
    public void SuccessItemSell(int addValue)
    {
        ownSeed.SeedIn(addValue);                       //UserのSeed更新.
        ui.UpdateSeedText(ownSeed.ReadOwnSeed());       //UIの更新.
    }

    //User用SelectPanelに生成されるNode用.ItemNodeAddPriceプレファブの売値表示用の売値計算.単価を返す.
    public int CalcOnePrice(byte sellOaiAbs)
    {
        // int sellPrice = 0;
        return itemDefinition.itemDefinitionList.First(s => s.absoluteNumber == sellOaiAbs).value;
    }

    /// <summary>
    /// SelectPanel上にあるD&DしたItemの総額表示計算用.
    /// 購買と売却で処理を別ける.
    /// </summary>
    /// <param name="selectedItems"></param>
    /// <returns></returns>
    public int CalcTotalPrice(List<ItemNodeProp> selectedItems)
    {
        int totalValue = 0;
        switch (BorS)
        {
            case BorSSelect.buy:
                totalValue = CalcSelectedTotalBuyItemValue(selectedItems);
                break;
            case BorSSelect.sell:
                foreach (var p in selectedItems)
                {
                    totalValue += CalcSellPrice(p.oneAbs, shopDef);        //1Absづつ算出して足してゆく.
                }
                break;
            default:
                break;
        }
        return totalValue;
    }

    //購入予定アイテムの総額を計算.
    private int CalcSelectedTotalBuyItemValue(List<ItemNodeProp> selectedItem)
    {
        // return selectedItem.Sum(x => x.mc.price * x.oneAbs.iCt);
        return selectedItem.Sum(x => x.oneAbs.definition.value * StaticMyClass.CPUShopBuyCoefficient * x.oneAbs.iCt);
    }

}

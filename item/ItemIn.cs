using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Photon.Pun;


/// <summary>
/// Player.
/// Item取得.
/// </summary>
public class ItemIn : MonoBehaviour, IItemIn
{
    [NonSerialized]
    public IMasterAccessUserOwndataDB mauo;            //callback.
    private PhotonView view;
    private IUserOwnItemTable userOwnItemTable;
    private IItemOut itemOut;

    //Instantiate.
    [NonSerialized]
    public IAllocationItemStatus allocationItemStatus = null;
    private INotificationToUser notificationToUser;
    [NonSerialized]
    public IMiniNotification miniNotification;     //Ins.
    [NonSerialized]
    public ILoadItemDefinitionByAbs definitionByAbs;
    /// <summary>
    /// Masterへシナリオ取得アイテム作成依頼をした時のみTrue.
    /// Utageでの表示制御のため
    /// </summary>
    private bool scenarioItemCreateRequestFlag;

    /// <summary>
    /// User用.
    /// User購入予定アイテムのキュー.
    /// 購入ボタンOnで追加され,MasterからのRaiseでキューから使用&削除される.
    /// </summary>
    private Queue<List<OneAbsItemClass>> buyItemListQueue = new Queue<List<OneAbsItemClass>>();


    void Start()
    {
        GameObject single2 = GameObject.FindWithTag("single2");
        userOwnItemTable = single2.GetComponent<IMyPlayerDataRef>().MyItemTable;
        notificationToUser = GetComponent<INotificationToUser>();
        itemOut = GetComponent<IItemOut>();
    }

    /// <summary>
    /// 最大所持可能Item数(Abs数)を返す.
    /// </summary>
    /// <returns></returns>
    public int CanOwnItemMaxCount()
    {
        return StaticMyClass.maxItemOwnCount;
    }

    /// <summary>
    /// Playerが持てる全アイテムの個数Over確認.
    /// OverしたらTrue.
    /// </summary>
    /// <returns>取得予定値と現在の所持数の合計が最大所持可能数を超えたらTrue.超えなければfalseを返す.</returns>
    public bool IsTotalItemCountOver()
    {
        return userOwnItemTable.GetUserOwnItemTable().Count >= StaticMyClass.maxItemOwnCount ? true : false;
    }

    /// <summary>
    /// 対象のAbsのアイテムを既に所持しているかの確認.
    /// </summary>
    /// <param name="abs"></param>
    /// <returns>既に所持ならTrue.</returns>
    public bool haveOwnItem(int abs)
    {
        return userOwnItemTable.GetUserOwnItemTable().ContainsKey(abs);
    }


    /// <summary>
    /// Userがアイテムをあといくつ所持できるかを返す.
    /// </summary>
    /// <returns>最大所持可能数 - 現在の所持数(Abs数)</returns>
    public int CheckUserOwnItemCapacity()
    {
        var readOnlyOwnTable = userOwnItemTable.GetUserOwnItemTable();
        return StaticMyClass.maxItemOwnCount - readOnlyOwnTable.Count;
    }



    /// <summary>
    /// Item取得権発生時の1Absの所持数OverFlowCheck.
    /// 取得予定値と現在の所持数の合計が最大所持可能数を超えないならfalseを返す.
    /// </summary>
    /// <param name="itemAbsoluteNum"></param>
    /// <param name="addCount"></param>
    /// <returns>OverしたらTrue.</returns>
    public bool IsItemOverFlow(int itemAbsoluteNum, int addCount)
    {
        Dictionary<int, OneAbsItemClass> readOnlyOwnTable = userOwnItemTable.GetUserOwnItemTable();
        if (!readOnlyOwnTable.ContainsKey(itemAbsoluteNum))
        {
            //そもそも持っていない.addCountのみを評価.
            if (StaticMyClass.maxOwnOneAbsItemCount >= addCount) return false;      //&& existFreeSpace
            else return true;
        }
        else
        {
            //すでに所持しているAbsNumのitem.所持している数も評価.
            if (StaticMyClass.maxOwnOneAbsItemCount >= (readOnlyOwnTable[itemAbsoluteNum].iCt + addCount)) return false;
            else return true;
        }
    }

    /// <summary>
    /// 指定したアイテムが"取得できるか"の確認.
    /// </summary>
    /// <returns>取得できるならTrue.</returns>
    public bool ExistFreeSpace(int abs, int addCount)
    {
        Dictionary<int, OneAbsItemClass> readOnlyOwnTable = userOwnItemTable.GetUserOwnItemTable();
        if (!readOnlyOwnTable.ContainsKey(abs))
        {
            //そもそも持っていない.addCountを評価かつイベントリに1以上の秋
            if ((StaticMyClass.maxOwnOneAbsItemCount >= addCount) && CheckUserOwnItemCapacity() >= 1) return true;
            else return false;
        }
        else
        {
            //すでに所持しているAbsNumのitem.所持している数を評価.
            if (StaticMyClass.maxOwnOneAbsItemCount >= (readOnlyOwnTable[abs].iCt + addCount)) return true;
            else return false;
        }
    }


    /// <summary>
    /// UserOnly.
    /// 「レアドロ」&「合成」&「シナリオ取得アイテム」でのアイテム取得で呼ばれる.
    /// </summary>
    /// <param name="json"></param>
    public void GetItemForUser(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        //もし例外発生なら上位で捕捉してしゅうりょう.
        Dictionary<int, OneAbsItemClass> itemTable = JsonUtility.FromJson<Serialization<int, OneAbsItemClass>>(json).ToDictionary();

        allocationItemStatus.AllocationItemInItemforUser(itemTable);    //user側定義割当て.
        userOwnItemTable.UpdateItem(itemTable);

        //そのままitemTableから表示用に取り出すと同Absは纏められてしまうので個数も加味.同じアイテムが連続表示されないようランダム混ぜる.
        /*if (!scenarioItemCreateRequestFlag) */
        notificationToUser.NoticeGetItem(CreateShowItemName(itemTable));

    }


    /// <summary>
    /// List<ItemNodeProp> の形でまとめた購入予定アイテムをList<OneAbsItemClass>の形に変換してキューに追加.
    /// </summary>
    /// <param name="buyItemNodes">同Absは纏められている</param>
    public void AddBuyItemQueue(List<ItemNodeProp> buyItemNodes)
    {
        List<OneAbsItemClass> oaiLis = buyItemNodes.Select(x => x.oneAbs).ToList();
        buyItemListQueue.Enqueue(oaiLis);
    }

    /// <summary>
    /// MasterからのItemShop購入成功Raiseの受信後.
    /// キューの先頭の要素をUserOwnItemTableに追加する.
    /// </summary>
    /// <param name="hashJson">hash付Itemを購入した場合Masterが生成したhash.生成Hashがなければnull</param>
    public void SuccessedRise(object hashJson)
    {
        string hashjson = hashJson as string;       //キャストできないときの処理.asによるキャスト: nullを返します
        if (string.IsNullOrEmpty(hashjson))
        {
            userOwnItemTable.UpdateItem(DequeueAndChangeItemListToTable()); //GUID付を買っていない場合はGUID割当て処理なしでUpdate.
        }
        else
        {
            List<AbsAndGUIDClass> guidLis = DeserializeItemBuySuccessRaise(hashjson);
            Dictionary<int, OneAbsItemClass> addTable = DequeueAndChangeItemListToTable();  //Queueから取り出して変換.
            allocationItemStatus.AllocationUserBuyShopItems(addTable, guidLis);     //Queueから取り出して変換したDictionary<int,OneAbsItemClass>にデシリアライズしたものから対応するAbsのGUIDを割当て.
            userOwnItemTable.UpdateItem(addTable);      //GUIDを割当てたDictionaryをuserOwnItemTableに追加.
        }
    }

    //Json→List<AbsAndGUIDClass>へのデシリアライズ.
    private List<AbsAndGUIDClass> DeserializeItemBuySuccessRaise(string json)
    {
        return JsonUtility.FromJson<Serialization<AbsAndGUIDClass>>(json).ToList();
    }

    //キューの先頭の要素(List<OneAbsItemClass>)をDictionary<int,OneAbsItemClass>に変換する.
    private Dictionary<int, OneAbsItemClass> DequeueAndChangeItemListToTable()
    {
        List<OneAbsItemClass> dequeue = buyItemListQueue.Dequeue();    //キューから取り出す.取り出されたオブジェクトは、キューから無くなります。
        //同Absは購入前のD&D時にまとまっているはず.
        Dictionary<int, OneAbsItemClass> addMap = dequeue.ToDictionary(x => x.iAbs);

        return addMap;
    }

    /// <summary>
    /// イベントリ容量確認.
    /// </summary>
    /// <returns></returns>
    public bool CanBilling()
    {
        if (CheckUserOwnItemCapacity() < StaticMyClass.billingGachaCount)
        {
            //空き無
            notificationToUser.ShowMessage(StaticMyClass.itemmCountOverMessage2);
            return false;
        }
        else
        {
            //抽選に進む.
            return true;
        }
    }


    /// <summary>
    /// Itemの取得.UserOnly.Hash付き不可.
    /// </summary>
    /// <param name="abs"></param>
    public void GetItemForUser(int abs, int addCount)
    {
        Dictionary<int, OneAbsItemClass> itemTable = new Dictionary<int, OneAbsItemClass>();
        itemTable[abs] = new OneAbsItemClass()
        {
            iAbs = abs,
            iCt = addCount,
            oil = null,
        };

        allocationItemStatus.AllocationItemInItemforUser(itemTable);    //user側定義割当て.
        userOwnItemTable.UpdateItem(itemTable);

        //そのままitemTableから表示用に取り出すと同Absは纏められてしまうので個数も加味.同じアイテムが連続表示されないようランダム混ぜる.
        /* if (!scenarioItemCreateRequestFlag)*/
        notificationToUser.NoticeGetItem(CreateShowItemName(itemTable));

    }



    private string SerializeItemdata(Dictionary<int, OneAbsItemClass> itemTable)
    {
        //Dictionaryをシリアライズ.
        Serialization<int, OneAbsItemClass> seri = new Serialization<int, OneAbsItemClass>(itemTable);
        return JsonUtility.ToJson(seri);
    }


    //DB更新用Dictionary<AbsNum,OneAbsItemClass>(itemCountが加減値を加味したTotal)を作成.引数にはUnity用Dictionary.
    private Dictionary<int, OneAbsItemClass> CreateDBTable(IUserOwnItemTable userOwnItemTable, Dictionary<int, OneAbsItemClass> uniDic)
    {
        Dictionary<int, OneAbsItemClass> userItemTable = userOwnItemTable.GetUserOwnItemTable();
        //Count加算後の返却用Table.自前のディープコピー.
        Dictionary<int, OneAbsItemClass> addedItemMap = new Dictionary<int, OneAbsItemClass>();
        foreach (KeyValuePair<int, OneAbsItemClass> kv in uniDic)
        {
            OneAbsItemClass oai = new OneAbsItemClass();
            addedItemMap[kv.Key] = oai;
            oai.iAbs = kv.Key;
            oai.iCt = kv.Value.iCt;
            if (kv.Value.oil != null)
            {
                List<OneItemClass> oiLis = new List<OneItemClass>();
                foreach (var p in kv.Value.oil)
                {
                    var OneItemClass = new OneItemClass(p.GUID);

                    //項目増えたらSwich?.
                    if (kv.Value.definition.itemType == ItemDefinition.ItemType.Equipment)
                    {
                        OneItemClass.eDefinition = p.eDefinition;
                        OneItemClass.eStatus = p.eStatus;
                    }
                    oiLis.Add(OneItemClass);
                }
                oai.oil = oiLis;
            }
        }


        foreach (KeyValuePair<int, OneAbsItemClass> kv in uniDic)
        {
            if (userItemTable.ContainsKey(kv.Key))
            {
                addedItemMap[kv.Key].iCt += userItemTable[kv.Key].iCt;

            }
        }

        return addedItemMap;          // 指定したItemが存在しない場合(newItem?)はそのまま.
    }


    /// <summary>
    /// レア表示用に加工する.
    /// </summary>
    /// <param name="itemTable">取得したレアItemの名前.</param>
    /// <returns></returns>
    private IReadOnlyList<string> CreateShowItemName(Dictionary<int, OneAbsItemClass> itemTable)
    {
        //複数個は纏められているので個数分作成.
        List<string> result = new List<string>();
        foreach (var p in itemTable)
        {
            for (int i = 0; i < p.Value.iCt; i++)
            {
                result.Add(p.Value.definition.itemName);
            }
        }

        //混ぜる
        return result.OrderBy(i => Guid.NewGuid()).ToList();
    }


    /// <summary>
    /// User.FIeldItemの生成＆取得.
    /// </summary>
    /// <param name="abs"></param>
    private void GetFieldItem(int abs)
    {
        OneAbsItemClass oai = new OneAbsItemClass()
        {
            iAbs = abs,
            iCt = 1,
        };
        userOwnItemTable.UpdateItem(oai);
    }


    /// <summary>
    /// FieldItem取得のために必要なItemを持っているかの確認.
    /// </summary>
    /// <param name="requiredItem"></param>
    /// <returns>持っているならTrue.</returns>
    private bool ExistRequiredItem(int requiredItem)
    {
        if (requiredItem == 0) return true;     //必要なItemがない場合は0のはず.
        return userOwnItemTable.GetOneAbs(requiredItem) != null;
    }

    /// <summary>
    /// FieldItem取得したことをMasterに報告.==>master関与なし.全てHashは無しのため                =>FI休止中.ShootRayで止めてる
    /// </summary>
    public void FieldItemTapped(bool isWait, int locationNum, int abs, int requiredItem)
    {
        //  Debug.Log("Send to Master by Field Item Prop.  LocationNum:" + locationNum + "  Abs:" + abs);
        if (isWait) return;     //再生待ちの場合は何もしない.取得不能.

        //Item取得可能か確認.空き容量確認.
        if (!ExistFreeSpace(abs, 1))
        {
            //取得不能の表示.
            miniNotification.ShowMiniNotice(StaticMyClass.itemmCountOverMessage2);
            return;
        }
        else if (!ExistRequiredItem(requiredItem))
        {
            //FieldItem取得のために必要なItemを持っていない場合.
            miniNotification.ShowMiniNotice(definitionByAbs.GetItemDefinition(requiredItem).itemName + StaticMyClass.requiredItemDoNotHave);        //「～が必要」表示
        }
        else
        {
            //生成.FieldItemはすべてHash無し.
            GetFieldItem(abs);
            //MasterへRPC.
            view.RPC(nameof(ReceiveGetFieldItem), RpcTarget.MasterClient, locationNum, abs);
            //取得した旨の表示.
            miniNotification.ShowMiniNotice(definitionByAbs.GetItemDefinition(abs).itemName);
            //RequiredItemがある場合はそれが壊れるか否かの判定.
            var oai = userOwnItemTable.GetOneAbs(requiredItem);
            if (oai != null /*&& destroyRequired.CheckDestroy(oai)*/)
            {
                itemOut.Delete(oai);
                miniNotification.ShowMiniNotice(definitionByAbs.GetItemDefinition(requiredItem).itemName + StaticMyClass.requiredItemDestroyMessage);//「～が壊れました」表示
            }
        }

    }


}

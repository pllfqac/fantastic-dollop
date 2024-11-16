using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;
using System.Linq;
using Photon.Realtime;
using System;


/// <summary>
/// single2.
/// 合成のUser側.
/// </summary>
public class CompositionbyUser : MonoBehaviour
{
    public IMyPlayerDataRef myRef;  //Ins.
    [NonSerialized]
    public CompositionUI UI;        //ins 
    private RaiseEventClass raise;  

 /*   private IItemDelete itemDelete;
    private IOwnSeed seed;      
    private IItemIn itemIn;*/

    /// <summary>
    /// Userが選択した合成の原料.
    /// </summary>
    private Dictionary<int, OneAbsItemClass> tempSelectedItems = new Dictionary<int, OneAbsItemClass>();


    private void Start()
    {
        raise = GetComponent<RaiseEventClass>();
       // itemDelete = GetComponent<IItemDelete>();
       // itemIn = GetComponent<IItemIn>();
    }


    /// <summary>
    /// 合成「決定」ボタンで呼ばれる.
    /// </summary>
    /// <param name="items">Userが選択した合成の原料</param>
    public  void Composition(List<ItemNodeProp> items)
    {
        //確認
        foreach(var p in items)
        {
            Debug.Log("原料 Abs:" + p.oneAbs.iAbs + "  count:" + p.oneAbs.iCt);
        }

        if (items == null || items.Count == 0) return;

        string selectedItemsStr = SerializeTable(ChangeNodeProptoDictionaryforSelectedtems(items));
        //さらにKnowledgeLvも送るためシリアライズする.
        string knowledge = GetComponent<IKnowledge>().CreateSerializeKnowledgeLevel();
        //Userが選択した合成の原料とKnowledgeLvをSimpleClassにまとめてさらにJson.
        string json = JsonUtility.ToJson(new SimpleClass { data1 = selectedItemsStr, data2 = knowledge });
        SelectedItemRaise(json);
    }

    /// <summary>
    /// アイテムリストをMasterへRaise.
    /// </summary>
    /// <param name="sendData">Userが選択した合成の原料とKnowledgeLevelの各文字列をsimpleClassにまとめてjsonした文字列.</param>
    private void SelectedItemRaise(string sendData)
    {
        Debug.Log("<color=blue> シリアライズしてMasterへRaise.Json:" + sendData + "</color>");
        RaiseEventOptions option = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.MasterClient,
        };
        raise.StartRaise((byte)RaiseEventClass.EEventType.CompositionItemSelected, sendData, true, option);
    }

    /// <summary>
    /// List<ItemNodeProp>からDictionary<int, OneAbsItemClass> への変換.
    /// </summary>
    /// <param name="selectedItemLis"></param>
    /// <returns></returns>
    public Dictionary<int, OneAbsItemClass> ChangeNodeProptoDictionaryforSelectedtems(List<ItemNodeProp> selectedItemLis)
    {
        //ItemNodePropをDictionary<int,OneAbs>に変換.
        Debug.Log("List<ItemNodeProp> Count:" + selectedItemLis.Count);
        if(tempSelectedItems.Count!=0) tempSelectedItems.Clear();
        //GUID付が複数ある場合まとめる.itemCountはマイナスにする.
    //    Dictionary<int, OneAbsItemClass> tempSelectedItems = new Dictionary<int, OneAbsItemClass>();      //返却用Table.

        foreach (ItemNodeProp node in selectedItemLis)
        {
            if (tempSelectedItems.ContainsKey(node.GetOneAbsItemClass().iAbs))
            {
                //Tableにすでに同Absが含まれている場合,GUID付ItemであるのでoneItemClassに追加する.
                tempSelectedItems[node.oneAbs.iAbs].oil.Add(node.oneItem);      //OneItemClassのついか  
                tempSelectedItems[node.oneAbs.iAbs].iCt = tempSelectedItems[node.oneAbs.iAbs].oil.Count;      //Count数更新.GUID数=Count.
                Debug.Log("GUID付まとめ中. Abs:" + tempSelectedItems[node.oneAbs.iAbs] + " Count:" + tempSelectedItems[node.oneAbs.iAbs].oil.Count.ToString());
            }
            else
            {
                //selectedItemLis[node.oneAbs.itemAbsoluteNumber] = node.oneAbs; //oneAbsがList<oneItemClass>を持つ場合,対象Node以外のGUIDも持つ可能性があるので不適切.
                Debug.Log("new OneAbs().    Abs:" + node.oneAbs.iAbs);
                OneAbsItemClass aoi = new OneAbsItemClass();
                aoi.iAbs = node.oneAbs.iAbs;
                aoi.iCt = node.oneAbs.iCt;
                aoi.definition = node.oneAbs.definition;
                Debug.Log("item Definition:" + aoi.definition);
                // if(node.oneItem!=null && node.oneItem.GUID!=null)
                if (node.oneAbs.oil != null)    /*node生成時GUID付でなければ明示的にnull入れてる.  条件判断でList<OneItemClass> = null とOneItemClass=null　がごっちゃになってる可能性ある.*/
                {
                    //NodeのGUIDを入れる.
                    aoi.oil = new List<OneItemClass>() { new OneItemClass(node.GetGUID()) { eStatus=node.oneItem.eStatus} };
                    Debug.Log("<size=32>tempに入っているか確認  eStatus.increvalue</size>" + aoi.oil.First().eStatus.increvalue);
                    Debug.Log("NodeのGUIDを入れる.GUID:" + node.GetGUID());
                }
                else
                {
                    Debug.Log("aoi.oneItemList = null");
                    aoi.oil = null;                         //※※※※※※※※※※※ 明示的にnullをいれないとnull扱いにならない!! ※※※※※※※※※※※※※

                }
                tempSelectedItems[aoi.iAbs] = aoi;
            }
        }
    //    tempSelectedItems = itemTable;
        return tempSelectedItems;
    }


    private string SerializeTable(Dictionary<int, OneAbsItemClass> selectedTable)
    {
        /*  1Absづつ.
         * Dictionary<int, OneAbsItemClass>を,OneAbsItemClassを軽量にしたSerialize用Class「SerializationOneAbsItemClass」のListにしてそれをJsonにする.*/
        //List<OneItemClass>のGUIDを,それのみのLIstに変換してSerializationOneAbsItemClassに.
        List<SerializationOneAbsItemClass> soaiLis = new List<SerializationOneAbsItemClass>();
        foreach (var oai in selectedTable.Values)
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
                soai.stLis = null;            //GUIDないアイテムの場合,一応null入れておく.
            }
            soaiLis.Add(soai);
        }
        string json = JsonUtility.ToJson(new Serialization<SerializationOneAbsItemClass>(soaiLis));
        Debug.Log("SerializationOneAbsItemClass Json:" + json);
        // string json = JsonUtility.ToJson(new Serialization<int, OneAbsItemClass>(sellTable));      //売却アイテムTableをSerialize.
        return json;
    }

    //============MasterからのRaise===================

    /// <summary>
    /// 合成不可だったときに呼ばれる.
    /// Itemの消滅はしない.
    /// </summary>
    public void ReceiveCompositionProprietyNG(byte statesInt)
    {
        Debug.Log("ReceiveCompositionProprietyNG   statesInt:" + statesInt);
        StaticMyClass.CompositionResultStatus states = (StaticMyClass.CompositionResultStatus)Enum.ToObject(typeof(StaticMyClass.CompositionResultStatus), statesInt);
        Debug.Log("<size=22><color=Yellow>Can't Composition!</color></size>  " + states);
        UI.UpdateUI(states);
        tempSelectedItems.Clear();
    }

    /// <summary>
    /// 合成失敗だったときに呼ばれる.
    /// (Equip以外の)原料は消滅する.
    /// </summary>
    public void ReceiveCompositionFailure(int cost)
    {
        Debug.Log("<size=22>合成に失敗しました!</size>");
        myRef.OwnSeed.SeedOut(cost);
        UI.UpdateUI(StaticMyClass.CompositionResultStatus.Failure);
        Increment();
        DeleteItems();
    }





    /// <summary>
    /// 合成の成功.
    /// 原料は消滅する.
    /// </summary>
    /// <param name="vs">Object[itemのJson,合成費用(int)]</param>
    public void ReceiveCompositionSuccess(object[] vs, StaticMyClass.CompositionResultStatus SuccessState)
    {
        string newItemJson = (string)vs[0];
        int cost = (int)vs[1];
        Debug.Log("<size=22>合成に成功しました!</size>" + SuccessState);
        myRef.ItemIn.GetItemForUser(newItemJson);
        myRef.OwnSeed.SeedOut(cost);
        UI.UpdateUI(SuccessState);
        Increment();
        DeleteItems();
    }


    /// <summary>
    /// 合成の成功.Success.Otherの2つめのRaise.
    /// 原料は消滅する.
    /// </summary>
    /// <param name="cost">合成費用.</param>
    public void ReceiveCompositionSuccess(int cost,StaticMyClass.CompositionResultStatus SuccessState)
    {
        Debug.Log("<size=22>合成に成功しました!</size>"+SuccessState);
        myRef.OwnSeed.SeedOut(cost);
        UI.UpdateUI(SuccessState);
        DeleteItems();
    }

    /// <summary>
    /// 原料消滅.
    /// </summary>
    private void DeleteItems()
    {
        Debug.Log("DeleteItems()");
        //Equipは除く.
        var equips = tempSelectedItems.Where(x => x.Value.definition.itemType == ItemDefinition.ItemType.Equipment).ToArray();
        foreach(var p in equips)
        {
            Debug.Log("abs:" + p.Key);
            tempSelectedItems.Remove(p.Key);
        }
        myRef.ItemDelete.Delete(tempSelectedItems);
        tempSelectedItems.Clear();

    }

    /// <summary>
    /// Enhancement or Equipが係わる場合のFailure　でのインクリ
    /// </summary>
    private void Increment()
    {
        foreach(var oneAbs in tempSelectedItems.Values)
        {
            if(oneAbs.definition.itemType==ItemDefinition.ItemType.Equipment)
            {
                OneItemClass Equip = oneAbs.oil.First();
                Equip.eStatus.increvalue++;
                if (Equip.eStatus.increvalue > Equip.eStatus.incremaxvalue) Equip.eStatus.increvalue = Equip.eStatus.incremaxvalue;
                Debug.Log("Equip インクリ:"+Equip.eStatus.increvalue);
            }
        }


    }
    //================================================
}

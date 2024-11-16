using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Single2.
/// 合成をするClass.
/// </summary>
public class Composition : MonoBehaviour, IComposition
{
    [SerializeField]
    private CompositionDefineMap defineMap = null;
    [SerializeField]
    private ParameterTable itemDefineMap = null;
    private RaiseEventClass raise;
    private IMasterAllMemberTable mamt;
    private IPlayerTable pTable;
    private IAllocationItemStatus allocation;
    private DBItemSave save;

    //お金足りるか→所持確認→合成の可否判定→成否判定.
    //前者はItemは失われない.後者は失われる.

    public void Start()
    {
        pTable = GetComponent<IPlayerTable>();
        save = GetComponent<DBItemSave>();
        raise = GetComponent<RaiseEventClass>();
        mamt = GetComponent<IMasterAllMemberTable>();
        allocation = GetComponent<IAllocationItemStatus>();

    }



    /// <summary>
    /// 合成の実行.
    /// UserからのRaise.
    /// 引数には原料 Tkey:Abs TValue:個数.
    /// </summary>
    /// <param name="receiveData">Userが選択した合成の原料とKnowledgeLevelの各文字列をsimpleClassにまとめてjsonした文字列.</param>
    /// <returns>合成成功で作られた1つのアイテムが入る.失敗でnull.</returns>
    public async Task CompositionItem(string receiveData, int userId)
    {
        string selectedItemStr = string.Empty;      //Userが選択した合成の原料.TKey:AbsNum.
        string knowledgeLvStr = string.Empty;       //KnowledgeLevelをSinple3Class.data3にまとめてjsonしたもの
        //受信した文字列を分割する.
        try
        {
            SimpleClass s = JsonUtility.FromJson<SimpleClass>(receiveData);
            selectedItemStr = s.data1;
            knowledgeLvStr = s.data2;
        }
        catch (System.Exception)
        {
            Debug.LogWarning("合成の実行をキャンセルしました  UserID:" + userId);
            return;
        }

        Debug.Log("userId:" + userId + "  Composition request raise!   Json:  " + selectedItemStr);
        CompositionDefine recipe = null;
        StaticMyClass.CompositionResultStatus state;
        Dictionary<int, OneAbsItemClass> selectedItem = null;

        try
        {
            selectedItem = DeserializeSelectedItem(selectedItemStr, userId);
        }
        catch (MyUniException ex)
        {
            Debug.Log("Composition Deserialize Error!!  ID:" + userId);
            Debug.Log(ex);
        }

        //所持確認
        if (!IsOwnItems(selectedItem, userId))
        {
            Debug.Log("User Own Item Check Error! UserId:" + userId);
            raise.UserKickRaise(userId);
            return;
        }

        //可否判定
        if (!CheckPropriety(selectedItem, out recipe, userId, out state))
        {
            Debug.Log("Can't Composition!!  UserId:" + userId + " State:" + state);
            raise.StartRaise((byte)(RaiseEventClass.EEventType.CompositionProprietyNG), (byte)state, true, raise.DefOption(userId));
            return;
        }

        //成否判定
        if (!JudgmentComposition(recipe))
        {
            Debug.Log("Composition Failure!!    UserId:" + userId);
            //合成用Item消滅.
            raise.StartRaise((byte)(RaiseEventClass.EEventType.CompositionFailure), recipe.compositionCost, true, raise.DefOption(userId));
            //Equipの場合合成失敗でも合成回数増加
            if (itemDefineMap.GetItemDefinition(recipe.ResultItemAbs).itemType == ItemDefinition.ItemType.Equipment)
            {
                IncreaseCompoValue(selectedItem.Values.Where(x => x.definition.itemType == ItemDefinition.ItemType.Equipment).First().oil.First());//1つのみのはずなのでFirstでok
                //DBSave.
                OneAbsItemClass oai = selectedItem.Values.First(x => x.definition.itemType == ItemDefinition.ItemType.Equipment);
                await save.UpdateEquipmentComposition(userId, oai, StaticMyClass.CompositionResultStatus.Failure, recipe.compositionCost);

            }
            DestroyMaterial(selectedItem, userId);
            return;
        }

        //合成開始.
        await CompositionMain(recipe, selectedItem, userId, knowledgeLvStr);
    }

    /// <summary>
    /// UserからのraiseをDictionaryに変換する.
    /// </summary>
    /// <param name="json"></param>
    /// <returns>Abs,iCt,(あれば)GUIDしか含まれていないので注意.</returns>
    private Dictionary<int, OneAbsItemClass> DeserializeSelectedItem(string json, int userId)
    {
        Dictionary<int, OneAbsItemClass> result = new Dictionary<int, OneAbsItemClass>();
        List<SerializationOneAbsItemClass> soa = null;
        try
        {
            soa = JsonUtility.FromJson<Serialization<SerializationOneAbsItemClass>>(json).ToList();
        }
        catch
        {
            throw new MyUniException(StaticError.ErrorType.CompositionDeseriError);
        }

        foreach (var p in soa)
        {
            Debug.Log("Abs:" + p.i + " Count:" + p.i2);
            OneAbsItemClass oai = new OneAbsItemClass();
            oai.iAbs = p.i;
            oai.iCt = p.i2;
            oai.definition = itemDefineMap.GetItemDefinition(p.i);
            if (p.stLis != null && p.stLis.Count != 0)
            {
                oai.oil = p.stLis.Select(x => new OneItemClass(x)).ToList();
                Debug.Log("oai.oil.count:   " + oai.oil.Count);
                foreach (var oic in oai.oil)
                {
                    //EquipはeStatusも
                    AllocationRankOneItem(oic, userId);
                    Debug.Log("eLevel:" + oic.eStatus.level);
                }
            }
            result[p.i] = oai;
        }
        return result;
    }


    /// <summary>
    /// UserからのraiseできたItemのうちRank=1 ItemにeDefine,eStatusを割り当てる
    /// </summary>
    /// <param name="items">Userが選択したItems.</param>
    private void AllocationRankOneItem(OneItemClass selectedItem, int userId)
    {
        var e = mamt.GetUserOwnItemTable(userId).GetOneItemClass(selectedItem.GUID);
        selectedItem.eDefinition = e.eDefinition;
        selectedItem.eStatus = e.eStatus;
        Debug.Log("eStatus allocation.  GUID: " + selectedItem.GUID + "   eLevel: " + selectedItem.eStatus.level);
    }



    /// <summary>
    /// 各チェックで問題がないときの合成.
    /// </summary>
    /// <param name="selectedItemMap"></param>
    /// <param name="userId"></param>
    private async Task CompositionMain(CompositionDefine recipe, Dictionary<int, OneAbsItemClass> selectedItemMap, int userId, string knowledgeLvStr)
    {
        Debug.Log("Start Composition Main!");

        //成功時処理
        OneAbsItemClass resultItem = new OneAbsItemClass();
        resultItem.iAbs = recipe.ResultItemAbs;
        resultItem.iCt = 1;
        resultItem.definition = itemDefineMap.GetItemDefinition(resultItem.iAbs);

        Debug.Log("Composition Create Item Abs:" + resultItem.iAbs + " type:" + resultItem.definition.itemType);
        if (resultItem.definition.itemRank == 1)
        {        //----Hash有りのみ----
            resultItem.oil = new List<OneItemClass>();

            //Equip LevelUp.
            if ((recipe.equipmentLevel > 1) &&
                (selectedItemMap.Where(x => x.Value.definition.itemType == ItemDefinition.ItemType.Equipment).First().Value.oil.First().eStatus.level == (recipe.equipmentLevel - 1)))
            {
                //User抽出したItemの中からequipを取得//Equipは一つしかSelect出来ないのでfirstでおｋ
                OneItemClass selectedOic = selectedItemMap.Where(x => x.Value.definition.itemType == ItemDefinition.ItemType.Equipment).First().Value.oil.First();
                OneItemClass newCreateOic = new OneItemClass(selectedOic.GUID);   //Hashは同じ値を継承.
                newCreateOic.eDefinition = selectedOic.eDefinition;
                newCreateOic.eStatus = new EquipmentStatus();
                resultItem.oil.Add(newCreateOic);
                //Equip Statusの更新               
                newCreateOic.eStatus.level = recipe.equipmentLevel;
                Debug.Log("Equip LevelUp!  =>" + newCreateOic.eStatus.level);
                allocation.AllocationLevelUppedEquipmentStatusforOverTwoLevel(userId, resultItem, selectedOic, knowledgeLvStr);  //継承値＋ Knowledgeボーナス              
                //DB Save
                await save.UpdateEquipmentComposition(userId, resultItem, StaticMyClass.CompositionResultStatus.EquipLevelUpSuccess, recipe.compositionCost);
            }
            else if ((recipe.equipmentLevel == 0) && resultItem.definition.itemType == ItemDefinition.ItemType.Equipment)
            {
                //Enhancement.単純な強化
                OneItemClass selectedOic = selectedItemMap.Where(x => x.Value.definition.itemType == ItemDefinition.ItemType.Equipment).First().Value.oil.First();
                Debug.Log("Enhancement before epwr:" + selectedOic.eStatus.epwr);
                Debug.Log("Enhancement before edex:" + selectedOic.eStatus.edex);
                Debug.Log("recipe.Enhancement pwr:" + recipe.Enhancement.epwr);
                Debug.Log("recipe.Enhancement dex:" + recipe.Enhancement.edex);
                allocation.AllocationSimpleEnhancement(userId, selectedOic, recipe.Enhancement, knowledgeLvStr);     //EnhancementValueで定義した値からランダムでeSt増加＋Knowledgeボーナス
                IncreaseCompoValue(selectedOic);
                Debug.Log("Enhancement End! epwr:" + selectedOic.eStatus.epwr);
                Debug.Log("Enhancement End! edex:" + selectedOic.eStatus.edex);
                resultItem.oil.Add(selectedOic);
                //DB Save
                await save.UpdateEquipmentComposition(userId, resultItem, StaticMyClass.CompositionResultStatus.EquipEnhancementSuccess, recipe.compositionCost);
            }
            /*   else if (recipe.equipmentLevel==1)
               {
                   //Equip新規.
               }*/
            else
            {
                //Equip以外の新規Hash付き.2つUserへraise送る
                OneItemClass newOic = allocation.AllocationItemParametor(resultItem.iAbs);
                resultItem.oil.Add(newOic);
                pTable.FindUserObject(userId).obj.GetComponent<IItemIn>().GetItemforMaster(resultItem);
                //合成成功での原料Item消滅指示raise.
                raise.StartRaise((byte)RaiseEventClass.EEventType.CompositionSuccessbyOther, recipe.compositionCost, true, raise.DefOption(userId));
            }
        }

        DestroyMaterial(selectedItemMap, userId);
    }

    /// <summary>
    /// 合成料金の確認と支払い.
    /// 所持金が足りれば支払う.
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="needSeed">合成に必要なSeed.</param>
    /// <returns>足りればtrue.</returns>
    private bool IsOwnSeed(int playerId, int needSeed)
    {
        IOwnSeed own = mamt.GetUserOwnSeedData(playerId);
        if (own.ReadOwnSeed() > needSeed)
        {
            Debug.Log("Composition Seed Cost pay!  " + needSeed);
            own.SeedOut(needSeed);
            return true;    //Seed足りた
        }
        else return false;
    }

    /// <summary>
    /// 所持確認.Userが選択したItem個数を実際に所持しているかの確認.
    /// 合成可能な原料の十分数(=レシピ定義以上の数)の確認とは別.
    /// </summary>
    /// <param name="selectedItems">Userが選択したItem.</param>
    /// <returns>すべて所持していたらtrue.それ以外はfalse.</returns>
    private bool IsOwnItems(Dictionary<int, OneAbsItemClass> selectedItems, int userId)
    {
        return mamt.GetUserOwnItemTable(userId).ContainItem(selectedItems);
    }

    /// <summary>
    /// 合成の可否判定.
    /// </summary>
    /// <param name="selectedItemMap">Userが選択したアイテム.</param>
    /// <param name="recipe">Userが選択したアイテムに適応した合成レシピ.合成不可ならnull返す.</param>
    /// <returns>Trueで合成可能.Falseで不可.不可でもアイテムは失われない.</returns>
    private bool CheckPropriety(Dictionary<int, OneAbsItemClass> selectedItemMap, out CompositionDefine recipe, int userId, out StaticMyClass.CompositionResultStatus state)
    {
        //     //Tkey:abs,TVelue.iCt:Userが選択したアイテム個数.

        recipe = null;
        //Equipが含まれている場合,EquipItemは一つのみであることを確認.1or0だとTrue.それ以外はfalse.
        int equipCount = 0;
        foreach (int abs in selectedItemMap.Keys)
        {
            if (itemDefineMap.GetItemDefinition(abs).itemType == ItemDefinition.ItemType.Equipment) ++equipCount;
        }
        Debug.Log("Equipment Count:" + equipCount);
        if (equipCount >= 2)
        {
            state = StaticMyClass.CompositionResultStatus.EquipCountOver;
            return false;
        }

        //Equipのみ.合成可能回数は残っているか?
        if ((equipCount == 1) && selectedItemMap.Where(s => s.Value.definition.itemType == ItemDefinition.ItemType.Equipment).
             Any(k => k.Value.oil.First().eStatus.increvalue == k.Value.oil.First().eStatus.incremaxvalue))
        {
            state = StaticMyClass.CompositionResultStatus.EquipIncrementRemainingCountZero;
            return false;
        }

        //選択されたItemが合成定義表にあるか.okならここでレシピ取得してる.
        recipe = defineMap.GetRecipe(selectedItemMap);
        if (recipe == null)
        {
            Debug.Log("recipe==null");
            state = StaticMyClass.CompositionResultStatus.RecipeNull;
            return false;
        }
        //原料はレシピで定義されている数より多いか?
        var res = IsSelectedMaterialCountCheck(selectedItemMap, recipe);
        Debug.Log("User Own Item > Recipe Value ? " + res);
        if (!IsSelectedMaterialCountCheck(selectedItemMap, recipe))
        {
            state = StaticMyClass.CompositionResultStatus.MaterialLack;
            return false;
        }

        if (!IsOwnSeed(userId, recipe.compositionCost))
        {
            state = StaticMyClass.CompositionResultStatus.SeedLack;
            return false;
        }

        //Equipのみ.equipLevelUpOnly.recipeのequipLevelと選択されたEquipのlevelが同じでないか
        if ((equipCount == 1) && recipe.equipmentLevel != 0 && !CanEquipLevelUp(selectedItemMap, recipe))
        {
            Debug.Log("NonLevel");
            state = StaticMyClass.CompositionResultStatus.NonLevel;
            return false;
        }
        state = StaticMyClass.CompositionResultStatus.CompositionPossible;
        return true;
    }

    /// <summary>
    /// 合成の成否判定.一定確率で失敗する.
    /// 定義した値より大きい値の場合「失敗」とみなす. 
    /// </summary>
    /// <param name="recipe"></param>
    /// <returns>合成成功でTrue.失敗でFase.</returns>
    private bool JudgmentComposition(CompositionDefine recipe)                          //合成成功確率上げるItem実装した時ここで追加とか
    {
        return recipe.compositionSuccessProbabilityContant > Random.Range(0, 101);      //定義した値より大きい値の場合「失敗」とみなす. 
    }

    /// <summary>
    /// 合成成功&失敗でのアイテム消去.
    /// </summary>
    /// <param name="selectedItemMap"></param>
    /// <param name="userId"></param>
    private void DestroyMaterial(Dictionary<int, OneAbsItemClass> selectedItemMap, int userId)
    {
        //Equipは除く.
        var itemsToRemove = selectedItemMap.Where(s => s.Value.definition.itemType == ItemDefinition.ItemType.Equipment).ToArray();
        foreach (var p in itemsToRemove)
        {
            selectedItemMap.Remove(p.Key);
        }


        pTable.FindUserObject(userId).obj.GetComponent<IItemDelete>().Delete(selectedItemMap);
    }

    /// <summary>
    /// Userが選択した原料はレシピで定義されている数より多いか確認
    /// </summary>
    /// <returns>okならtrue.足りなければfalse.</returns>
    private bool IsSelectedMaterialCountCheck(Dictionary<int, OneAbsItemClass> selecedItem, CompositionDefine recipe)
    {
        foreach (var p in recipe.materials)
        {
            int requiredValue = p.d2;
            if (selecedItem[p.d].iCt < requiredValue) return false;
        }
        return true;
    }

    /// <summary>
    /// 合成したことによる合成数の増加.
    /// </summary>
    /// <param name="selectedEquip"></param>
    private void IncreaseCompoValue(OneItemClass selectedEquip)
    {
        selectedEquip.eStatus.increvalue++;
    }


    /// <summary>
    /// equipLevelUpOnly.
    /// recipeのequipLevelと選択されたEquipのlevelが同じでないか
    /// </summary>
    /// <param name="selectedItems"></param>
    /// <param name="recipe"></param>
    /// <returns>LevelUp可能ならtrue.</returns>
    private bool CanEquipLevelUp(Dictionary<int, OneAbsItemClass> selectedItems, CompositionDefine recipe)
    {
        OneItemClass selectedEquip = selectedItems.First(x => x.Value.definition.itemType == ItemDefinition.ItemType.Equipment).Value.oil.First();
        Debug.Log("CanEquipLevelUp() guid:" + selectedEquip.GUID);
        if (selectedEquip.eStatus.level == (recipe.equipmentLevel - 1)) return true;
        else return false;
    }


}

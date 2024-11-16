using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Canvas-CPUShopPanel-MerchandiseListPanel-ScrollView-Content.
///CPU shopの売っているアイテムのリストを,ItemNodeのリストとしてItemNode生成・表示する.
///tapでそのItemの情報を表示できる.
/// </summary>
public class CPUShopMerchandiseListScrollCtrl : MonoBehaviour
{

	[SerializeField]
	//	private RectTransform itemNode = null;
	private RectTransform itemNodeAddPrice = null;


	public IEquipmentStatuGenerator eStatusGenerator = null;        //cpuShopで販売している装備のステ表示用.

	private List<GameObject> createdNode = new List<GameObject>();

	[SerializeField]
	private ItemDataUI itemDataUi = null;


	//[SerializeField]
	//private EquipmentDefinitionTable eDefinitionTable = null;
	[SerializeField]
	private ParameterTable itemTable = null;

	private void Start()
	{
		eStatusGenerator = GameObject.FindWithTag("single2").GetComponent<IEquipmentStatuGenerator>();
	}


	public void CreateMerchandiseList(CPUItemShopMerchandise cpuShopMerchanDef, Dictionary<int, ItemDefinition> itemDef)
	{
		//Item　1個づつ
		foreach (var ShopMerchandiseOneItem in cpuShopMerchanDef.shopItemList)
		{
			StartCoroutine(CreateNode(ShopMerchandiseOneItem, itemDef));
		}
	}

	private IEnumerator CreateNode(MerchandiseClass mc, Dictionary<int, ItemDefinition> itemDef)
	{
		yield return null;

		if (!itemDef.Any(a => a.Key == mc.itemAbsNum)) yield break;

		var item = GameObject.Instantiate(itemNodeAddPrice) as RectTransform;
		item.SetParent(transform, false);
		createdNode.Add(item.gameObject);
		item.tag = "ItemNodeforShop";
		item.transform.gameObject.AddComponent<NodeTap>();
		//============================表示用============================
		//Item定義からName,Imageを取得してNodeにつける & Nodeの子のItemCountにValueを付ける.
		//1つのアイテムのAbsNumと同じ番号のItemDefinitionを取得.
		ItemDefinition def = itemDef.First(x => x.Key == mc.itemAbsNum).Value;
		item.transform.Find("ItemNameText").GetComponent<Text>().text = def.itemName;
		item.transform.Find("NodeImage").GetComponent<Image>().sprite = def.sprite;
		item.transform.Find("OnePrice").GetComponent<Text>().text = (def.value * StaticMyClass.CPUShopBuyCoefficient).ToString();   //mc.price.ToString();  CPUShopのBuyはValueの2倍
		item.transform.Find("ItemCount").gameObject.SetActive(false);       //ここでは使用しない
		item.transform.Find("StaticText").gameObject.SetActive(false);      //ここでは使用しない
		mc.itemDef = def;   //使うか未定.
							//========================Nodeにデータを持たせる========================
							//MerchandiseClassをそのままNodeに持たせてフローで使いたいがMonoBehaviourを継承していないとAddComponentできないのでMonoBehaviourの参照を持たせる.
		ItemNodeProp nodePr = item.GetComponent<ItemNodeProp>();
		nodePr.oneAbs.definition = def;
		nodePr.mc = mc;
		nodePr.itemDataUi = this.itemDataUi;
		item.GetComponent<DragMoveObject>().Init(nodePr);        //Tap出来るようにする.

		//Equipmentかどうか確認して装備ならstatusを取得.
		if (def.itemType == ItemDefinition.ItemType.Equipment)
		{
			nodePr.oneItem.eStatus = eStatusGenerator.GetbaseEquipmentStatus(mc.itemAbsNum);
			//nodePr.oneItem.eDefinition = eDefinitionTable.eDefinitionTable.First(x => x.absoluteNumber == mc.itemAbsNum);
			nodePr.oneItem.eDefinition = itemTable.eDefinitionTable.FirstOrDefault(x => x.absoluteNumber == mc.itemAbsNum);
			//確認用
			if (nodePr.oneItem.eDefinition == null) Debug.LogError("ShopError");   //Editor角煮尿.Editorフリーズするので
			EquipmentDefinition ed = nodePr.oneItem.eDefinition;
		}
	}

	public void DeleteShopDisplayNodes()
	{
		if (createdNode.Count == 0) return;
		foreach (var p in createdNode.ToArray())
		{
			Destroy(p);
		}

	}
}

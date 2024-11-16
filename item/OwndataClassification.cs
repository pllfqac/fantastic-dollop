using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using System.Linq;
using System.Runtime.InteropServices;


public delegate void CreateLoginSkillInfoDel(byte[] sInfoTable);

/// <summary>
/// Player.
/// MasterからのItem,Seed関連Raise後の処理フロー分類.
/// Login時のみ.
/// </summary>
public class OwndataClassification : MonoBehaviour, IOwndataClassification
{

	public event CreateLoginSkillInfoDel LoginCreateEvent;

	private IUserOwnItemTable userOwnItemTable;
	private IOwnSeed ownSeed;
	private CreateSkillList csl;
	//User Only.GUIDをKeyにitemNodePropを取得する.
	[NonSerialized]
	public ISelectable selectableItemNode;


	public void Init(IUserOwnItemTable itemTable, IOwnSeed ownSeed, CreateSkillList createSkillList)
	{
		this.userOwnItemTable = itemTable;
		this.ownSeed = ownSeed;
		csl = createSkillList;
	}

	/// <summary>
	/// Login時に暗号化ShaMDNをMasterに送ってMasterがServerから取得してきたseed,Item,Equipment,skillLevel情報のセット.
	/// SimpleClassのJson.
	/// SimpleClass.data1 =SimpleClass2をjsonにして暗号化したもの.
	/// SimpleClass.data2=IV.
	/// </summary>
	public void GetLoginOwndata(string json)
	{
		//User側でこのデータが取れないことは「対処不可」の例外なので終了させる.ロギングは不要.
		if (string.IsNullOrEmpty(json)) throw new MyUniException();

		SimpleClass sc = JsonUtility.FromJson<SimpleClass>(json);
		string hukugou = DecryJson(sc.data1, sc.data2);

		//さらにSimpleClass2でJsonされてる
		SimpleClass2 s2 = JsonUtility.FromJson<SimpleClass2>(hukugou);
		string masterOwnClassJson = s2.data1;
		int[] sLevelArry = s2.data3;

		//Skill Levelの割り当て
		ClassifySkillLevel(sLevelArry);
		ReadContent(masterOwnClassJson);
	}

	//復号.
	private string DecryJson(string encryjson, string iv)
	{
		//なんかtagついてたので利用.呼出し多いなら参照もつ.
		Result_AES result = GameObject.FindGameObjectWithTag("single1").GetComponent<_AES2>().Decrypt(encryjson, iv, PhotonNetwork.LocalPlayer.ActorNumber.ToString());
		return result.hukugou_data;
	}

	//MasterOwnClassのJsonの処理.
	private void ReadContent(string masterOwnClassJson)
	{
		MasterOwnClass moc = JsonUtility.FromJson<MasterOwnClass>(masterOwnClassJson);
		ownSeed.SetOwnSeed(moc.seed);                              //もしnullだったら例外が発生. 
		userOwnItemTable.SetOwnItemTable(moc.iD);
		StartCoroutine(GetComponent<IEquipCtrl>().LoginEquip(moc.eP, null, selectableItemNode));      //Masterと同メソッドを使用.Userは第二引数はnull.        
	}

	/// <summary>
	/// Masterから送られてきたSkillLevel配列をUserに割り当て.
	/// </summary>
	/// <param name="skillLevelArr"></param>
	private void ClassifySkillLevel(int[] skillLevelArr)
	{
		var mySkillTable = skillLevelArr.Select((n, index) => new { index, n }).ToDictionary(n => n.index + 1, n => n.n);

		byte[] bytes = skillLevelArr.Select(x => (byte)x).ToArray();    //int[] => byte[] 
		LoginCreateEvent(bytes);
		csl.CreateList(mySkillTable);
	}
}
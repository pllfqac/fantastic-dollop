using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;
using System;
using System.Collections.ObjectModel;


/// <summary>
/// Player.
/// 他端末とのEquipmentの同期.
/// </summary>
public class EquipmentSync : MonoBehaviourPun, IEquipmentSync
{
	//Ins
	[NonSerialized]
	public RaiseEventClass raise = null;
	//PhotonCallback
	[NonSerialized]
	public MyPlayerDataRef myPlayerDataRef = null;      //他Playerが自キャラの参照を持つのはなんか変な気がするが...
	[NonSerialized]
	public ILoadItemDefinitionByAbs loadItem = null;
	[NonSerialized]
	public IGetSceneType getScene = null;   //Ins & photonCallback.

	private PlayerFade fade;
	[NonSerialized]
	public IMyUtageCtrl uta;

	private ICheckEquiped checkEquiped;
	private IAreaCulling areaCulling;
	private CharaAnimationSync animationSync;
	private IEquipmentObjectManager equipmentObject;

	private bool isTownSyncSended = false;      //Townに入ったとき一度だけ同期RPC打つための判断bool.Trueで送ったとみなす.

	///Raiseの連続送信防止用カウンタ.
	///1度送信されたらこの回数FadeInしてもRaiseしない.
	public int RaiseWaitCount { private get; set; }

	/// <summary>
	/// 自端末別キャラ用.
	/// 自キャラがシナリオシーンのとき同PT以外は非表示にするために使用.
	/// 自キャラと同PTのときTrue.
	/// </summary>
	[NonSerialized]
	public IExistPartyMember member;

	private void Start()
	{
		fade = GetComponent<PlayerFade>();
		checkEquiped = GetComponent<ICheckEquiped>();
		areaCulling = GetComponent<IAreaCulling>();
		animationSync = GetComponent<CharaAnimationSync>();
		equipmentObject = GetComponent<IEquipmentObjectManager>();
		RaiseWaitCount = StaticMyClass.SendEquipSyncRaiseRate;
	}



	//=================================RPCでの同期=================================

	/// <summary>
	/// 装備の変更があったとき他端末との同期.
	/// ItemAbs送ってる.
	/// 同じAreaのみ.
	/// </summary>
	public void EquipSyncByRPC(byte equipItemAbs)
	{
		photonView.Group = areaCulling.nowGroupId;
		photonView.RPC("ReceiveEquipSyncbyRPC", RpcTarget.Others, equipItemAbs);
	}


	/// <summary>
	/// 装備したときの同期用RPCの受信.
	/// 自端末別キャラ.
	/// </summary>
	[PunRPC]
	private void ReceiveEquipSyncbyRPC(byte changeEquipItemAbs)
	{
		Debug.Log("装備の同期用RPCの受信!    item Abs " + changeEquipItemAbs);
		Equip(changeEquipItemAbs);
	}


	/// <summary>
	/// 装備を外した時の他端末との同期.
	/// </summary>
	public void UnEquipSyncByRPC(byte unequipItemAbs)
	{
		photonView.Group = areaCulling.nowGroupId;
		photonView.RPC("ReceiveUnEquipSync", RpcTarget.Others, unequipItemAbs);
	}

	/// <summary>
	/// 自端末別キャラ.
	/// </summary>
	/// <param name="unequipItemAbs"></param>
	[PunRPC]
	private void ReceiveUnEquipSync(byte unequipItemAbs)
	{
		UnEquip(unequipItemAbs);
	}


	/// <summary>
	/// Area侵入時の他端末との装備同期.同じAreaのみ.
	/// 装備していなければ0を送るので注意.
	/// byte[]の順は固定.
	/// </summary>
	public void EnterAreaEquipmentSyncByRPC()
	{
		if (getScene.GetSceneType() == SceneDefineTable.SceneType.town && isTownSyncSended) return;

		if (getScene.GetSceneType() == SceneDefineTable.SceneType.town) isTownSyncSended = true;    //Townのとき一度のみ送る.通信量削減のため
		else isTownSyncSended = false;

		ReadOnlyDictionary<CharacterEquipmentPlace.EquipPlaceType, byte> equippingMap = checkEquiped.GetEquippingItemAbsTable();
		byte headEquipItemAbs = equippingMap[CharacterEquipmentPlace.EquipPlaceType.head];      //他の装備も送りたければ増やす
		byte arm1EquipItemAbs = equippingMap[CharacterEquipmentPlace.EquipPlaceType.arm1];
		byte arm2EquipItemAbs = equippingMap[CharacterEquipmentPlace.EquipPlaceType.arm2];
		photonView.Group = areaCulling.nowGroupId;
		photonView.RPC(nameof(ReceiveAreaEnterEquipmentSync), RpcTarget.Others, new byte[] { headEquipItemAbs, arm1EquipItemAbs, arm2EquipItemAbs });
	}


	//RPC受信時はActiveSelf=false.

	///上記装備同期RPCの受信.byte[]の順は固定.
	/// arm2→arm1の順で処理すること.
	[PunRPC]
	private void ReceiveAreaEnterEquipmentSync(byte[] equipAbsArr)
	{
		if (equipAbsArr.Length != 3) return;
		if (!this.gameObject.activeSelf && !uta.IsPlaying && member.CheckJoinedParty(photonView.Owner.ActorNumber)) fade.FadeIn();      //ここでActiveに

		if (uta.IsPlaying)
		{
			this.gameObject.SetActive(true);        //シナリオ中ならSkinnedMeshのみ非表示にする.
			fade.QuickOut();
		}
		if (this.gameObject.activeSelf) StartCoroutine(CommonReceiveEquipSync(equipAbsArr));
		myPlayerDataRef.EquipmentSync.ResponseEquipSync(photonView.Owner.ActorNumber);       //RPCしてきた端末(上の自キャラ)に向けてRaiseする.ここでPlayerオブジェが切り替わる.
	}

	/// <summary>
	/// RPCの後に呼ばれる.
	/// RPCしてきた端末(上の自キャラ)に向けてRaiseする.
	/// </summary>
	/// <param name="viewOwnerId">送信先Player</param>
	public void ResponseEquipSync(int viewOwnerId)
	{
		ReadOnlyDictionary<CharacterEquipmentPlace.EquipPlaceType, byte> equippingTableByItemAbs = checkEquiped.GetEquippingItemAbsTable();
		byte head = equippingTableByItemAbs[CharacterEquipmentPlace.EquipPlaceType.head];      //他の装備も送りたければ増やす
		byte arm1 = equippingTableByItemAbs[CharacterEquipmentPlace.EquipPlaceType.arm1];
		byte arm2 = equippingTableByItemAbs[CharacterEquipmentPlace.EquipPlaceType.arm2];
		raise.StartRaise((byte)RaiseEventClass.EEventType.EquipSync, new byte[] { head, arm1, arm2 }, true, raise.DefOption(viewOwnerId));
	}

	/// <summary>
	/// 先にAreaにいたPlayerからの装備同期Raiseの受信.
	/// 負荷分散のためコルーチン.
	/// arm2→arm1の順で処理すること.
	/// </summary>
	public void ReceiveEquipSyncRaise(byte[] equipAbsArr)
	{
		if (!this.gameObject.activeSelf && !uta.IsPlaying && member.CheckJoinedParty(photonView.Owner.ActorNumber)) fade.FadeIn();
		if (uta.IsPlaying)
		{
			this.gameObject.SetActive(true);        //シナリオ中ならSkinnedMeshのみ非表示にする.
			fade.QuickOut();
		}
		//表示対象装備増やした時,配列の順番注意.
		if (this.gameObject.activeSelf) StartCoroutine(CommonReceiveEquipSync(equipAbsArr));    //シナリオシーンのときは非Activeとなり得る
	}

	/// <summary>
	/// RPC,Raise受信共通処理.
	/// 負荷分散のためコルーチン.
	/// ※ arm2→arm1の順で処理すること.
	/// </summary>
	/// <param name="itemAbs"></param>
	private IEnumerator CommonReceiveEquipSync(byte[] equipAbsArr)
	{
		yield return null;
		if (equipAbsArr[0] != 0) Equip(equipAbsArr[0]);
		else UnEquip(CharacterEquipmentPlace.EquipPlaceType.head);
		yield return null;

		if (equipAbsArr[2] != 0) Equip(equipAbsArr[2]);
		else UnEquip(CharacterEquipmentPlace.EquipPlaceType.arm2);

		if (equipAbsArr[1] != 0) Equip(equipAbsArr[1]);
		else UnEquip(CharacterEquipmentPlace.EquipPlaceType.arm1);
		yield return null;
	}

	//=================================受信後処理=================================

	//装備した時.
	private void Equip(byte itemAbs)
	{
		EquipmentDefinition equipmentDefinition = loadItem.GetEquipmentItemDefinition(itemAbs);
		if (equipmentDefinition == null) return;
		//装備に対応するanimationの変更.
		if (equipmentDefinition.ePlaceType == CharacterEquipmentPlace.EquipPlaceType.arm1) animationSync.SetSyncAnimationByWeaponType(equipmentDefinition.WeaponEquipmentType);
		equipmentObject.EquipObject(itemAbs);   //装備の見た目の変更.
	}

	//装備が外れた時.
	private void UnEquip(byte itemAbs)
	{
		//武器が外れた時
		EquipmentDefinition eDef = loadItem.GetEquipmentItemDefinition(itemAbs);
		if (eDef == null) return;
		if (eDef.ePlaceType == CharacterEquipmentPlace.EquipPlaceType.arm1)
			animationSync.SetSyncAnimationByWeaponType(StaticMyClass.WeaponEquipmentType.Unarmed);//Tawnだとしても良しなにしてくれるのでこれでおｋ.

		equipmentObject.UnEquipObject(eDef);
	}

	//上記メソッドをItemAbsでなくequipPlaceType指定Ver.
	private void UnEquip(CharacterEquipmentPlace.EquipPlaceType equipPlace)
	{
		if (equipPlace == CharacterEquipmentPlace.EquipPlaceType.arm1)
			animationSync.SetSyncAnimationByWeaponType(StaticMyClass.WeaponEquipmentType.Unarmed);//Tawnだとしても良しなにしてくれるのでこれでおｋ.

		equipmentObject.UnEquipObject(equipPlace);
	}


}

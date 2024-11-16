using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Photon.Pun;

/// <summary>
/// Player.
/// Photonインタレストグループの送信側に関するOnOff設定.
/// </summary>
public class AreaCulling : MonoBehaviourPunCallbacks, IAreaCulling
{
	//Privateシーンのコライダとのコリジョンは受信設定しない


	private PlayerFade PlayerFade;
	private IEquipmentSync equipmentSync;


	public byte nowGroupId { get; private set; }
	private byte tempGroupId = 0;       //OTEnterで表示消すRPC後にすぐ引き返した場合、自身のGroupIDが変更されたままになっている問題に対応するため.

	private void Awake()
	{
		PlayerFade = GetComponent<PlayerFade>();
	}

	private void Start()
	{
		equipmentSync = GetComponent<IEquipmentSync>();
	}


	/// <summary>
	/// インタレストグループの受信設定.
	/// </summary>
	/// <param name="groupId"></param>
	/// <param name="enable">受信する場合はTrue.</param>
	public void ReceiveSettings(byte groupId, bool enable)
	{
		PhotonNetwork.SetInterestGroups(groupId, enable);
	}


	//Player->CullingAreaChangeTriggerで検知したOTEで呼ばれる.

	/// <summary>
	/// boxCollider境界とのコリジョン.
	/// 入った時.
	/// </summary>
	/// <param name="other"></param>
	public void CullingAreaOTEnter(Collider other)
	{
		if (photonView.IsMine)
		{
			byte newGroupId = GetGroupNum(other.tag);         //TagからGroupIDの数字を取り出す

			SendMyOTE(nowGroupId);		//今いたAreaに向けてRPC.
			tempGroupId = nowGroupId;       //一時保存.

			Debug.Log("<size=25>OnTriggerEnter  " + other.tag  + "</size>");


			nowGroupId = newGroupId;   //ここで他端末の自キャラと同期切れる.SendMyOTE()してない場合は,他端末の自キャラがFadeOutできない
			ReceiveSettings(newGroupId, true);	
			if(equipmentSync!=null) equipmentSync.EnterAreaEquipmentSyncByRPC();
		}
	}

	/// <summary>
	/// boxCollider境界とのコリジョン.
	/// 離れるとき.
	/// </summary>
	/// <param name="leaveCollider"></param>
	public void CullingAreaOTExit(Collider leaveCollider)
	{
		if (photonView.IsMine)
		{
			byte leaveGroupId = GetGroupNum(leaveCollider.tag);
			Debug.Log("OnTriggerExit  " + leaveCollider.tag + "   ID : " + leaveGroupId);
			if (nowGroupId == leaveGroupId)
			{
				//同じインタレストGroupへ途中で引き返した場合.
				nowGroupId = tempGroupId;
				//そのままでは別端末上の自キャラの同期が止まって表示されたままなので,遷移元に向けてRPC.
				SendMyOTE(leaveGroupId);
			}
			else
			{
				tempGroupId = nowGroupId;
			}

			ReceiveSettings(leaveGroupId, false);
		}
	}

	/// <summary>
	/// AreaColliderオブジェに設定したTagから,"GroupXX"の"Group"以降を数字にして取得.
	/// </summary>
	/// <param name="str">Tag</param>
	/// <returns>取得した文字列</returns>
	private byte GetGroupNum(string tagName)
	{
		//6文字目（インデックスは5）から最後まで取得する
		return byte.Parse(tagName.Substring(5));
	}



	//==========================RPC==========================
	/// <summary>
	/// 自端末自キャラのOTEで他端末自キャラにOTEしたことを伝えるRPCを送る.
	/// </summary>
	/// <param name="sendTargetGroupId">RPCを送る対象のGroupID.</param>
	/// <param name="sendId">Enter,Exit両方で遷移先IDを渡す.</param>
	private void SendMyOTE(byte sendTargetGroupId)
	{
		Debug.Log("===>> RPC送信!   Target ===>> " + sendTargetGroupId);
		photonView.Group = sendTargetGroupId;
		photonView.RPC(nameof(GetMyOTE), RpcTarget.Others);				//MasterだけはMissingエラー出る可能性があるのでRPC送信は区別したい
	}

	//上記メソッドの受信.
	[PunRPC]
	private void GetMyOTE()
	{
		Debug.Log(" GetMyOTE()");
		PlayerFade.FadeOut();
	}


}

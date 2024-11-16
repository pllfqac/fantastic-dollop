using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMyPhotonChatManager 
{
	/// <summary>
	/// 文字送信インターバル中(送信不可)はTrue.
	/// </summary>
	bool IsInterval { get; }

	/// <summary>
	/// PhotonChatの初期設定.
	/// PhtonChatに接続.
	/// </summary>
	/// <param name="userName">User Name.ユニークであること.</param>
	void ConnectPhotonChat(string userName);

	/// <summary>
	/// SceneのPublicMessageの送信.
	/// InputFieldの入力確定で呼ばれる.
	/// </summary>
	/// <param name="message">送りたい文字列.</param>
	void SendSceneMessage(string targetChannel, string message);

	/// <summary>
	/// Party(Club)のPublicMessageの送信.
	/// </summary>
	/// <param name="message"></param>
	void SendPartyMessage(string message);

	/// <summary>
	/// 1:1のPrivateMessageの送信.
	/// </summary>
	/// <param name="targetName">送信対象のキャラ名.</param>
	/// <param name="message">送りたい文字列.</param>
	void SendPrivateMessage(string targetName, string message);

	//Channelの購買・非購買.
	void MySubscribe(string subscribeChannelName);
	void MyUnSubscribe(string unSubscribeChannelName);


	//==========Party============
	/// <summary>
	/// PartyMember変更時に呼ぶ.
	/// PartyChannelの更新.
	/// </summary>
	/// <param name="newPartyMemberIdList"></param>
	void UpdatedPartyChannel(List<int> newPartyMemberIdList);

	/// <summary>
	/// 自身がPartyから脱退した時.
	/// </summary>
	void ExitParty();
}

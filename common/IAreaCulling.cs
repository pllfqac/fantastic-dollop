using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Player用インタレストグループ設定.
/// ネットワークカリングで通信量を削減したい.
/// </summary>
public interface IAreaCulling
{
	byte nowGroupId { get; }

	//=========================インタレストグループの受信設定=========================
	/*プレイヤーはPhotonNetwork.SetInterestGroups()で、受信対象のグループの追加と削除を行うことができます。グループIDはbyte型の値で、1～255の範囲で複数選択できます。
	 * グループID0は、ルームへ参加しているプレイヤー全員が自動的に受信対象として追加されるデフォルトのグループで、設定を変更（削除）することはできません。*/

	//=========================インタレストグループの送信設定=========================
	/*オブジェクト同期やRPCは、送信される（送信データを作成される）時にPhotonView.Groupで指定されているグループIDが送信対象に設定されます。
	 * グループID0を指定すると、ルームへ参加しているプレイヤー全員に送信することになります。
	 * （PhotonView.Groupは、送信時にグループIDを設定するためだけの値で、変更することで通信は発生しません）*/


	//Player->CullingAreaChangeTriggerのコリジョンで呼ばれる.
	void CullingAreaOTEnter(Collider other);
	void CullingAreaOTExit(Collider other);

	/// <summary>
	/// インタレストグループの受信設定.
	/// </summary>
	/// <param name="groupId">設定したいGroupID</param>
	/// <param name="enable">受信する場合はTrue</param>
	void ReceiveSettings(byte groupId, bool enable);
}

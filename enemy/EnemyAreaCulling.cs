using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using Photon.Pun;

/// <summary>
/// Enemy.
/// EnemyはSceneごとにインタレスト設定してる.
/// </summary>
public class EnemyAreaCulling : MonoBehaviour
{
	/*(仮案)
     * Player:Scene Changeでインタレストグループを変更する.インタレストグループ番号はSceneindexと一対一対応させる.
     * つまり　interestGroup=SceneIndex
     * Playerが購読するインタレストグループは,0(デフォ)とSceneIndexの2つとなる.
     * 
     * Enemy:SpawnしたSceneにのみ関係する.Spawn時に,view.groupとインタレストグループの両方をそのSceneindexに設定する.
     */


	private PhotonView _view;       //元Test用PlayerのphotonView
	public PhotonView view
	{
		get
		{
			if (_view == null) _view = GetComponent<PhotonView>();
			return _view;
		}
	}



	/// <summary>
	/// View.groupを変更する.
	/// </summary>
	/// <param name="groupId">変更したいview.groupの値</param>
	public void SendSettings(byte groupId)
	{
		view.Group = groupId;
	}

	

}

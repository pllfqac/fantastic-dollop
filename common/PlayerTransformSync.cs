using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

/// <summary>
/// Player.
/// PhotonTransformViewの代わりの自作Transform同期スク.
/// </summary>
public class PlayerTransformSync : MonoBehaviourPunCallbacks,IPunObservable
{
	/// <summary>
	/// 自キャラのシナリオ中か判断してからFade処理.
	/// </summary>
	[NonSerialized]
	public IMyUtageCtrl uta;

	private IAreaCulling areaCulling;
	private PlayerFade PlayerFade;

	private Vector2 tempV2;
	private Vector3 startPosition;
	private Vector3 endPosition;
	private float elapsedTime = 0f;
	private float rotationEulerY;



    private void Awake()
	{
		PlayerFade = GetComponent<PlayerFade>();        //Startだと間に合わなくてNullRefでるっぽい
	}

	private void Start()
	{
		if (photonView.IsMine)
		{
            //通信量削減のためメッセージの送信頻度を変更している
            //PhotonNetwork.SendRate = StaticMyClass.SendMyRate;		=>デフォ値30を使用
            //OnPhotonSerializeが1秒に何度呼ばれるか.OnPhotonSerializeで書き込まれた更新は一時的にキューに入れられ、次のLateUpdateで送信
            PhotonNetwork.SerializationRate = 10;                   
            areaCulling = GetComponent<IAreaCulling>();
		}
		startPosition = transform.position;
		endPosition = transform.position;
		rotationEulerY = transform.rotation.eulerAngles.y;
	}



	// データを送受信するメソッド
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
            //送信
            //メッセージ数を節約.前回に送信した座標から、一定の距離以上移動した場合のみ && 回転角が閾値以上なら 同期用の値を送信する
            float sm = Vector3.SqrMagnitude(endPosition - transform.position);
			float rotateDiffAbs = Mathf.Abs(this.transform.rotation.eulerAngles.y - rotationEulerY);
			//Debug.Log("SqrMagnitude  " + sm);
			//	Debug.Log("Rotation Diff Abs  " + rotateDiffAbs);
			//空のPhotonStreamは送信されません
			if (sm > 0.1f || rotateDiffAbs > 5)
			{
				//通信量削減のためvector3->vector2で
				tempV2.x = transform.position.x;
				tempV2.y = transform.position.z;

				photonView.Group =areaCulling.nowGroupId;
				stream.SendNext(tempV2);
				stream.SendNext(this.transform.rotation.eulerAngles.y);         //回転角はY軸のみ利用.

				endPosition = transform.position;
				rotationEulerY = this.transform.rotation.eulerAngles.y;
			}
		}
		else
		{
			//受信
			//非ActiveならActiveに
			if (!this.gameObject.activeSelf)
			{
				//		Debug.Log("existScenario.ExistScenarioScene:<color=orange>" + existScenario.ExistScenarioScene 
				//			+ "</color>  CheckJoinedParty:<color=orange>" + member.CheckJoinedParty(photonView.Owner.ActorNumber)+" </color>");
				//Debug.Log(!uta.IsPlaying &&(!existScenario.ExistScenarioScene || (existScenario.ExistScenarioScene && member.CheckJoinedParty(photonView.Owner.ActorNumber))));
				//(同じSceneかつ(これはAreaCullingで判断している))自キャラがScenarioSceneにいる時のみ次の判断→ 同じPTなら表示. 
 				if (!uta.IsPlaying /*&&
					(!existScenario.ExistScenarioScene || (existScenario.ExistScenarioScene && member.CheckJoinedParty(photonView.Owner.ActorNumber)))*/)  PlayerFade.FadeIn();
				Vector3 pos = transform.position;
				this.transform.position = new Vector3(pos.x, pos.y, pos.z);             //pos.yは各オブジェで計算すること.とりまpos.y
            }
			// 受信時の座標を、補間の開始座標にする
			startPosition = transform.position;
			// 受信した座標を、（transfrom.positionへ直接反映させずに）補間の終了座標にする
			tempV2 = (Vector2)stream.ReceiveNext();
			endPosition = new Vector3(tempV2.x, transform.position.y, tempV2.y);
			rotationEulerY = (float)stream.ReceiveNext();
			elapsedTime = 0f;
		}
	}

	//SendNext()とReceiveNext()の数があってないと例外出るっぽい


	private void Update()
	{
		if (!photonView.IsMine)
		{
			//Debug.Log("srm:  " + (endPosition - startPosition).sqrMagnitude);
			// 受信時の座標から受信した座標へ補間移動する.値が離れすぎならワープ.
			if ((endPosition - startPosition).sqrMagnitude > StaticMyClass.warpValue) transform.position = endPosition;
			else
			{
				elapsedTime += Time.deltaTime;
				transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / StaticMyClass.InterpolationDuration);
				Vector3 rotationAngles = this.transform.rotation.eulerAngles;
				rotationAngles.y = rotationEulerY;
				this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.Euler(rotationAngles), StaticMyClass.CharaRotationSpeed * Time.deltaTime);
			}
		}
	}

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

#pragma warning disable 649


/// <summary>
/// Player.
/// 自端末別キャラ用のWalk,RunのAnimatorの切り替え.
/// 通信量削減のためWalk,Runは同期をしないため.
/// </summary>
public class MoveAnimatorCtrl : MonoBehaviour
{
	private Animator animator;
	Vector3 latestPos;


	[SerializeField]
	private float runThresholdValue;		//この値以上のSpeedの場合Runに	

	private void Start()
	{
		if (GetComponent<PhotonView>().IsMine) this.enabled = false; //Destroy(this);
		animator = GetComponent<Animator>();
		latestPos = this.transform.position;
	}



	private void Update()
	{
		float speed = ((transform.position - latestPos) / Time.deltaTime).sqrMagnitude;
		//Debug.Log(speed);
		latestPos = transform.position;

		CharaMoveAnimation(speed);
	}


	//キャラのWalk,RunのAnimation.この二つは通信で同期はしないため.
	private void CharaMoveAnimation(float speed)
	{
		if (speed>0.01f)
		{
			if (speed<runThresholdValue)
			{
				//歩く
				animator.SetBool("is_Walking", true);
				animator.SetBool("is_Running", false);
			}
			else
			{
				//走る
				animator.SetBool("is_Walking", false);
				animator.SetBool("is_Running", true);
			}
		}
		else
		{
			animator.SetBool("is_Running", false);
			animator.SetBool("is_Walking", false);
		}
	}
}

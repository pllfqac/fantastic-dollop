using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

//Enemy.
public class EnemySearchArea : MonoBehaviour {

	/// <summary>
	/// EnemyのSearchAreaの自キャラとのOTEでよばれる.
	/// EnmeyNodeのオンオフ.
	/// </summary>
	public event Action<bool> EnemyNodeSetActiveEvent;

    private EnemyEasyAI enemyAi;     //inspe


	private void Start()
	{
		enemyAi = this.transform.root.GetComponent<EnemyEasyAI>();
	}

	

	
	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("<size=30>Player Search!!</size>");
		//自端末自キャラのみ対象.
		if (PhotonNetwork.IsMasterClient || other.gameObject.GetInstanceID() !=  enemyAi.playerDataRef.myHitAreaGameObjectInstanceID) return;
		if (EnemyNodeSetActiveEvent != null) EnemyNodeSetActiveEvent(true);
	}


	/// <summary>
	/// 自端末他キャラでも一応setAttackTarget呼ぶ.
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerStay(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            enemyAi.SetAttackTarget(other.transform);  //transformはHitAreaのTr
        }
    }
    
    /*上記OnTriggerStay()でのsetAttackTarget()は自端末の自キャラのみ.自端末別キャラへのATはRPCのみで入る.         */


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            enemyAi.DeleteTarget(other);            //自端末の自キャラの場合RPCで同期.頻度が少ないと踏んだので.
													//自端末自キャラのみ対象.
			if (PhotonNetwork.IsMasterClient || other.gameObject.GetInstanceID() != enemyAi.playerDataRef.myHitAreaGameObjectInstanceID) return;
			if (EnemyNodeSetActiveEvent != null) EnemyNodeSetActiveEvent(false);
		}
    }
}

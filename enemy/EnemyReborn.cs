using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



/// <summary>
/// Enmey.
/// Rebornて名前だけど最初の生成でも使う.
/// Enemyの再生成時間と場所を決める.各SceneのEnemyStartPoint数を定義する.
/// </summary>
public class EnemyReborn : MonoBehaviour,IEnemyReborn {

	private PhotonView view;
	private RandomManager rManager;
	private EnemyEasyAI eEasyAi;
	private HitPointCalculation hpc;
	private EnemyData eData;
	private EnemyHP eHp;                //All.
	[System.NonSerialized]
	public IEnemyStartPositionCtrl esp;		//MasterOnly.

	private void Awake()
	{		
		rManager = GetComponent<RandomManager>();
		eEasyAi = GetComponent<EnemyEasyAI>();
		hpc = GetComponent<HitPointCalculation>();
	}

	private void Start()
	{
		view = this.GetComponent<PhotonView>();         //MasterがEnemy生成するときViewはaddで後付けなのでAwakeでは取れない.
		eHp = GetComponent<EnemyHP>();
		eData = GetComponent<EnemyData>();
	}

	//Master用.
	public void ReBornStart()
	{
		Debug.Log(" ReBornStart()");
		StartCoroutine(DeadTime());
	}

	private IEnumerator DeadTime()
	{
		Debug.Log("DeadTime()");
		yield return new WaitForSeconds(StaticMyClass.enemyRebornTime);
		view.RPC("Reborn", RpcTarget.All,esp.GetESP(eData.enemyID, eData._sceneIndex));
	}

	//Enmey再生成.
	[PunRPC]
	public void Reborn(Vector2 rePos)
	{
		//EnemyDownアニメーションのイベントで呼ばれるように変更.

		rManager.SetPosition(rePos, StaticMyClass.addTargetValue, StaticMyClass.randomValueInit);        //TV,RCは各端末上で変更.
		eHp.HP = eData.Status.maxhp;
		eEasyAi.Reborn();

		if (!PhotonNetwork.IsMasterClient)
		{
			hpc.changeEnemyHP_UI(eData.Status.maxhp);
        }
		Debug.Log("Enemy再生成!");
	}



	/// <summary>
	/// Enemyの生成位置の決定.
	/// y→zに変換が必要.
	/// </summary>
	/// <returns></returns>
	public Vector2 GetEspVector2()
    {
		Debug.Log("EnemyID:" + eData.enemyID + "  SceneIndex:" + eData._sceneIndex);
		return esp.GetESP(eData.enemyID, eData._sceneIndex);
    }
}

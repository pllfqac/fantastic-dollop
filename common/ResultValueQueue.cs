using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;

/// <summary>
/// Player,Enemy.
/// 自端末自キャラのSkillを受けたオブジェにのみ対応.
/// Skill使用時,Masterからのダメージ(回復)RPCを,Hitした瞬間に表示するように調整するバッファ的なキュー.
/// 1サイクルのみ考慮.
/// Hit時の受信側.
/// </summary>
public class ResultValueQueue : MonoBehaviour, IReceiveShowTiming, IResultValueEnqueue, IEnemyDelayAction
{
	/// <summary>
	/// このスク付けたオブジェがEnemyの時のみ使用.
	/// 撃破時に呼ぶ.
	/// </summary>
	public event Action enemyDeadAction;

	private IUseValueQueue useValueQueue;       //Damage,Revi.
	private IUseUccQueue useUccQueue;           //Buff,Debuff.

	/// <summary>
	/// SkillUserが自キャラのときはこの参照を使用.
	/// </summary>
	[NonSerialized]
	public DamageUIAnimationCtrl DamageUIAnimationCtrl = null;  //Ins,punCallback.

	/// <summary>
	/// Damage,Revi用のキュー.
	/// </summary>
	private Queue<OneDamageInfoClass> bufferQueue = new Queue<OneDamageInfoClass>();

	/// <summary>
	/// Buff,Debuff用のキュー.
	/// </summary>
	private Queue<UccInfoClass> UccQueue = new Queue<UccInfoClass>();

	/// <summary>
	/// MasterからのRPCが後で来た場合増加.
	/// 1RPCで1増加.
	/// </summary>
	private byte hitCount;
	private byte uccHitCount;
	/// <summary>
	/// MasterからのRPCが後で来た時用.
	/// </summary>
	private StaticMyClass.SendStatus tempSendStatus;

	private void Start()
	{
		useValueQueue = GetComponent<IUseValueQueue>();
		useUccQueue = GetComponent<IUseUccQueue>();
	}

	//=================キューへの追加===================

	/// <summary>
	/// DamageInfoClassできたデータをList(OneDamageInfoClass)に変換してQueueに保存.
	/// </summary>
	/// <param name="damageInfoClassList"></param>
	public void OneDamageInfoClassEnqueue(IReadOnlyList<OneDamageInfoClass> damageInfoClassList)
	{
		//DamageInfoClassをIReadOnlyList<OneDamageInfoClass>に変換してOneDamageInfoClassを順にQueueに追加.
		foreach (OneDamageInfoClass odic in damageInfoClassList)
		{
			bufferQueue.Enqueue(odic);
		}

		if (hitCount > 0)
		{
			List<OneDamageInfoClass> oneDamages = new List<OneDamageInfoClass>();
			while (hitCount > 0)
			{
				if (bufferQueue.Count == 0) break;
				oneDamages.Add(bufferQueue.Dequeue());
				hitCount--;
			}
			DamageUIAnimationCtrl.StartDamageUIAnimation(oneDamages, true);
		}
		Debug.Log("Queue.Count " + bufferQueue.Count);
	}

	/// <summary>
	/// 上記メソッドのクイックVer.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="isCritical"></param>
	public void OneDamageInfoClassEnqueue(int? value, bool isCritical)
	{
		Debug.Log("value:" + value + "  キューへ追加   InstanceID:" + this.gameObject.GetInstanceID());

		bufferQueue.Enqueue(new OneDamageInfoClass(value, isCritical));

		if (hitCount > 0)
		{
			DamageUIAnimationCtrl.StartDamageUIAnimation(bufferQueue.Dequeue(), true);
			hitCount--;
		}
		Debug.Log("Queue.Count " + bufferQueue.Count);
	}




	/// <summary>
	/// UCCの受信でのQueueへの追加.
	/// Uccは表示のみ遅らせて表示する目的でQueue使用.
	/// </summary>
	/// <param name="condition">命中でTrue.</param>
	public void UccEnqueue(UccInfoClass uccInfoClass)
	{
		UccQueue.Enqueue(uccInfoClass);
		Debug.Log("UCC Queueに追加.  Queue Count: " + UccQueue.Count);

		if (uccHitCount > 0)
		{
			useUccQueue.UseUccQueue(UccQueue.Dequeue(), tempSendStatus);
			uccHitCount--;
			Debug.Log("UccQueueデクリ  uccHitCount:" + uccHitCount);
		}
	}

	//=======================受信=======================

	/// <summary>
	/// キューからの放出.
	/// Hitしたタイミングでメッセージシステムから呼ばれる.
	/// </summary>
	/// <param name="oneHitDequeueCount">1Hitしたときのデキュー回数.</param>
	public void OnReceiveHit(byte oneHitDequeueCount, StaticMyClass.SendStatus sendStatus)
	{

		Debug.Log("====>>  1Hitしたときのデキュー回数:" + oneHitDequeueCount);
		//Damage or Revi
		if (bufferQueue.Count > 0)
		{
			if (oneHitDequeueCount == 1)
			{
				OneDamageInfoClass oneDamageInfoClass = bufferQueue.Dequeue();
				useValueQueue.ValueDequeue(oneDamageInfoClass);
				DamageUIAnimationCtrl.StartDamageUIAnimation(oneDamageInfoClass, true);     //UI処理.
			}
			else
			{
				List<OneDamageInfoClass> tempLis = new List<OneDamageInfoClass>();
				//1Hit以上.Listにまとめる.
				for (int i = 0; i < oneHitDequeueCount; i++)
				{
					//Debug.Log(i);
					if (bufferQueue.Count == 0) continue;
					OneDamageInfoClass oneDamageInfoClass = bufferQueue.Dequeue();
					useValueQueue.ValueDequeue(oneDamageInfoClass);
					tempLis.Add(oneDamageInfoClass);
				}
				DamageUIAnimationCtrl.StartDamageUIAnimation(tempLis, true);     //UI処理.
			}
			if (enemyDeadAction != null)
			{
				enemyDeadAction();                 //EnemyOnly.
				Debug.Log("enemyDeadAction != null	");
			}
			Debug.Log("bufferQueueから放出.");
		}
		else if (sendStatus == StaticMyClass.SendStatus.DamageOnly || sendStatus == StaticMyClass.SendStatus.DamagePlusUCC) hitCount += oneHitDequeueCount;

		//UCC
		if (UccQueue.Count > 0)
		{
			useUccQueue.UseUccQueue(UccQueue.Dequeue(), sendStatus);
			if (uccHitCount > 0) uccHitCount--;
		}
		else if (UccQueue.Count == 0 && (sendStatus == StaticMyClass.SendStatus.DamagePlusUCC || sendStatus == StaticMyClass.SendStatus.UccOnly))
		{
			uccHitCount++;
			tempSendStatus = sendStatus;
		}
		Debug.Log("<size=22> Queue.Count:" + bufferQueue.Count + "  HitCount:" + hitCount + "  UCCキューCount:" + UccQueue.Count() + "   UccHitCount:" + uccHitCount + "</size>");
	}




	//=====================Clear======================
	public void ClearBufferQueue()
	{
		Debug.Log("-ClearBufferQueue-");
		bufferQueue.Clear();
		UccQueue.Clear();
		hitCount = 0;
		uccHitCount = 0;
		enemyDeadAction = null;
	}


}

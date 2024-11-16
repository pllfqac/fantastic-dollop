using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// メッセージシステム受信側.
/// HitしたタイミングでHitされた側へ送られる.
/// </summary>
public interface IReceiveShowTiming:IEventSystemHandler
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="oneHitDequeueCount">1Hitでのデキュー回数.</param>
	/// <param name="sendStatus">SkillUserの使用したSkillの情報.</param>
	void OnReceiveHit(byte oneHitDequeueCount,StaticMyClass.SendStatus sendStatus);
}
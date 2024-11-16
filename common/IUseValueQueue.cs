using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Damage情報のキューを使う(デキューする)とき呼ぶ.
/// </summary>
public interface IUseValueQueue
{
	/// <summary>
	/// Queueに入れたDamageInfoClassを使用してHPの変更をする.
	/// </summary>
	/// <param name="oneDamageInfoClass"></param>
	void ValueDequeue(OneDamageInfoClass oneDamageInfoClass);
}
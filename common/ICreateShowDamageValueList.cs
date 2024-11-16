using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// User用,表示用ダメージ等の生成.
/// </summary>
public interface ICreateShowDamageValueList
{
	/// <summary>
	/// User側での多段Hitの場合のDamage表示の再構築.
	/// TotalDamageとしてまとめられているのでCriticalHitも加味して良しなに1HitあたりのDamageを算出する.
	/// </summary>
	/// <param name="dc"></param>
	/// <returns>1HitあたりのDamageをまとめたもの.missはnull,回復はマイナスが入る</returns>
	IReadOnlyList<OneDamageInfoClass> ReconstructionDamage(DamageInfoClass dc);
}
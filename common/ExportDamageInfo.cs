using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Player.Enemy.
/// HitPointCalculation.DamageInfoClassのデータをInt32に纏めたりその逆の処理をする.
/// </summary>
public class ExportDamageInfo : MonoBehaviour
{
    //※Damage「0」はhitCountに入る."miss"は入らない.
    //Damage0とmissは区別する.

    /// <summary>
    /// 送信用にDamageInfoClass => Int に変換する.
    /// </summary>
    /// <param name="infoClass"></param>
    /// <returns></returns>
    public int ConvertToSendDamageInfoInt32(DamageInfoClass infoClass)
	{
		//角煮尿
		Debug.Log("  TotalDamage:" + infoClass.totalDamage
				+ "  Hit Count Def:" + infoClass.hitCountDefinition
							+ "  Critical:" + infoClass.criticalHitCount
				+ "  Condi:" + (int)infoClass.deadOrAlive
							+ "  hit Count:" + infoClass.hitCount
				);
	//	if (infoClass.hitCount == 0) return 0;

		//Damage=0のときのみ固定値を送る.
		if (infoClass.totalDamage == 0) infoClass.totalDamage = StaticMyClass.noDamagePaddingValue;	


		//順番大事.
		return int.Parse(
			infoClass.totalDamage.ToString()//.PadLeft(6, '0')
			+ infoClass.hitCountDefinition.ToString()
			+ infoClass.criticalHitCount.ToString() 
			+ ((int)infoClass.deadOrAlive).ToString()
			+ infoClass.hitCount.ToString());

	}

	/// <summary>
	/// 受信したときのInt => DamageInfoClassへの変換.
	/// </summary>
	/// <param name="SendDamageInfoInt32">定義Hit回数も渡すようにした.</param>
	/// <returns></returns>
	public DamageInfoClass ConvertToDamageInfoClass(int SendDamageInfoInt32)
	{
		Debug.Log("SendDamageInfoInt32	" + SendDamageInfoInt32);
		DamageInfoClass infoClass = new DamageInfoClass();
        //  if (SendDamageInfoInt32 == 0)                  return infoClass;

        //1桁目.Hit回数を取得.Damage0とmissは区別する.Damage=0でHitCountがあるならDamage0.HitCount=0ならmiss.

        infoClass.hitCount = (int)(SendDamageInfoInt32 / (long)Math.Pow(10, 0) % 10);
		//2桁目.状態を取得.
		int condi = (int)(SendDamageInfoInt32 / (long)Math.Pow(10, 1) % 10);
		//数値をEnumに変換.
		infoClass.deadOrAlive = (StaticMyClass.ReceiveDamageInfoIntObjectCondition)Enum.ToObject(typeof(StaticMyClass.ReceiveDamageInfoIntObjectCondition), condi);
		//3桁目.Critical回数を取得.
		infoClass.criticalHitCount = (int)(SendDamageInfoInt32 / (long)Math.Pow(10, 2) % 10);
		//4桁目.定義Hit回数.
		infoClass.hitCountDefinition = (int)(SendDamageInfoInt32 / (long)Math.Pow(10, 3) % 10);
		//5～10桁目.TotalDamage.9999*9Hitなので5桁以降確保.Left:文字列の先頭から指定した長さの文字列を取得する
		infoClass.totalDamage = int.Parse(SendDamageInfoInt32.ToString().Substring(0, (SendDamageInfoInt32.ToString().Length - 4)));
		Debug.Log("TotalDamage:" + infoClass.totalDamage);
		if (infoClass.totalDamage == StaticMyClass.noDamagePaddingValue) infoClass.totalDamage = 0;
		//角煮尿
			Debug.Log("  TotalDamage:" + infoClass.totalDamage
					+ "  Hit Count Def:" + infoClass.hitCountDefinition
								+ "  Critical:" + infoClass.criticalHitCount
					+ "  Condi:" + (int)infoClass.deadOrAlive
								+ "  hit Count:" + infoClass.hitCount
					);
		return infoClass;
	}
}

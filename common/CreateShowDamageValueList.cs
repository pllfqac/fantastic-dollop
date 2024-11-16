using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Player.Enemy.
/// MasterからのDamage,回復情報のRPCを受けての自端末上での表示用の数値の算出.
/// Totalであっていればおｋとするので,このスクでランダム性持たせてる.
/// </summary>
public class CreateShowDamageValueList : MonoBehaviour,ICreateShowDamageValueList
{
    /// <summary>
    /// User側での多段Hitの場合のDamage表示の再構築.
    /// TotalDamageとしてまとめられているのでCriticalHitも加味して良しなに1HitあたりのDamageを算出する.
    /// </summary>
    /// <param name="dc"></param>
    /// <returns>1HitあたりのDamageをまとめたもの.Damage「0」は0.missはnull,回復はマイナスが入る</returns>
    public IReadOnlyList<OneDamageInfoClass> ReconstructionDamage(DamageInfoClass dc)
	{
        Debug.Log("--ReconstructionDamage--");
		var result = new List<OneDamageInfoClass>();

        //missのみ.
		if (dc.hitCount == 0)
		{
            Debug.Log("dc.hitCount == 0");                  //Damage「0」はここには入らない."miss"は入る.
			for(int i=0; i< dc.hitCountDefinition; i++)
			{
				Debug.Log("Hitしなかった.空OneDamageInfoClass作成	");
				result.Add(new OneDamageInfoClass(null, false));	
			}
			return result;
		}

		//クリティカル無しの通常ダメージ1Hitぶんを算出.四捨五入でいいのか不明.
		var oneHitDamage = (dc.totalDamage / (dc.hitCount + 0.5f * dc.criticalHitCount));
		Debug.Log("One Hit Damage " + oneHitDamage+ "  criticalHitCount:"+dc.criticalHitCount + "  defineHitCount:"+dc.hitCountDefinition);
		//1Hitあたり10%の幅を持たせてTotalの整合性を確保して表示用Damageを作成.
		if ((dc.hitCount - dc.criticalHitCount) > 0) { result.AddRange(CreateOneDamageList(AddRandomness(oneHitDamage, (dc.hitCount - dc.criticalHitCount)),false)); }    //ノーマルダメージ
		if (dc.criticalHitCount > 0) result.AddRange(CreateOneDamageList(AddRandomness((oneHitDamage * 1.5f), dc.criticalHitCount),true)); //Critical																											
		SmallValueCorrection(result, dc.totalDamage);  //Totalで微妙な差がでるので補正
		CreateShowList(result, dc);        //missを追加してランダムにシャッフル.
		return result;
	}


	//補助Method
	private List<OneDamageInfoClass> CreateOneDamageList( List<int?> vs,bool isClitical)
	{
		var lis = new List<OneDamageInfoClass>();

		foreach(var p in vs)
		{
			lis.Add(new OneDamageInfoClass(p, isClitical));
		}
		return lis;
	} 


/*	/// <summary>
	/// User側での多段Hitの場合のDamage表示の再構築.
	/// TotalDamageとしてまとめられているのでCriticalHitも加味して良しなに1HitあたりのDamageを算出する.
	/// </summary>
	/// <param name="dc"></param>
	/// <returns>1HitあたりのDamageをまとめたもの.missはnull,回復はマイナスが入る</returns>
	public IReadOnlyList<int?> ReconstructionDamage(DamageInfoClass dc)
	{
		var lis = new List<int?>();
		if (dc.hitCount == 0)
		{
			lis.Add(null);
			return lis;
		}
		//クリティカル無しの通常ダメージ1Hitぶんを算出.四捨五入でいいのか不明.
		var oneHitDamage = (dc.totalDamage / (dc.hitCount + 0.5f * dc.criticalHitCount));
		//Debug.Log("One Hit Damage " + oneHitDamage+"  deff:"+( dc.hitCount - dc.criticalHitCount));
		//1Hitあたり10%の幅を持たせてTotalの整合性を確保して表示用Damageを作成.
		if ((dc.hitCount - dc.criticalHitCount) > 0) { lis.AddRange(AddRandomness(oneHitDamage, (dc.hitCount - dc.criticalHitCount))); }    //ノーマルダメージ
		if (dc.criticalHitCount > 0) lis.AddRange(AddRandomness((oneHitDamage * 1.5f), dc.criticalHitCount)); //Critical																											
		SmallValueCorrection(lis, dc.totalDamage);  //Totalで微妙な差がでるので補正
		CreateShowList(lis, dc);		//missを追加してランダムにシャッフル.
		return lis;
	}*/


	/// <summary>
	/// 幅を持たせてTotalの整合性を確保する.
	/// ノーマルHitとCriticalHitは独立して考える.
	/// </summary>
	/// <param name="oneHitDamageValue"></param>
	/// <returns>oneHitDamageValueに10%の幅を持たせた値のList.</returns>
	private List<int?> AddRandomness(float oneHitDamageValue, int hitCount)
	{
		if (hitCount == 1) return new List<int?>() { (int)Math.Round(oneHitDamageValue) };
		int totalWidthSum = 0;  //付与したRandom値の合計.この値がHitCount数分の和で0になるように以下の計算をしている.
		List<int?> vs = new List<int?>();
		for (int remaining = hitCount; remaining > 0; remaining--)
		{
			int randomValue = 0;    //付与するRandom値.
									//Debug.Log("Remaining:" + remaining + "   TotalWidthSum:" + totalWidthSum);
									//残りのhitCount数で、合計値が0になり±10%に収まる範囲の値を求める.
			if (remaining == 1)
			{
				randomValue -= totalWidthSum;
			}
			else
			{
				//取ることのできる最大・最小値からランダム値を計算
				if (totalWidthSum >= 0) randomValue = (int)Math.Round(UnityEngine.Random.Range(-oneHitDamageValue * 0.1f, (oneHitDamageValue * 0.1f) - totalWidthSum));
				else randomValue = (int)Math.Round(UnityEngine.Random.Range((-oneHitDamageValue * 0.1f) - totalWidthSum, oneHitDamageValue * 0.1f));
			}
			totalWidthSum += randomValue;
			vs.Add((int)Math.Round(oneHitDamageValue, MidpointRounding.AwayFromZero) + randomValue);
			Debug.Log("One Hit Damage " + Math.Round(oneHitDamageValue, MidpointRounding.AwayFromZero)
				+ "  Remaining:" + remaining + "   totalWidthSum:" + totalWidthSum + "  RandomValue:" + randomValue);
		}
		return vs;
	}


	private void SmallValueCorrection(List<int?> resultDamageList, int totalDamage)
	{
		//算出した誤差はとりま配列の最後の値に足す.
		var p = resultDamageList.Last() - (resultDamageList.Sum() - totalDamage);
		resultDamageList[resultDamageList.Count - 1] = p;
	}

	private void SmallValueCorrection(List<OneDamageInfoClass> resultDamageList, int totalDamage)
	{
		//算出した誤差はとりま配列の最後の値に足す.
		//int oneDamageSum = ((resultDamageList.Where(x => x.OneDamage != null).Select(x => (int)x.OneDamage).Sum()));     //missであるnullは除く.
		int? p = resultDamageList.Last().OneDamage - (((resultDamageList.Select(x => (int)x.OneDamage).Sum())) - totalDamage);
		resultDamageList[resultDamageList.Count - 1].OneDamage = p;
	}

	/// <summary>
	/// miss表示も含めた表示用List.
	/// [miss]の部分はnull.
	/// </summary>
	/*private void CreateShowList(List<int?> damageList, DamageInfoClass dc)																
	{
		//missを追加.
		if ((dc.hitCountDefinition - dc.hitCount) != 0)
		{
			for (int i = 0; i < (dc.hitCountDefinition - dc.hitCount); i++)
			{
				damageList.Add(null);
			}
		}
		//シャッフル
		Shuffle<int?>(damageList);
	}*/

	private void CreateShowList( List<OneDamageInfoClass> damageList, DamageInfoClass dc) 
	{
		//missを追加.
		if ((dc.hitCountDefinition - dc.hitCount) != 0)
		{
			for (int i = 0; i < (dc.hitCountDefinition - dc.hitCount); i++)
			{
                Debug.Log("missの作成");
                damageList.Add(new OneDamageInfoClass(null,false));
			}
		}
		//シャッフル
		Shuffle<OneDamageInfoClass>(damageList);
	}

	/// <summary>
	/// ランダムに並び替え
	/// </summary>
	private void Shuffle<T>(List<T> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			T temp = list[i];
			int randomIndex = UnityEngine.Random.Range(0, list.Count);
			list[i] = list[randomIndex];
			list[randomIndex] = temp;
		}
	}


}

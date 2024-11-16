using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;

/// <summary>
/// MasterOnly.
/// Player.Enemy 
/// クリティカルヒットの確率の計算.
/// </summary>
public class JudgmentCritical : IJudgmentCritical
{
	/*void Start()
	{
		if (!PhotonNetwork.IsMasterClient) Destroy(this);
	}*/

	/// <summary>
	/// CriticalHitの場合True.
	/// </summary>
	/// <param name="skillUserCsc"></param>
	/// <param name="skillReceiveUserCsc"></param>
	/// <returns></returns>
	public Task<bool> JudgeCriticalHit(CommonStatusClass skillUserCsc, CommonStatusClass skillReceiveUserCsc)
	{
		//skill使用者の(Dex+Agi)/2
		int skillUserValue = (skillUserCsc.Dexerity + skillUserCsc.Agility) / 2;
		int diffValue = skillUserValue - skillReceiveUserCsc.Agility;

		float criticalHitProbability = StaticMyClass.CriticalHitGradients * diffValue + StaticMyClass.CriticalHitIntercepts;
		if (criticalHitProbability < 0) criticalHitProbability = StaticMyClass.CriticalHitMinRate;
		if (criticalHitProbability > 90) criticalHitProbability = StaticMyClass.CriticalHitMaxRate;

		int randomValue = Random.Range(StaticMyClass.CriticalHitMinRate, 101);

		//return criticalHitProbability > randomValue;
		bool result = criticalHitProbability > randomValue;
		return Task.FromResult(result);
	}
}

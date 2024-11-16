using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// MasterOnly.
/// Player.Enemy.
/// クリティカルヒットの確率の計算.
/// </summary>
public interface IJudgmentCritical
{
	/// <summary>
	/// CriticalHitかどうかを計算
	/// </summary>
	/// <param name="skillUserCsc">Skill使用者</param>
	/// <param name="skillReceiveUserCsc">Skill被使用者</param>
	/// <returns>CriticalHitの場合True.</returns>
	 Task< bool> JudgeCriticalHit(CommonStatusClass skillUserCsc, CommonStatusClass skillReceiveUserCsc);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyHistory
{
	void SaveDamageData(int damage, AttackInfoClass aic, IMasterPlayerExpLog explog);
	Dictionary<int, EnemyHistory.EnemyDamageInfoClass> GetDamageRatioTable();
	Dictionary<int, IMasterPlayerExpLog> GetExpLogTable();
	void LifeZero();
}

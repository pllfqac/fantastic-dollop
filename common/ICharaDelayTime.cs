using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// Skill,Item使用時のDelayTimeの管理.
/// </summary>
public interface ICharaDelayTime
{
	/// <summary>
	/// DelayTimeの開始.
	/// </summary>
	/// <param name="delayTime"></param>
	Task StartDelayTime(float delayTime);
}

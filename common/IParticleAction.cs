using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IParticleAction 
{

	/// <summary>
	/// Particleの再生.
	/// miss(null)だったら何も表示しない.
	/// Damage,Revi共通.
	/// </summary>
	/// <param name="receiveValue">damageなら正,Reviなら負の値.missならnull.</param>
	/// <param name="isCritical">CriticalならTrue.</param>
	void ReceiveParticlePlay(int? receiveValue, bool isCritical);


	//蘇生
	void ResuscitationParticlePlay();

	//Level UP
	void LevelUpParticlePlay();


	//UCC受けた時
	void ReceiveUccParticlePlay(SkillInfoClass.Condition condition);
	//HPDown,ParlizeはParticle継続表示.UC終了RPCでParticleも終了.
	void HPDownParticleEnd();
	void ParalyzePaticleEnd();
}

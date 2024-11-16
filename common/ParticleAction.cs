using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Player or Enemy/PlayerParticles.
/// Particleの制御.
/// </summary>
public class ParticleAction : MonoBehaviour,IParticleAction
{
	/// Skill使用時のParticle呼出しはCharaAnimationEvent.cs



	// Inspector
	[SerializeField]
	private ParticleSystem receiveDamageParticle=null;
	[SerializeField]
	private ParticleSystem criticalHitParticle = null;
	[SerializeField]
	private ParticleSystem reviParticle = null;
	[SerializeField]
	private ParticleSystem reviCriticalParticle = null;     //Reviのクリティカル.
	[SerializeField]
	private ParticleSystem resuscitationParticle = null;
	[SerializeField]
	private ParticleSystem levelUpParticle = null;

	//Ucc
	[SerializeField]
	private ParticleSystem statusUpParticle = null;
	[SerializeField]
	private ParticleSystem statusDownParticle = null;
/*	[SerializeField]
	private ParticleSystem velocityUpParticle = null;
	[SerializeField]
	private ParticleSystem velocityDownParticle = null;*/
	[SerializeField]
	private ParticleSystem hpDownParticle = null;
	[SerializeField]
	private ParticleSystem paralyzeParticle = null;


	//===========================

	/// <summary>
	/// Damage,Reviを受けた時のParticleの再生.
	/// </summary>
	/// <param name="receiveValue"></param>
	/// <param name="isCritical">CriticalならTrue.</param>
	public void ReceiveParticlePlay(int? receiveValue, bool isCritical)
	{
		if (receiveValue == null) return;

		if(receiveValue>0)
		{
			//Damage.
			if (isCritical) criticalHitParticle.Play();
			else receiveDamageParticle.Play();
		}
		else if(receiveValue<0)
		{
			//Revi.
			if (isCritical) reviCriticalParticle.Play();
			else reviParticle.Play();
		}
	}

	/// <summary>
	/// 蘇生のパーティクル.
	/// </summary>
	public void ResuscitationParticlePlay()
	{
		resuscitationParticle.Play();
	}


	/// <summary>
	/// LevelUPしたとき
	/// </summary>
	public void LevelUpParticlePlay()
	{
		levelUpParticle.Play();
	}

	/// <summary>
	/// UCC受けた時.
	/// 自キャラOnlyの一部SkillはこのParticleを使用しない.
	/// </summary>
	/// <param name="condition"></param>
	public void ReceiveUccParticlePlay(SkillInfoClass.Condition condition)
	{
		switch (condition)
		{
			case SkillInfoClass.Condition.None:
				break;
			case SkillInfoClass.Condition.StatusUpAll:
			case SkillInfoClass.Condition.StatusUp_Pwr:
			case SkillInfoClass.Condition.StatusUp_Dex:
			case SkillInfoClass.Condition.StatusUp_Def:
			case SkillInfoClass.Condition.StatusUp_Mat:
			case SkillInfoClass.Condition.StatusUp_Mde:
			case SkillInfoClass.Condition.StatusUp_Agi:
			case SkillInfoClass.Condition.VelocityUp:
				Debug.Log("Status Up Particle");
				statusUpParticle.Play();
				break;

			case SkillInfoClass.Condition.StatusDownAll:
			case SkillInfoClass.Condition.StatusDown_Pwr:
			case SkillInfoClass.Condition.StatusDown_Dex:
			case SkillInfoClass.Condition.StatusDown_Def:
			case SkillInfoClass.Condition.StatusDown_Mat:
			case SkillInfoClass.Condition.StatusDown_Mde:
			case SkillInfoClass.Condition.StatusDown_Agi:
			case SkillInfoClass.Condition.VelocityDown:
				Debug.Log("Debuff Particle");
				DebuffParticlePlay();
				break;
			case SkillInfoClass.Condition.HpDown:hpDownParticle.Play();break;
			case SkillInfoClass.Condition.paralyze:paralyzeParticle.Play();break;
			default:break;
		}
	}


	//Debuff
	private void DebuffParticlePlay()
	{
		//Cameraの方向にZ軸の未回転
		Quaternion quaternion = Quaternion.LookRotation(Camera.main.transform.position - statusDownParticle.transform.position, Vector3.up);
		quaternion.x = 0;
		quaternion.z = 0;
		quaternion.eulerAngles += new Vector3(90, 0, 0);
		statusDownParticle.transform.rotation = quaternion;
		//少しカメラ側に移動
		statusDownParticle.transform.localPosition = new Vector3(0.5f, 2.3f, 0.5f);
		statusDownParticle.Play();
	}


	//HPDown,ParlizeはParticle継続表示.UC終了RPCでParticleも終了.
	public void HPDownParticleEnd()
	{
		hpDownParticle.Stop();
	}

	public void ParalyzePaticleEnd()
	{
		paralyzeParticle.Stop();
	}

	//==================================================== Skill Animation Clipの「イベント」で呼ぶメソッド ====================================================
	//===>> CharaAnimationEvent.csで検知.



}

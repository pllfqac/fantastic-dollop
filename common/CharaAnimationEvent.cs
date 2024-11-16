using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

/// <summary>
/// Player.
/// すべてSkillAnimationのイベントから呼ばれる.
/// Particleの発生等.
/// </summary>
public class CharaAnimationEvent : MonoBehaviour
{
	[SerializeField]
	private SkillParticleManager spm;
	private TrailRendererManager trm;
	private IRendererController rendererController;
	private Vector3 pos;    //Skill26.自キャラの移動を伴うSkillのため元の位置一時保存用.
	private Animator animator;
	private IConvertDirection convertDirection;
	[SerializeField]
	private PlayerHitWarpPoint playerWarp = null;
	[NonSerialized]
	public MyCamera.CameraController cameraCtrl;        //Instant
	[SerializeField]
	private SkillColliderManager aa;
	private PhotonView view;

	//skill26用Curve
	private AnimationCurve curve;



	private void Start()
	{
		animator = GetComponent<Animator>();
		trm = GetComponent<TrailRendererManager>();
		rendererController = GetComponent<IRendererController>();
		convertDirection = GetComponent<IConvertDirection>();
		view = GetComponent<PhotonView>();
	}

	/// <summary>
	/// Animationのイベントで呼ぶ.                                  廃止で
	/// </summary>
	public void SkillEnd()
	{
		Debug.Log("廃止で");
	}

	//==============Hitしたとき==============
	public void Hit()
	{
		if (view.IsMine)
		{
			Debug.Log("<size=30> Hit </size>");
			aa.IncrementHitEvent();
			aa.SendHit();
		}
	}

	//==============共通したSkillParticle呼出し==============
	public void SkillParticleEvent(int abs)
	{
		Debug.Log("<size=25>Particle Event!" + abs.ToString() + "</size>");
		spm.GetParticleSystem((byte)abs).Play();
	}



	/// <summary>
	/// Particleをコルーチンで動かす.
	/// </summary>
	/// <param name="abs"></param>
	public void StartParticleMove(int abs)
	{
		StartCoroutine(spm.ParticleMoveCoroutine(spm.GetParticleSystem((byte)abs)));
	}


	//==============個別に定義が必要なSkillのParticle呼出し等==============
	#region Sword
	//Skill2 
	public void SkillParticleEvent2(int slashAngle)
	{
		// クォータニオン → オイラー角への変換
		Vector3 rotationAngles = spm.GetParticleSystem(2).transform.localRotation.eulerAngles;
		rotationAngles.z = slashAngle;
		// オイラー角 → クォータニオンへの変換
		spm.GetParticleSystem(2).transform.localRotation = Quaternion.Euler(rotationAngles);
		spm.GetParticleSystem(2).Play();
	}

	//Skill2 突き
	public void SkillParticleEvent2_2()
	{
		spm.GetSkill2_2Particle().Play();
	}



	#endregion
	#region TwoHandSword
	//Skill8 薙ぎ払い
	public void SkillParticleEvent8_2()
	{
		spm.StartParticle8_2();
	}

	public void SkillParticleEvent9()
	{
		spm.StartParticle9();
	}
	#endregion

	#region Spear
	//Spear NormalAttack
	public void SpearNormalAttackParticle()
	{
		spm.StartSpearNormalAttackParticle();
	}

	//Skill11 3撃目
	public void SkillParticleEvent11_2()
	{
		spm.StartParticle11_2();
	}

	//Skill12.Trail系のParticleを使用しているのでTrailRendererManager.csで管理?
	public void SkillParticleEvent12()
	{
		trm.SetTrailParticleForWeapon(spm.GetParticleSystem(12));
		spm.StartParticle12();
	}

	//Skill12の終了で呼ぶ
	public void Skill12ParticleEnd()
	{
		spm.EndParticle12();
		spm.GetParticleSystem(12).Stop();
		trm.ResetTrailParticleForWeapon(spm.gameObject.transform);
	}

	public void SkillParticleEvent13_2()
	{
		spm.StartParticle13_2();
	}

	#endregion
	#region Staff
	public void Skill20Animation()
	{
		spm.StartParticle20();
	}
	#endregion
	#region Dagger

	/// <summary>
	/// Skill22,26
	/// TrailのMaterialを変更.
	/// Animationのイベントとして設定する場合は,Intの項目に対応する数値を入力すること
	/// </summary>
	public void ChangeTrailMaterial(int skillAbs)
	{
		trm.ChangeMaterialForSkill(skillAbs);
	}

	public void ReturnTrailMateril()
	{
		trm.ReturnDefaultTrailMaterial();
	}

	//Skill26.
	/*	public void Skill26Action()
		{
			Debug.Log("Skill26Action!!");
			convertDirection.IsDisabled = true;
			trm.ChangeMaterialForSkill(26);
			spm.StartParticle26();
			trm.StartTrail();
			trm.StartOtherTrail();
			pos = transform.position;
			animator.applyRootMotion = true;

			//自キャラのみ
			if (!view.IsMine) return;
			playerWarp.enableWarp = false;  //Warpしないように
			cameraCtrl.Target = null;       //cameraの追従を止める.
		}



		public void Skill26Move(float moveDistance)
		{
			//this.transform.position += transform.forward * moveDistance;
			this.transform.Translate(0, 0, moveDistance);
		}

	 */



	//Swordsman@MoveAttack1_Sを使うVer.指定したUnitだけ動く.
	public void Skill26Action(float moveDistance)
	{
		Debug.Log("Skill26Action!!");
		convertDirection.IsDisabled = true;
		trm.ChangeMaterialForSkill(26);
		spm.StartParticle26();
		trm.StartTrail();
		trm.StartOtherTrail();
		pos = transform.position;
		//	animator.applyRootMotion = true;
		StartCoroutine(CharaMoveEnumerator(moveDistance));
		//自キャラのみ
		if (!view.IsMine) return;
		playerWarp.enableWarp = false;  //Warpしないように
		cameraCtrl.Target = null;       //cameraの追従を止める.
	}


	/// <summary>
	/// Skill26でのCharaの移動用.
	/// </summary>
	/// <param name="moveDistance">移動させたいUnit数を指定.</param>
	/// <returns></returns>
	private IEnumerator CharaMoveEnumerator(float moveDistance)
	{
		curve = AnimationCurve.Linear(0, 0, 0.3f, moveDistance);
		float startTime = Time.time;
		while ((Time.time - startTime) < 0.3f)      //0.3s → Charaの動き始めから停止位置までのAnimation上の時間.
		{
			float curveValue = curve.Evaluate((Time.time - startTime));     //Evaluate()	評価したいカーブ内の時間のカーブの値を返す
																			//Debug.Log("curveValue:" + curveValue);
			transform.position = pos + transform.forward * curveValue;                  //位置の変更
			yield return null;
		}
	}


	//Skill26攻撃モーション中に呼ばれる.
	public void Skill26TrailEnd()
	{
		trm.EndTrail();
		trm.EndOtherTrail();
	}

	//Skill26攻撃モーション終了で呼ばれる.
	public void Skill26RePosition()
	{
		Debug.Log("Skill26RePosition");
		trm.ReturnDefaultTrailMaterial();
		spm.SaveLeaveAlonePositionForSkill26particle(); //Particleは遅れて戻す.今の位置を一時保存.
														//	animator.applyRootMotion = false;	//Animationによる位置移動を停止する.
		rendererController.QuickOut();
		transform.position = pos;
		spm.ResetSkill26ParticlePosition();     //Particleは遅れて戻すので位置そのまま.
		rendererController.QuickIn();
		convertDirection.IsDisabled = false;

		//自キャラのみ
		if (!view.IsMine) return;
		playerWarp.enableWarp = true;
		cameraCtrl.Target = this.transform;
	}

	#endregion
	#region Bow

	//Bowのイベントの一部はEquipmentObjectManager.csで受ける.

	//ArrowオブジェのTailを放出開始
	public void Skill27EnableTrail()
	{
		trm.ChangeArrowTrail(true);
	}


	public void Skill29Action()
	{
		StartCoroutine(spm.Skill29());
	}

	public void Skill30Action()
	{
		spm.StartSkill30();
	}

	#endregion
	#region Unarmed
	public void StartUnarmedNormalAttackParticle()
	{
		spm.StartUnarmedNormalAttack();
	}

	#endregion


}

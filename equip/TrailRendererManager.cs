using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Player.
/// 武器オブジェクトにつけたTrailRendererとAnimationの連携を管理.
/// </summary>
public class TrailRendererManager : MonoBehaviour, IArrowObjectTrailRenderer
{
	//Spear,Unarmedは現状TrailRenderer無し

	//Trail用デフォMaterial
	[SerializeField]
	private Material defaultTrailMaterial = null;

	//Dagger.Skill22用.
	[SerializeField]
	private Material skill22Material = null;
	//Skill26用.
	[SerializeField]
	private Material skill26Material = null;

	/// <summary>
	/// 装備中のWeaponのTrailの参照.
	/// </summary>
	[NonSerialized]
	public TrailRenderer equippingWeaponTrailRenderer = null;

	/// <summary>
	/// そのほかのTrailへの参照.
	/// </summary>
	[NonSerialized]
	public TrailRenderer OtherTrailRenderer = null;


	private Color tempColor;

	//生成した「Arrowオブジェ」のTrailRenderer.
	public TrailRenderer ArrowObjectTrailRendere { set; private get; }


	/// <summary>
	/// Skillのアニメーションイベントに登録する(一部コードからも呼ぶ).
	/// (大多数の武器の)Skill使用Animationで呼ばれるEvent.Trail表示のオンオフ.
	/// </summary>
	public void StartTrail()
	{
		if (equippingWeaponTrailRenderer != null) equippingWeaponTrailRenderer.emitting = true;
	}

	public void EndTrail()
	{
		if (equippingWeaponTrailRenderer != null) equippingWeaponTrailRenderer.emitting = false;
	}


	/// <summary>
	/// Skillのアニメーションイベントに登録する.
	/// AnimationClipにevent設定した,Daggerの左手用
	/// </summary>
	public void StartOtherTrail()
	{
		if (OtherTrailRenderer != null) OtherTrailRenderer.emitting = true;
	}

	public void EndOtherTrail()
	{
		if (OtherTrailRenderer != null) OtherTrailRenderer.emitting = false;
	}



	//Particles_Skillsオブジェの子に置いているParticleを,武器オブジェのTrailRendererManagerと同じ階層に移動.
	public void SetTrailParticleForWeapon(ParticleSystem particle)
	{
		/*
		 * Particles_Skillsオブジェの子に置いているParticleを,武器オブジェのTrailRendererManagerと同じ階層に移動
		 * ParticleのPlay.
		 * 位置を戻す.
		 */
		if (equippingWeaponTrailRenderer == null) return;
		particle.transform.parent = equippingWeaponTrailRenderer.transform;
		//	particle.transform.localPosition = new Vector3(1.6f, 0.5f, 0.7f);	//Vector3.zero;						//半径方向にやや大きめにつける
		particle.transform.localPosition = Vector3.zero;                        //大きすぎた

		particle.Play();
	}


	/// <summary>
	/// 武器オブジェのTrailRendererManagerと同じ階層に移動させたParticleを,Particles_Skillsオブジェの子に戻す
	/// </summary>
	/// <param name="targetParent">Particleの元あった場所.</param>
	public void ResetTrailParticleForWeapon(Transform targetParent)
	{
		if (equippingWeaponTrailRenderer == null) return;
		foreach (Transform child in equippingWeaponTrailRenderer.transform)
		{
			child.parent = targetParent;
			child.transform.localPosition = Vector3.zero;
		}
	}


	/// <summary>
	/// Skill用にMaterialを変更する.
	/// </summary>
	public void ChangeMaterialForSkill(int skillAbs)
	{
		if (equippingWeaponTrailRenderer == null || OtherTrailRenderer == null) return;

		switch (skillAbs)
		{
			case 22:
				equippingWeaponTrailRenderer.material = skill22Material;
				OtherTrailRenderer.material = skill22Material;
				break;
			case 26:
				equippingWeaponTrailRenderer.material = skill26Material;
				OtherTrailRenderer.material = skill26Material;
				break;
			default: ReturnDefaultTrailMaterial(); break;
		}
	}

	/// <summary>
	/// TrailのMaterialを元に戻す.
	/// </summary>
	public void ReturnDefaultTrailMaterial()
	{
		if (equippingWeaponTrailRenderer == null || OtherTrailRenderer == null) return;
		equippingWeaponTrailRenderer.material = defaultTrailMaterial;
		OtherTrailRenderer.material = defaultTrailMaterial;
	}






	/// <summary>
	/// ArrowオブジェのTrailを有効無効にする.
	/// </summary>
	/// <param name="arrowTrailEnable">Trueで有効</param>
	public void ChangeArrowTrail(bool arrowTrailEnable)
	{
		if (ArrowObjectTrailRendere == null) return;
		ArrowObjectTrailRendere.emitting = arrowTrailEnable ? true : false;
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Canvas-OtherUserDamageUIGenerator.
/// 自キャラ以外のオブジェのDamage表示UI用PrefabをInstantiateする.
/// </summary>
public class InstantiateDamageUIGenerator : MonoBehaviour
{
	/// <summary>
	/// Damage表示用のTextをまとめたPrefab.
	/// </summary>
	[SerializeField]
	private GameObject DamageUIGenerator = null;


	/// <summary>
	/// BR切替後のみかつphotonCallbackで呼ばれる.
	/// </summary>
	/// <param name="transform">自端末に生成されたキャラのTransform.</param>
	public void InstantiateOthePlayerDamageUIGenerator(Transform transform)
	{
		GameObject damageUIGenerator = Instantiate(DamageUIGenerator);
		DamageUIAnimationCtrl dui= damageUIGenerator.GetComponent<DamageUIAnimationCtrl>();

		transform.GetComponent<HitPointCalculation>().DamageUIAnimationCtrl = dui;
		transform.GetComponent<ResultValueQueue>().DamageUIAnimationCtrl = dui;
		//変な気もするが,OtherPlayerのParticle参照はここで入れてる.
		dui.particleAction= transform.Find("Particles_Common").GetComponent<IParticleAction>();
		damageUIGenerator.GetComponent<DamageUIPosition>().target = transform;
		damageUIGenerator.transform.SetParent(this.gameObject.transform, false);
	}
}

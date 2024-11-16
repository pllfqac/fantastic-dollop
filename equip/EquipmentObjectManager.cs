using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

/// <summary>
/// Player.
/// 表示されるEquipmentオブジェの表示制御.
/// </summary>
public class EquipmentObjectManager : MonoBehaviourPun, IEquipmentObjectManager
{
	[NonSerialized]
	public IEquipmentObjectDataTable EquipmentObjectDataTable = null;        //Instant or PhotonCallback.
	[NonSerialized]
	public ILoadItemDefinitionByAbs byAbs = null;       //Ins or PhotonCallback.
	[NonSerialized]
	public IGetSceneType getScene = null;   //Instatiate. or photonCallback.
	private TrailRendererManager trailRendererManager;

	//装備を持つ箇所.
	[SerializeField]
	private Transform EquipmentEndPoint_R = null;
	[SerializeField]
	private Transform EquipmentEndPoint_L = null;
	//Dageer左手専用
	[SerializeField]
	private Transform Dagger_L_Transform = null;

	//現在装備中の保持.
	private GameObject tempEquipping_R = null;
	private GameObject tempEquipping_L = null;
	private GameObject tempEquipping_Head = null;

	//生成したArrowの位置用.
	[SerializeField]
	private Transform arrowTr = null;
	Vector3 tempRotation;

	private void Start()
	{
		trailRendererManager = GetComponent<TrailRendererManager>();
	}


	/// <summary>
	/// FadeでEquipの表示・非表示をするために取得.
	/// </summary>
	/// <returns>Rendererがあるもののみ返す.１つも無い場合はnullを返す.</returns>
	public List<Renderer> GetEquippingObjRenderer()
	{
		List<Renderer> renderers = new List<Renderer>();

		if (tempEquipping_Head != null) renderers.Add(tempEquipping_Head.GetComponent<Renderer>());

		if (getScene?.GetSceneType() != SceneDefineTable.SceneType.town)
		{
			if (tempEquipping_L != null) renderers.AddRange(GetAllRenderer(tempEquipping_L));//renderers.Add(tempEquipping_L.GetComponent<Renderer>());
			if (tempEquipping_R != null) renderers.AddRange(GetAllRenderer(tempEquipping_R));//renderers.Add(tempEquipping_R.GetComponent<Renderer>());
																							 //Townの時はWeaponは常に非表示なので取得できない
		}
		Debug.Log("Head and Weapon Renderer Get! Count:	" + renderers.Count);
		/*	foreach(Renderer ren in renderers)
			{
				Debug.Log("Renderer obj Name:" + ren.transform.gameObject.name);
			}
		*/
		if (renderers.Count == 0) renderers = null;
		return renderers;
	}


	/// <summary>
	/// 親とすべての子のRendererを取得する.
	/// Fade対象でないShaderでも返す.
	/// 孫は対象外.
	/// </summary>
	/// <param name="ParentObject"></param>
	/// <returns></returns>
	private List<Renderer> GetAllRenderer(GameObject ParentObject)
	{
		List<Renderer> result = new List<Renderer>();
		if (ParentObject.TryGetComponent(out Renderer ren)) result.Add(ren);    //親
																				//子
		for (int i = 0; i < ParentObject.transform.childCount; i++)
		{
			if (ParentObject.transform.GetChild(i).gameObject.TryGetComponent(out Renderer renderer))
				result.Add(renderer);
		}
		return result;
	}

	//装備を外す.
	public void UnEquipObject(EquipmentDefinition eDef)
	{
		switch (eDef.ePlaceType)
		{
			case CharacterEquipmentPlace.EquipPlaceType.head:
				break;
			case CharacterEquipmentPlace.EquipPlaceType.body:
				break;
			case CharacterEquipmentPlace.EquipPlaceType.back:
				break;
			case CharacterEquipmentPlace.EquipPlaceType.arm1:
				UnEquipmentWeapon(eDef.WeaponEquipmentType);
				break;
			case CharacterEquipmentPlace.EquipPlaceType.arm2:
				Destroy(tempEquipping_L);
				break;
			case CharacterEquipmentPlace.EquipPlaceType.legs:
				break;
			default:
				break;
		}
	}

	//武器の場合さらに場合分け.
	private void UnEquipmentWeapon(StaticMyClass.WeaponEquipmentType weaponType)
	{
		switch (weaponType)
		{
			case StaticMyClass.WeaponEquipmentType.none:
				Destroy(tempEquipping_R);
				Destroy(tempEquipping_L);
				break;
			case StaticMyClass.WeaponEquipmentType.Sword:
				Destroy(tempEquipping_R);
				break;
			case StaticMyClass.WeaponEquipmentType.TwoHandSword:
				Destroy(tempEquipping_R);
				break;
			case StaticMyClass.WeaponEquipmentType.Spear:
				Destroy(tempEquipping_R);
				break;
			case StaticMyClass.WeaponEquipmentType.Staff:
				Destroy(tempEquipping_R);
				break;
			case StaticMyClass.WeaponEquipmentType.Dagger:
				Destroy(tempEquipping_R);
				Destroy(tempEquipping_L);
				break;
			case StaticMyClass.WeaponEquipmentType.Bow:
				Destroy(tempEquipping_R);
				Destroy(tempEquipping_L);
				bowObjectAnimator = null; //要らんかもしれん
				break;
			case StaticMyClass.WeaponEquipmentType.Unarmed:
				Destroy(tempEquipping_R);
				Destroy(tempEquipping_L);
				break;
			default:
				break;
		}
		trailRendererManager.equippingWeaponTrailRenderer = null;
		trailRendererManager.OtherTrailRenderer = null;
		trailRendererManager.ArrowObjectTrailRendere = null;
	}

	/// <summary>
	/// 装備箇所を指定した装備解除.
	/// </summary>
	/// <param name="placeType"></param>
	public void UnEquipObject(CharacterEquipmentPlace.EquipPlaceType placeType)
	{
		switch (placeType)
		{
			case CharacterEquipmentPlace.EquipPlaceType.head:
				Destroy(tempEquipping_Head);
				break;
			case CharacterEquipmentPlace.EquipPlaceType.body:
				break;
			case CharacterEquipmentPlace.EquipPlaceType.back:
				break;
			case CharacterEquipmentPlace.EquipPlaceType.arm1:
				UnEquipWeaponByTempObj();
				break;
			case CharacterEquipmentPlace.EquipPlaceType.arm2:
				Destroy(tempEquipping_L);
				break;
			case CharacterEquipmentPlace.EquipPlaceType.legs:
				break;
			default:
				break;
		}
	}

	private void UnEquipWeaponByTempObj()
	{
		if (tempEquipping_R == null) return;
		//Tagで判断.
		StaticMyClass.WeaponEquipmentType weapon;
		if (!Enum.TryParse(tempEquipping_R.tag, out weapon)) return;        //変換失敗なら何もしない.
		else UnEquipmentWeapon(weapon);
	}



	//装備をする.
	public void EquipObject(byte itemAbs)
	{
		GameObject insObj = EquipmentObjectDataTable.GetGameObject(itemAbs);
		if (insObj == null) return;     //無理はしない.

		switch (byAbs.GetEquipmentItemDefinition(itemAbs).ePlaceType)
		{
			case CharacterEquipmentPlace.EquipPlaceType.head:   //未実装.Townでも表示&変更あり.
				break;
			case CharacterEquipmentPlace.EquipPlaceType.body:
				break;
			case CharacterEquipmentPlace.EquipPlaceType.back:
				break;
			case CharacterEquipmentPlace.EquipPlaceType.arm1:
				EquipObjectWeapon(insObj, byAbs.GetEquipmentItemDefinition(itemAbs).WeaponEquipmentType);
				break;
			case CharacterEquipmentPlace.EquipPlaceType.arm2:
				EquipObjectArm2(insObj);
				break;
			case CharacterEquipmentPlace.EquipPlaceType.legs:
				break;
			default:
				break;
		}
	}

	//武器の場合さらに場合分け.
	private void EquipObjectWeapon(GameObject insObj, StaticMyClass.WeaponEquipmentType weapon)
	{
		switch (weapon)
		{
			case StaticMyClass.WeaponEquipmentType.none:
				break;
			case StaticMyClass.WeaponEquipmentType.Sword:
				Destroy(tempEquipping_R);
				//Shield以外の左手オブジェは破棄.
				if (tempEquipping_L != null && tempEquipping_L.tag != "Shield") Destroy(tempEquipping_L);
				tempEquipping_R = Instantiate(insObj, EquipmentEndPoint_R);
				trailRendererManager.equippingWeaponTrailRenderer = tempEquipping_R.transform.Find("TrailPosition").GetComponent<TrailRenderer>();
				break;
			case StaticMyClass.WeaponEquipmentType.TwoHandSword:
				Destroy(tempEquipping_L);
				Destroy(tempEquipping_R);
				tempEquipping_R = Instantiate(insObj, EquipmentEndPoint_R);
				trailRendererManager.equippingWeaponTrailRenderer = tempEquipping_R.transform.Find("TrailPosition").GetComponent<TrailRenderer>();
				break;
			case StaticMyClass.WeaponEquipmentType.Spear:
				Destroy(tempEquipping_L);
				Destroy(tempEquipping_R);
				tempEquipping_R = Instantiate(insObj, EquipmentEndPoint_R);
				Debug.Log(tempEquipping_R.name);
				trailRendererManager.equippingWeaponTrailRenderer = tempEquipping_R.transform.Find("TrailPosition").GetComponent<TrailRenderer>();
				break;
			case StaticMyClass.WeaponEquipmentType.Staff:
				Destroy(tempEquipping_L);
				Destroy(tempEquipping_R);
				tempEquipping_R = Instantiate(insObj, EquipmentEndPoint_R);
				trailRendererManager.equippingWeaponTrailRenderer = tempEquipping_R.transform.Find("TrailPosition").GetComponent<TrailRenderer>();

				break;
			case StaticMyClass.WeaponEquipmentType.Dagger:
				Destroy(tempEquipping_L);
				Destroy(tempEquipping_R);
				tempEquipping_R = Instantiate(insObj, EquipmentEndPoint_R);
				trailRendererManager.equippingWeaponTrailRenderer = tempEquipping_R.transform.Find("TrailPosition").GetComponent<TrailRenderer>();
				tempEquipping_L = Instantiate(insObj, EquipmentEndPoint_L);
				trailRendererManager.OtherTrailRenderer = tempEquipping_L.transform.Find("TrailPosition").GetComponent<TrailRenderer>();
				//Dagger左専用Transformに変更.
				tempEquipping_L.transform.position = Dagger_L_Transform.position;
				tempEquipping_L.transform.rotation = Dagger_L_Transform.transform.rotation;
				break;
			case StaticMyClass.WeaponEquipmentType.Bow:
				Destroy(tempEquipping_L);
				Destroy(tempEquipping_R);
				tempEquipping_L = Instantiate(insObj, EquipmentEndPoint_L);
				tempEquipping_R = Instantiate(EquipmentObjectDataTable.GetGameObject(255), EquipmentEndPoint_R.transform);    //Arrow.
				tempEquipping_R.transform.position = arrowTr.position;
				tempEquipping_R.transform.rotation = arrowTr.rotation;
				bowObjectAnimator = tempEquipping_L.GetComponent<Animator>();
				arrowRenderer = tempEquipping_R.GetComponentInChildren<MeshRenderer>();
				trailRendererManager.ArrowObjectTrailRendere = tempEquipping_R.GetComponent<TrailRenderer>();
				break;
			case StaticMyClass.WeaponEquipmentType.Unarmed:
				Destroy(tempEquipping_L);
				Destroy(tempEquipping_R);
				break;
			default:
				Destroy(tempEquipping_L);
				Destroy(tempEquipping_R);
				break;
		}

		//Townの場合は表示しない.
		if (getScene.GetSceneType() == SceneDefineTable.SceneType.town)
		{
			if (tempEquipping_L != null) tempEquipping_L.SetActive(false);
			if (tempEquipping_R != null) tempEquipping_R.SetActive(false);
		}
	}

	private void EquipObjectArm2(GameObject insObj)
	{
		if (tempEquipping_L != null) Destroy(tempEquipping_L);
		tempEquipping_L = Instantiate(insObj, EquipmentEndPoint_L);
		//Townの場合は表示しない.
		if (getScene.GetSceneType() == SceneDefineTable.SceneType.town)
		{
			if (tempEquipping_L != null) tempEquipping_L.SetActive(false);
			if (tempEquipping_R != null) tempEquipping_R.SetActive(false);
		}
	}


	//TownへのSceneChangeで呼ばれる
	public void ChangeEquipObjectStateInTown()
	{
		//表示されているArm1,2の装備を非表示に変更.
		if (tempEquipping_L != null) tempEquipping_L.SetActive(false);
		if (tempEquipping_R != null) tempEquipping_R.SetActive(false);
	}

	//FieldへのSceneChangeで呼ばれる
	public void ChangeEquipObjectStateInField()
	{
		if (tempEquipping_L != null) tempEquipping_L.SetActive(true);
		if (tempEquipping_R != null) tempEquipping_R.SetActive(true);
	}





	//==========================================BowオブジェクトのAnimator==========================================


	/// <summary>
	/// Bow装備中のそのBowオブジェクトのAnimator.
	/// </summary>
	private Animator bowObjectAnimator { get; set; }
	private MeshRenderer arrowRenderer;     //Arrowのレンダラ

	/// <summary>
	/// アニメーション[2Hand-Bow-Attack1]等から呼ばれるイベント.
	/// BowオブジェクトのAnimation開始.
	/// </summary>
	private void BowAnimationStart()
	{
		Debug.Log("BowAttackStart");
		if (bowObjectAnimator == null) return;
		bowObjectAnimator.SetTrigger("Attack");
		//撃つときのみ親子関係を解消.
		tempRotation = this.transform.rotation.eulerAngles;
	}

	/// <summary>
	/// アニメーション[2Hand-Bow-Attack1]等から呼ばれるイベント.
	/// 矢を放った瞬間.
	/// </summary>
	private void ReleaseArrow()
	{
		if (bowObjectAnimator == null || arrowRenderer == null) return;
		StartCoroutine(Enumerator());
	}

	//矢の表示を消す.
	private IEnumerator Enumerator()
	{
		yield return null;
		Color color = arrowRenderer.material.color;
		arrowRenderer.material.color = new Color(0, 0, 0, 0f);
		yield return new WaitForSeconds(0.8f);
		arrowRenderer.material.color = color;
	}

	/// <summary>
	/// アニメーション[2Hand-Bow-Attack1]等から呼ばれるイベント.
	/// Arrowオブジェクトを飛ばすVer.
	/// </summary>
	/// <param name="moveTime">Arrowオブジェの飛行時間</param>
	private void ReleaseArrowMove(float moveTime)
	{
		if (bowObjectAnimator == null || tempEquipping_R == null) return;
		StartCoroutine(ArrowMove(moveTime));
	}

	private IEnumerator ArrowMove(float moveTime)
	{
		tempEquipping_R.transform.parent = null; //子にすると相対位置変更むずいのでWorldに.
												 //Arrowの向きをまっすぐに.
		tempEquipping_R.transform.rotation = Quaternion.Euler(tempRotation);    //親オブジェと同じ向きにする.
		bool isChecked = false;     //ループ内で何度も入るのを防止

		while (moveTime > 0)
		{
			tempEquipping_R.transform.Translate(Vector3.forward * Time.deltaTime * StaticMyClass.ArrowSpeed);
			yield return null;
			if (moveTime < 20 && !isChecked)
			{
				trailRendererManager.ChangeArrowTrail(false);     //ArrowオブジェのTrailRenderer使う場合,停止はここで.
				isChecked = true;
			}
			moveTime--;

		}

		tempEquipping_R.transform.parent = EquipmentEndPoint_R.transform;
		tempEquipping_R.transform.localPosition = arrowTr.localPosition;
		tempEquipping_R.transform.localRotation = arrowTr.localRotation;

	}



	/// <summary>
	/// Test用
	/// </summary>
	/// <param name="gameObject"></param>
	/// <param name="weapon"></param>
	public void EquipObject(GameObject gameObject, StaticMyClass.WeaponEquipmentType weapon)
	{
		EquipObjectWeapon(gameObject, weapon);
	}

	public void EquipObject(GameObject gameObject, GameObject obj2 = null)
	{
		tempEquipping_L = gameObject;
		if (obj2 != null) tempEquipping_R = obj2;
	}
}

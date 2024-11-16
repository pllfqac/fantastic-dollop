using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// Player.
/// Walk,Run以外のAnimationを管理.
/// </summary>
public class CharaAnimation : MonoBehaviour,ICharaAnimation
{

	//Animation Eventは別スク[CharaAnimationEvent.cs]


	private Animator Animator;
	private ICheckEquiped checkEquiped;

	[System.NonSerialized]
	public IGetSceneType getScene = null;	//Instatiate. or photonCallback.

	//Addressableで呼びたい
	[SerializeField]
	private RuntimeAnimatorController defAnimator = null;		//街用Animator.
	[SerializeField]
	private RuntimeAnimatorController sword = null;
	[SerializeField]
	private RuntimeAnimatorController twoHandSword = null;
	[SerializeField]
	private RuntimeAnimatorController spear = null;
	[SerializeField]
	private RuntimeAnimatorController staff = null;
	[SerializeField]
	private RuntimeAnimatorController dagger = null;
	[SerializeField]
	private RuntimeAnimatorController bow = null;
	[SerializeField]
	private RuntimeAnimatorController unarmed = null;


	void Start()
	{
		Animator = GetComponent<Animator>();
		checkEquiped = GetComponent<ICheckEquiped>();
	}


	//=================Animator Controllerの変更=================



	/// <summary>
	/// SceneChange  or 武器を変更した場合に 呼ばれる
	/// </summary>
	public void ChangeAnimatorController()
	{
		if (getScene.GetSceneType() == SceneDefineTable.SceneType.town) ChangeAnimator(StaticMyClass.WeaponEquipmentType.none);
		else ChangeAnimator(checkEquiped.GetEquippingWeaponType());
	}

	//上記の指定Ver.
	public void ChangeAnimatorController(StaticMyClass.WeaponEquipmentType wType)
	{
		//すでに同じAnimatorを使用していれば何もしない.
		if (Animator.runtimeAnimatorController == GetRuntimeAnimatorController(wType)) return;
		else ChangeAnimator(wType);
	}

	//自端末他キャラ向け.
	public void ChangeAnimationByReceiveWeaponChangeRaise(StaticMyClass.WeaponEquipmentType type)
	{
		if (getScene.GetSceneType() == SceneDefineTable.SceneType.town) ChangeAnimator(StaticMyClass.WeaponEquipmentType.none);
		else ChangeAnimator(type);
	}
	   	  


	/// <summary>
	/// 引数に指定したWeaponEquipmentTypeに対応するAnimatorCtrlをセットする.
	/// </summary>
	/// <param name="weaponType"></param>
	private void ChangeAnimator(StaticMyClass.WeaponEquipmentType weaponType)
	{
		Debug.Log("Change Animator  " + weaponType.ToString());
		Animator.runtimeAnimatorController = GetRuntimeAnimatorController(weaponType);
	}

	/// <summary>
	///　引数に指定したWeaponEquipmentTypeに対応するAnimatorCtrlを返す.
	/// </summary>
	/// <param name="weaponEquipmentType"></param>
	/// <returns></returns>
	private RuntimeAnimatorController GetRuntimeAnimatorController(StaticMyClass.WeaponEquipmentType weaponEquipmentType)
	{
		switch (weaponEquipmentType)
		{
			case StaticMyClass.WeaponEquipmentType.none:return defAnimator;
			case StaticMyClass.WeaponEquipmentType.Sword:return sword;
			case StaticMyClass.WeaponEquipmentType.TwoHandSword:return twoHandSword;
			case StaticMyClass.WeaponEquipmentType.Spear:return spear;
			case StaticMyClass.WeaponEquipmentType.Staff:return staff;
			case StaticMyClass.WeaponEquipmentType.Dagger:return dagger;
			case StaticMyClass.WeaponEquipmentType.Bow:return bow;
			case StaticMyClass.WeaponEquipmentType.Unarmed:return unarmed;
			default:return defAnimator;
		}
	}


	//=================Animationの実行=================


	/// <summary>
	/// SkillのAnimation.
	/// </summary>
	/// <param name="skillAbsNum"></param>
	public void PlaySkillAnimation(int skillAbsNum)
	{
		if (skillAbsNum == 1) Animator.SetTrigger("NormalAttack");
		else Animator.SetTrigger("Skill" + skillAbsNum);
	}

	public void PlaySkillAnimation(int skillAbsNum,int skillLevel)
	{
		try
		{
			// "Skill"+ SkillAbs+"Level" の文字列を組み立てる.
			Animator.SetInteger("Skill" + skillAbsNum + "Level", skillLevel);
		}
		catch (System.Exception)
		{
            return;		//Userは変換失敗のとき何もしない
		}

		if (skillAbsNum == 1) Animator.SetTrigger("NormalAttack");
		else Animator.SetTrigger("Skill" + skillAbsNum);

	}


	public void StartDownAnimation()
	{
		Animator.SetTrigger("Down");
	}

	public void ReceiveDamageAnimation()
	{
		Animator.SetTrigger("ReceiveDamage");
	}

	/// <summary>
	/// DownからidleへのAnimation遷移.
	/// </summary>
	public void Exit()
	{
		Animator.SetTrigger("Exit");
	}

	//Animatorから現在再生中のステートの長さを取得
	public AnimatorStateInfo GetAnimatorInfo()
	{
		return Animator.GetCurrentAnimatorStateInfo(0);
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Player.
/// Walk,Run以外のAnimationの同期.
/// </summary>
public class CharaAnimationSync : MonoBehaviourPun,ICharaSkillAnimationSync
{
	public IGetWeaponTypeBySkillAbs GetWeaponType {private get;  set; }		//PhotonCallback.

	private IAreaCulling areaCulling;
	private ICharaAnimation charaAnimation;

	private void Start()
	{
		areaCulling = GetComponent<IAreaCulling>();
		charaAnimation = GetComponent<ICharaAnimation>();
	}

	/// <summary>
	/// SkillAnimation同期のRPCを飛ばす.同じAreaのみ対象.
	/// </summary>
	public void SkillAnimationSync(int skillAbs,int sLevel)
	{
		photonView.Group = areaCulling.nowGroupId;
		photonView.RPC(nameof(ReceiveSkillAnimationSync), RpcTarget.Others,(byte)skillAbs,(byte)sLevel);
	}


	[PunRPC]
	private void ReceiveSkillAnimationSync(byte skillAbs,byte sLevel)
	{
        Debug.Log("SkillAnimationSYnc!  Abs:" + skillAbs + "  sLevel:" + sLevel);
		if (skillAbs == 1)
		{
			charaAnimation.PlaySkillAnimation(skillAbs);
		}
		else
		{
			var weaponType = GetWeaponType.GetWeaponType(skillAbs);
			if (weaponType == StaticMyClass.WeaponEquipmentType.none) return;
			charaAnimation.ChangeAnimatorController(weaponType);
			charaAnimation.PlaySkillAnimation(skillAbs,sLevel);
		}
	}

	/// <summary>
	/// 他端末から送られてきた装備同期Raiseで武器の変更があったとき.
	/// </summary>
	/// <param name="type">装備していなければ Unarmed</param>
	public void SetSyncAnimationByWeaponType(StaticMyClass.WeaponEquipmentType type)
	{
		charaAnimation.ChangeAnimationByReceiveWeaponChangeRaise(type);
	}
}

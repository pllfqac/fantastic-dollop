using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public interface ICharaAnimation
{
	/// <summary>
	/// SceneChange or 武器を変更した場合に呼ばれる.
	/// </summary>
	void ChangeAnimatorController();

	//上記の引数指定タイプ.
	void ChangeAnimatorController(StaticMyClass.WeaponEquipmentType type);

	/// <summary>
	/// 自端末他キャラ用.
	/// 武器同期Raiseで呼ばれる.
	/// </summary>
	/// <param name="type">装備なしならnone.実装先で街かどうかよしなに</param>
	void ChangeAnimationByReceiveWeaponChangeRaise(StaticMyClass.WeaponEquipmentType type);

	void StartDownAnimation();		//HP0になった時のAnimation.
	void ReceiveDamageAnimation();  //クリティカルHitのダメージを受けた時のAnimation.
	void Exit();            //Animationを抜けてIdle状態にする.
	void PlaySkillAnimation(int skillAbsNum);
	void PlaySkillAnimation(int skillAbsNum,int skillLevel);    //SkillLevelでAnimationのSetInteger弄るとき.

	
}

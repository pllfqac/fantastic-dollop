using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharaSkillAnimationSync
{

	IGetWeaponTypeBySkillAbs GetWeaponType {  set; }


	/// <summary>
	/// SkillのAnimation同期のRPCを飛ばす.
	/// 同じAreaのみ対象.
	/// </summary>
	void SkillAnimationSync(int skillAbs,int skillLevel);


}

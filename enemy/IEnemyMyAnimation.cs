using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMyAnimation 
{

	void AttackAnimationPlay();
	void DeadAnimationPlay();

	void MoveAnimation(float speed);

}

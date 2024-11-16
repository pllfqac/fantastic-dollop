using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Enemy.
/// EnemyのAnimation再生を制御.
/// </summary>
public class EnemyMyAnimation : MonoBehaviour, IEnemyMyAnimation
{


	private Animator Animator;


	void Awake()
	{
		Animator = GetComponent<Animator>();
	}


	public void AttackAnimationPlay()
	{
		Animator.SetTrigger("Attack");
	}

	public void DeadAnimationPlay()
	{
		Animator.SetTrigger("Down");
	}


	/// <summary>
	/// Down後のanimationEventで呼ばれる
	/// DownからidleへのAnimation遷移.
	/// </summary>
	public void DownAnimationEndHandler()
	{
		Debug.Log("DownAnimationEndHandler()");
		transform.position = new Vector3(0, 100, 0);        //重力はUPdate無効で無くしてる.
		Debug.Log("DownAnimation  Exit");
		Animator.SetTrigger("Exit");
	}



    public void MoveAnimation(float speed)
    {
		Animator.SetFloat("Speed", speed);
    }
}

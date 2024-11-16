using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Enemy.
/// EnemyNodeへの参照.
/// </summary>
public class EnemyNodeRef : MonoBehaviour
{

	[System.NonSerialized]
	public EnemyNode MyNode = null;   //Instantiate.


	//Targetアイコンの表示切替.
	public void ToggleTargetIcon(bool isShow)
	{
		MyNode.ToggleHitRayTarget(isShow);
	}
}

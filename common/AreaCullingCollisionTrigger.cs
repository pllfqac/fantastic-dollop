using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Player->CullingAreaChangeTrigger.
/// </summary>
public class AreaCullingCollisionTrigger : MonoBehaviour
{
	private IAreaCulling areaCulling;

	private void Awake()
	{
		Debug.Log("start acc");
		areaCulling = transform.root.GetComponent<IAreaCulling>();
	}

	//boxCollider境界との子リジョン
	private void OnTriggerEnter(Collider other)
	{
	//	Debug.Log("OTE AC");
		if (other.tag == "Player") return;
		areaCulling.CullingAreaOTEnter(other);
	}

	//boxCollider境界との子リジョン
	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player") return;
		areaCulling.CullingAreaOTExit(other);
	}

}

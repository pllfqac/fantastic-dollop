using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Canvas/EnemyCanvas.
/// Enemyの頭上Nodeの生成と管理.
/// </summary>
public class EnemyCanvas : MonoBehaviour
{

	[SerializeField]
	private GameObject enemyNode = null;       //Prefabs.

	//   private List<GameObject> nodeList = new List<GameObject>();     //生成したNodeのList.

	//Nodeの作成.
	public void CreateEnemyNode(Transform tr, string eName, float wariai)
	{
		Debug.Log("CreateEnemyNode");
		GameObject node = Instantiate(enemyNode) as GameObject;
		node.transform.SetParent(this.gameObject.transform, false);
		EnemyNode en = node.GetComponent<EnemyNode>();
		en.Target = tr;
		en.SetName(eName);
		en.ChangeHpValue(wariai);   //HPバー初期化.

		//     nodeList.Add(node);
		//NodeのHPバー更新メソッドの参照をEnemyオブジェクト側に持たせる.(別にnodeの参照を持たせてもいいが)
		tr.GetComponent<HitPointCalculation>().ChangeEnemyHP_Event += en.ChangeHpValue;
		//状態異常アイコンの表示先参照の取得.
		tr.GetComponent<IUnusualCondition>().UcPanel = en.ucPanel;
		//Enemy用Damage表示オブジェへの参照をEnemyオブジェクト側に持たせる.
		tr.GetComponent<HitPointCalculation>().DamageUIAnimationCtrl = en.DamageUIAnimationCtrl;
		tr.GetComponent<ResultValueQueue>().DamageUIAnimationCtrl = en.DamageUIAnimationCtrl;
		en.DamageUIAnimationCtrl.particleAction = tr.transform.Find("Particles_Common").GetComponent<IParticleAction>();

		//EnemyのSearchAreaにPlayerが入ったときのみEnemyNodeを表示するように.
		tr.Find("SearchArea").GetComponent<EnemySearchArea>().EnemyNodeSetActiveEvent += en.ShowToggleEnemyNode;
		tr.GetComponent<EnemyNodeRef>().MyNode = en;
	}

}

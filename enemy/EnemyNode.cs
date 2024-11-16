using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
/// <summary>
/// Prefabs.Node.生成後はEnemyCanvas配下.
/// Enemy生成で自動的に頭上に生成される.
/// </summary>
public class EnemyNode : MonoBehaviour {

	public Transform Target;        //EnemyのTr.
	private Vector3 screenPoint;

	[SerializeField]
	private Text nameText = null;  //Nameが入る.

	[SerializeField]
	private Slider hpSlider = null;

	public GameObject ucPanel;       //inspe.状態異常のアイコンをまとめるパネル.

	//[SerializeField]
	public DamageUIAnimationCtrl DamageUIAnimationCtrl = null;

	//hitRayTargetの対象にしたときのIcon
	[SerializeField]
	private Transform hitRayTargetImage = null;

	//Fade用
	[SerializeField]
	private Image sliderFill;
	[SerializeField]
	private Image sliderBackGround;



	private void Start()
	{
		if (this.gameObject.activeSelf) ShowToggleEnemyNode(false);   //最初は消しておく
	}

	   private void LateUpdate()
	   {
		   if (Target == null)     //回線切断とか.
		   {
			   Debug.Log("EnemyNode Destroy ");
			   Destroy(this.gameObject);
			   Target = null;
			   return;
		   }
		   //Mainカメラで撮影されているターゲットのPositionをスクリーン座標に変換.
		   screenPoint = Camera.main.WorldToScreenPoint(Target.position);
		   //得られたスクリーン座標のXY値をそのままUI TextのPositionに代入.
		   transform.position = new Vector3(screenPoint.x, screenPoint.y+30, 0);
	   }


	//HP SliderのValue変更.引数には0～1.
	public void ChangeHpValue(float wariai)
	{
		Debug.Log("HP割合: " + wariai + "    by EnemyNode");
		if (this.gameObject.activeSelf) hpSlider.value = wariai;
	}

    public void SetName(string name)
    {
        nameText.text = name;
    }

    //状態異常になったとき,対応するアイコンをuscImagePanelの子で表示する.
    public void SetUcIcon()
    {

    }

/*	public void GeneratDamageUI(IReadOnlyList<int?> showDamage)
	{
		if (this.gameObject.activeSelf) DamageUIAnimationCtrl.StartDamageUIAnimation(showDamage);
	}*/




	/// <summary>
	/// EnemyがPlayerへ近づいたときのみEnemyNodeを表示する.
	/// </summary>
	/// <param name="b">PlayerとOTEnterしたときTrue.OTExitでfalse.</param>
	public void ShowToggleEnemyNode(bool b)
	{
		//this.gameObject.SetActive(b);
		if (b)
		{
			//非Activeならコルーチン動かす前にActiveに
			this.gameObject.SetActive(true);
			StartCoroutine(FadeInCoroutine());
		}
		else
		{
			if(this.gameObject.activeSelf)
			StartCoroutine(FadeOutCoroutine());
		}
	}


	//NodeのFade

	/// <summary>
	/// 次第にはっきりと.
	/// </summary>
	private IEnumerator FadeInCoroutine()
	{
		Color c = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		for (float f = 0; f <= 1; f += 0.05f)
		{
			c.a = f;
			ChangeAlpha(c);
			yield return null;
		}

		c.a = 1;        //アルファを1にすると非透明になる.
		ChangeAlpha(c);
	}

	/// <summary>
	/// 次第に消える.
	/// </summary>
	private IEnumerator FadeOutCoroutine()
	{
		Color c = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		for (float f = 1f; f >= 0; f -= 0.05f)
		{
			c.a = f;
			ChangeAlpha(c);
			yield return null;
		}
		c.a = 0;
		ChangeAlpha(c);
		//this.gameObject.SetActive(false);       //SetActive(false)にするのは全コルーチンが終わってから
		DamageUIAnimationCtrl.WaitDamageUIAnimation();
	}


	private void ChangeAlpha(Color color)
	{
		nameText.color = color;
		sliderFill.color = color;
	//	ucPanelImage.color = color;
		sliderBackGround.color = color;
	}

	//hitRayTargetのオンオフ.Enemy経由で呼ばれる.
	public void ToggleHitRayTarget(bool isShow)
	{
		hitRayTargetImage.gameObject.SetActive(isShow);
	}
}

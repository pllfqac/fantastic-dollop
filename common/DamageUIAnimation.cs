using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Damage表示用Text.
/// Damageと命名したけど回復等も扱う.
/// Player->PlayerNameTarget->PlayerNameTextNode
/// </summary>
public class DamageUIAnimation : MonoBehaviour
{
	/// <summary>
	/// 使用中でTrue.
	/// </summary>
	[System.NonSerialized]
	public bool used;
	private Text text;		//Damage表示用.

	public AnimationCurve curve;    // アニメーションカーブ
	private CanvasRenderer canvasRenderer;  // CanvasRendererコンポーネント
	private RectTransform myRectPos;    //このオブジェのRectTr.
	private RectTransform parentRectTr; //一つ上の親を取得

	private void Start()
	{
		text = GetComponent<Text>();
		myRectPos = GetComponent<RectTransform>();
		canvasRenderer = GetComponent<CanvasRenderer>();
		canvasRenderer.SetAlpha(0f);       // 透明にしておく
		parentRectTr = transform.parent.GetComponent<RectTransform>();
	}


	/// <summary>
	/// Damage or 回復の数字をTextで表示する.
	/// Damageは正の数.回復は負の数.
	/// </summary>
	/// <param name="showDamageValue">Damageは正の数.回復は負の数.</param>
	/// <param name="isMine">自キャラが与えたDamage,ReviのときTrue.サイズに差をつけるため.</param>
	public void StartDamageTextAnimation(int showDamageValue,bool isMine)
	{
		used = true;
		SelectUIColor(showDamageValue);
		int fontSize = isMine ? StaticMyClass.IsMineShowFontSize : StaticMyClass.StandardShowFontSize;
		text.fontSize = fontSize;
		Debug.Log("font Size:" + fontSize);
		//	text.fontSize = isMine ? StaticMyClass.IsMineShowFontSize : StaticMyClass.StandardShowFontSize;
		text.text = Math.Abs(showDamageValue).ToString();  //ここで負の数も正に
		StartCoroutine(DamageTextAnimation());
	}

	/// <summary>
	/// 文字表示Ver.
	/// </summary>
	/// <param name="showDamageStr">表示したい文字</param>
	public void StartDamageTextAnimation(string showDamageStr)
	{
		used = true;
		text.color = StaticMyClass.StrTextColor;
		text.text = showDamageStr;
		StartCoroutine(DamageTextAnimation());
	}

	/// <summary>
	/// Critical,Comboイメージ用.
	/// AnimationCurveだけ使用したい.
	/// </summary>
	public void StartCriticalImage()
    {
		if (used) return;		//もし使用中なら表示はあきらめる.
		used = true;
		StartCoroutine(DamageTextAnimation());
    }

	/// <summary>
	/// damage,回復でTextの色を変更する.
	/// </summary>
	/// <param name="value">受けたDamage or 回復</param>
	private void SelectUIColor(int value)
	{
		if (value > 0) text.color = StaticMyClass.damageTextColor;
		else if (value == 0) text.color = StaticMyClass.StrTextColor;
		else text.color = StaticMyClass.reviTextColor;
	}


	private IEnumerator DamageTextAnimation()
	{
		float startTime = Time.time;
		while (Time.time - startTime < StaticMyClass.duration)
		{
			//Evaluate()	評価したいカーブ内の時間のカーブの値を返す
			float curveValue = curve.Evaluate((Time.time - startTime) / StaticMyClass.duration);
			// アニメーションカーブから取得した値を使って、イメージの位置と透明度を変更する
			canvasRenderer.SetAlpha(curveValue);
			myRectPos.position = new Vector2(parentRectTr.position.x, parentRectTr.position.y+ curveValue * StaticMyClass.positionCoefficient);
			if (used == false)
			{
				Debug.Log("SetAlpha(0)");
				canvasRenderer.SetAlpha(0);
				yield break;
			}
			yield return null;
		}
		myRectPos.position = parentRectTr.position;
		canvasRenderer.SetAlpha(0);
		used = false;
	}


}

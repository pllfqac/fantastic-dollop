using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MainScene->Canvas->SliderでOnOffした時にその表示を変えたい箇所複数.
/// </summary>
public class SliderTextChange : MonoBehaviour
{
	[SerializeField]
	private Text targetSettingSliderText = null;        //OnかOffかの表示変更対象のテキスト.

	/// <summary>
	/// スライダーの変更で呼ばれる.Slider上の"On","Off"テキストを変更する.
	/// 0のときOn,1のときOffとする.
	/// </summary>
	public void OnTextChangeByChangeSliderValue(float value)
	{
		Debug.Log("Value Change:" + value);
		if (value == 0) targetSettingSliderText.text = "On";
		else targetSettingSliderText.text = "Off";
	}

}

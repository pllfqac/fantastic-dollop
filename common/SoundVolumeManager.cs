using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Canvas->Configrations->OtherSettingPanel.
/// SoundのVolume設定.
/// </summary>
public class SoundVolumeManager : MonoBehaviour
{
	[SerializeField]
	private AudioSource AudioSource = null;

	//Sliderの一番右が最大で左が無音とする

	//Sliderを動かしたときに呼ばれる.
	public void OnSoundVolumeSetting(float value)
	{
		if (AudioSource == null) return;

		Debug.Log("Sound Volume Change :" + value);
		if (value == 0) AudioSource.mute = true;
		else
		{
			AudioSource.mute = false;
			AudioSource.volume = value;
		}
	}
}

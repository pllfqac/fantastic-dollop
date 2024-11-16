using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void PutAndroidBackButtonDel();



/// <summary>
/// Single1.
/// Android実機の戻るボタン処理クラス 
/// </summary>
public class OnAndroidButton : MonoBehaviour,IOnAndroidButton {

    public event PutAndroidBackButtonDel androidBackButtonEvent;


	//Title,NewUniState,Main以降　の各Sceneにおけるcanvasの確認表示パネル参照をその都度保持  各シーンのManagerがこの参照に入れる
	//Miainシーン以降はConfigurationPanel出す


	void Update () {
		Androidbutton_check();
	}

    private void Androidbutton_check()
    {
        //戻るボタン等が押されるとゲーム確認表示を出す     
        if ((Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Menu)))
        {
            Debug.Log("戻るボタンが押されました");
            if (androidBackButtonEvent != null) androidBackButtonEvent();

        }
    }
    
}
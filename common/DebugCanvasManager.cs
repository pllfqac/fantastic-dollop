using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

/// <summary>
/// debug用のCanvas管理スク
/// </summary>
public class DebugCanvasManager : MonoBehaviour {

    /// <summary>
    /// Trueでログ出力をOFFに
    /// </summary>
    [SerializeField]
    private bool DisableAllLog;


    public GameObject scrolleview;  //inspe
    public TextMeshProUGUI scrolleViewText;    //inspe
    public GameObject enemyDataPanel;   //inspe
    public DebugEnemuScrollCtrl debugEnemuCtrl = null;      //inspe
    
    [SerializeField]
    private int textSize = 10;      //文字サイズ Inspecter設定用.

    public GameObject myPlayer;     //Instantiateで自身のキャラの参照を得る
    private bool showTextBool;      //Trueで表示.

	void Start () {
        //実行時のプラットフォーム判断
        if (Application.platform == RuntimePlatform.Android)
        {
            var cls = new AndroidJavaClass("android.os.Build$VERSION");
            var apiLevel = cls.GetStatic<int>("SDK_INT");
            Debug.Log("<color=blue>Android Version :" + apiLevel+"</color>");

            Debug.Log("Log出力する? " + !DisableAllLog);

             Debug.unityLogger.logEnabled = !DisableAllLog;
        }

        scrolleViewText.fontSize = textSize;
        Debug.Log("Debug Canvas 文字サイズ: " + textSize);

		//シーンが遷移したことを検知する.DebugCanvasと他のCanvasの描画順の都合がつかないのでSceneによりDebugCanvas内一部オブジェ非表示にする.
		SceneManager.activeSceneChanged += ChangedActiveScene;
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("debug_Canvasのソート順を変更!        ");
            //debug_Canvasのソート順を変更する.debug_Canvasのデフォは-1
            int order = this.GetComponent<Canvas>().sortingOrder;
            if (order == -1) this.GetComponent<Canvas>().sortingOrder = 3;      //数値は適当
            else this.GetComponent<Canvas>().sortingOrder = -1;
        }
    }



    //debug用canvas表示 非表示切替
    public void OnShowDebugToggle(bool toggle)
    {
        if (toggle)
        {
			//Debug.unityLogger.logEnabled = true;
            scrolleViewText.enabled = true;
            scrolleview.SetActive(true);
            enemyDataPanel.SetActive(true);			
        }
        else
        {
            scrolleViewText.enabled = false;
            scrolleview.SetActive(false);
            enemyDataPanel.SetActive(false);
			//Debug.unityLogger.logEnabled = false;

		}
	}

    public bool IsShowDebugToggle
    {
        set
        {
            showTextBool = value;
            OnShowDebugToggle(showTextBool);   
        }
    }


    public void OnMainSceneUpdataButton()
    {
        scrolleViewText.text = "";      //Textを消す
        if (myPlayer == null) return;
  //      m_photonview = myPlayer.GetComponent<PhotonView>();
        string str = PhotonNetwork.NetworkClientState.ToString();
        Debug.Log(str);
    }


	public void ChangedActiveScene(Scene current, Scene next)
	{
		Debug.LogWarning("Sceneが変わりましたコールバック      "+current.name+"→"+next.name);    //current　はAdditiveでないと使えないっぽい?


		//charaStyleのtoggleが押せないので消す.
		if (next.name == "NewUniState_scene") this.gameObject.SetActive(false);		//	enemyDataPanel.SetActive(false);
		if(next.name== "Main_Scene" && !this.gameObject.activeSelf)  //Main_Sceneに入って一度のみ処理
		{
			Debug.Log("<color=blue>ChangedActiveScene</color>");
			//enemyDataPanel.SetActive(true);
			this.gameObject.SetActive(true);
			SceneManager.activeSceneChanged -= ChangedActiveScene;
		}
	}


    


/*
    //ドロップダウンでどれかが選択された時
    public void OnDropDown(Dropdown  i)
    {
        Debug.Log("dropDown" + i.value);
        switch (i.value)
        {
            case 0:dialog.editerMDN = "09011111111";
                break;
            case 1:dialog.editerMDN = "09011112222";
                break;
            case 2:dialog.editerMDN = "09011113333";
                break;
            default:dialog.editerMDN = "09012345678";
                break;
        }
    }
    
    */



    /*
    [SerializeField]
    private Text m_strategy = null;

    private Color[] PLAYER_COLOR = new Color[] { Color.white, Color.red, Color.green, Color.blue, Color.yellow };

    public void SetRoomStrategy(string i_strategy)
    {
        var properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add("Strategy", i_strategy);
        properties.Add("Sender", PhotonNetwork.player.ID);

        PhotonNetwork.room.SetCustomProperties(properties);
    }

    public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable i_propertiesThatChanged)
    {
        {
            object value = null;
            if (i_propertiesThatChanged.TryGetValue("Strategy", out value))
            {
                m_strategy.text = (string)value;
            }

        }

        {
            object value = null;
            if (i_propertiesThatChanged.TryGetValue("Sender", out value))
            {
                m_strategy.color = PLAYER_COLOR[(int)value];
            }

        }
    }
    */
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

public delegate void QuitNoDel();


public class QuitGamePanelCtrl : MonoBehaviour {

    public event QuitNoDel quitNoEvent;

    //連打防止用
    [SerializeField]
    private Button quitButton;     
    [SerializeField]
    private Button nonQuitButton;

    private void OnEnable()
    {
        quitButton.interactable = true;
        nonQuitButton.interactable = true;
    }

    //QuitGamePanelのYesボタンが押されたとき
    public async void OnQuitGame_Yes()
    {
        quitButton.interactable = false;
        nonQuitButton.interactable = false;
        Debug.Log("OnQuitGame_Yes");
        IQuitGame quit = new QuitGame();
        //mainScene以降のみ登録
        GameObject single2 = GameObject.FindWithTag("single2");
        if (single2 != null)
        {
            quit.UserSaveEvent += single2.GetComponent<IUpdateKnowledge>().UpdateKnowledgeTable;
        }

       // GameObject single3 = GameObject.FindWithTag("single3");
        //GameObject.FindWithTag()で見つからない時(=MainScene以前のScene)はnull.何もしない.
        //if (single3 != null) quit.UserSaveEvent += single3.GetComponent<Agriculture>().SaveAgricultureParms;    //Userが行うLogout時のSave処理
        await quit.QuitApplication();     //Game終了処理へ
    }

    //QuitGamePanelのNoボタンが押されたとき
    public void OnQuitGame_No()
    {
        quitButton.interactable = false;
        nonQuitButton.interactable = false;
        this.gameObject.SetActive(false);
        if (quitNoEvent != null)
        {
            quitNoEvent();
        }
    }
}

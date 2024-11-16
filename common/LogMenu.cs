using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

//debug_Canvasの方の実機デバック用
public class LogMenu : MonoBehaviour
{

    public ScrollRect scrollRect;   //inspe

    private int newLineOverValue = 150;

    [SerializeField]
    private TextMeshProUGUI m_textUI = null;


    private void Start()
    {
#if UNITY_EDITOR 
        return;
#endif
        Application.logMessageReceived += OnLogMessage;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= OnLogMessage;
    }


    //Application.logMessageReceivedに登録したコールバックの引数にLogTypeが渡されている  このLogTypeで色分け
    private void OnLogMessage(string i_logText, string i_stackTrace, LogType i_type)
    {
        if (string.IsNullOrEmpty(i_logText) || m_textUI == null)
        {
            return;
        }

        if (i_logText.Length > newLineOverValue) NewLine(i_logText);      //150字超えた場合.
        else m_textUI.text += " " + i_logText + System.Environment.NewLine;  //改行コードも一緒に渡
        /*       switch (i_type)
               {
                   case LogType.Error:
                       i_logText = string.Format("<size=40>{0}</size>", i_logText);
                       break;
                   case LogType.Assert:
                   case LogType.Exception:
                       i_logText = string.Format("<color=red>{0}</color>", i_logText);
                       break;
                   case LogType.Warning:
                       i_logText = string.Format("<color=yellow>{0}</color>", i_logText);
                       break;
                   default:
                       break;
               }
       */
        //追記
        //   i_logText= MyHelper.DebugLog(i_logText);
        CheckTextLength();

        scrollRect.verticalNormalizedPosition = 0;

    }

    //指定文字数超えたら改行して表示.
    private void NewLine(string log)
    {
        string nokori = log;

        while (nokori.Length >= 0)
        {
            //150文字以降から末尾までの文字列を取得.150字未満は例外で処理.
            try
            {
                string tempStr = nokori.Substring(0, newLineOverValue); //0～150の文字列を取得.
                m_textUI.text += " " + tempStr + System.Environment.NewLine;
                scrollRect.verticalNormalizedPosition = 0;
                nokori = nokori.Substring(newLineOverValue);
            }
            catch (ArgumentOutOfRangeException)
            {
                m_textUI.text += " " + nokori + System.Environment.NewLine;
                scrollRect.verticalNormalizedPosition = 0;
                break;
            }
        }
        return;
    }


    //Textの最大文字数を超えないように一万文字超えたら消去する
    private void CheckTextLength()
    {
        if (m_textUI.text.Length > 10000)
        {
            m_textUI.text = "";
            Debug.Log("debugCanvasの文字数が最大に達したため消去しました");
        }
    }
}
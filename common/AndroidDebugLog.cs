using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//実機での表示・確認用
public class AndroidDebugLog : MonoBehaviour
{

    public int showNum;     // 直近の保存数
    private int logNum;     //Logの出力ごとに増やす
    Rect rect;
    GUIStyle style;

    // ログの記録
    private List<string> logMsg = new List<string>();

    // Use this for initialization
    void Start()
    {
        logNum = 0;
        rect = new Rect(0, 0, Screen.width, Screen.height);
        // フォントサイズ 色の指定
        style = new GUIStyle();
        style.fontSize = 35;
        style.fontStyle = FontStyle.Normal;
        style.normal.textColor = Color.black;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        style.fontSize = 15;
#endif
        
        android_Log("Start");
    }


    // ログの出力
    public void android_Log(string message)
    {
        ++logNum;
        log(" No. " + logNum + " : " + message);
    }

    private void log(string msg)
    {

        logMsg.Add(msg);
        // 直近の数件のみ保存する
        if (logMsg.Count > showNum)
        {
            logMsg.RemoveAt(0);
        }
    }

    // 記録されたログを画面出力する OnGUIは毎フレーム呼ばれるらしい
    void OnGUI()
    {
        // 出力された文字列を改行でつなぐ
        string outMessage = "";
        foreach (string msg in logMsg)
        {
            outMessage += msg + System.Environment.NewLine;
        }

        // 改行でつないだログメッセージを画面に出す
        GUI.Label(rect, outMessage, style);
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

/// <summary>
/// Single1.
/// チュートリアル.
/// 操作説明など.
/// </summary>
public class Tutorial : MonoBehaviour
{

    //チュートリアル済みかの記録はPlayerPrefsでする. key:"tu". Int型でbitによるフラグ判定.
    //例)TU1=0b0001  => 

    /// <summary>
    /// 各チュートリアルをビットflagとして表した列挙型.
    /// </summary>
    [Flags]
    public enum TutorialFlag
    {


        LSB_1st=1,
        LSB_2nd=2,
        LSB_3th=4,
        LSB_4th=8,
    }


    /// <summary>
    /// 指定したチュートリアルが終了してるか確認する.
    /// </summary>
    /// <returns>終了してるならTrue.</returns>
    public bool CheckFinish(TutorialFlag checkFlag)
    {
        int flagInt = PlayerPrefs.GetInt("tu");
        //数字からenum型へ変換
        TutorialFlag flag = (TutorialFlag)Enum.ToObject(typeof(TutorialFlag), flagInt);
        bool result = (flag & checkFlag) == checkFlag;
        Debug.Log(checkFlag.ToString() + "  チュートリアルが終了? " + result.ToString());
        return result;
    }

    /// <summary>
    /// チュートリアルFlagを取得する.
    /// </summary>
    /// <returns></returns>
    public TutorialFlag GetFlag()
    {
        int flagInt = PlayerPrefs.GetInt("tu");
        //数字からenum型へ変換
        TutorialFlag flag = (TutorialFlag)Enum.ToObject(typeof(TutorialFlag), flagInt);
        Debug.Log("チュートリアルFlag取得:" + flag.ToString());
        return flag;
    }


    /// <summary>
    /// チュートリアルの開始.
    /// </summary>
    /// <param name="targetFlag">開始するチュートリアル.</param>
    public async Task StartTutorial(TutorialFlag targetFlag)
    {
        Debug.Log("チュートリアル " + targetFlag.ToString() + "　を開始します");

        //チュートリアル終了まで待機.
        await Task.Delay(3000);     
        FinishTutorial(targetFlag);
    }

    /// <summary>
    /// チュートリアルを終えた時呼ばれる.
    /// </summary>
    /// <param name="endFlag">終えたチュートリアル.</param>
    public void FinishTutorial(TutorialFlag endFlag)
    {
        int flagInt = PlayerPrefs.GetInt("tu");
        //数字からenum型へ変換
        TutorialFlag flag=(TutorialFlag)Enum.ToObject(typeof(TutorialFlag), flagInt);
        Debug.Log("現在のチュートリアル状況:" + flag.ToString());
        //ビットフラグを追加
        flag = flag | endFlag;
        //PlayerPrefsに記録
        PlayerPrefs.SetInt("tu", (int)flag);
        PlayerPrefs.Save();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Single2.
/// アプリがバックグラウンドにいるかを監視するクラス
/// </summary>
public class ApplicationBackgroundObserver : MonoBehaviour
{

    //バックグラウンドに行っているか
    private bool _isBackground = false;

    //アプリがバックグラウンドにいるかのステータスを変更された時のイベント
    public event Action<bool> ChangedBackgroundStatus;// = delegate { };

    private void Start()
    {
        //イベントに処理登録
        ChangedBackgroundStatus += OnChangeBackgroundStatus;
    }

    //アプリがバックグラウンドにいるかのステータスが変わった
    private void OnChangeBackgroundStatus(bool isBackground)
    {
        if (isBackground)
        {
            Debug.Log($"アプリがバックグラウンドへ");
        }
        else
        {
            Debug.Log($"アプリがバックグラウンドから復帰");
        }
    }

    //=================================================================================
    //バックグラウンドにいるかのステータス切り替え
    //=================================================================================

    private void OnApplicationPause(bool pauseStatus)
    {
        ChangeBackgroundStatus(pauseStatus);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        ChangeBackgroundStatus(!hasFocus);
    }

    //アプリがバックグラウンドにいるかのステータスを変更
    private void ChangeBackgroundStatus(bool isBackground)
    {
        if (isBackground == _isBackground)
        {
            return;
        }
        _isBackground = isBackground;
        ChangedBackgroundStatus(_isBackground);
    }
}

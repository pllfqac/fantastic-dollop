using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// LSBMapCanvas or Canvas-ChangeCameraButton.
/// 通常カメラとLSBMapカメラの手動切替のUI層処理.
/// </summary>
public class UIChangeCamera : MonoBehaviour
{
    /// <summary>
    /// LSBMapCameraへの切替ボタンのイベント.
    /// </summary>
    public void OnChangeToMapCamera()
    {
        GameObject.FindWithTag("single2").GetComponent<IChangeCamera>().ChangeToLSBCamera();
    }

    /// <summary>
    /// MainCameraへの切替ボタンのイベント.
    /// </summary>
    public void OnChangeToMainCamera()
    {
        GameObject.FindWithTag("single2").GetComponent<IChangeCamera>().ChangeToMainCamera();
    }
}

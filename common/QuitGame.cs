using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Realtime;
using Photon.Pun;
using System.Threading.Tasks;

public delegate Task UserSaveDelegate();

public class QuitGame :IQuitGame
{
    /// <summary>
    /// LogoutでUserが行うDBSave処理.
    /// </summary>
    public event UserSaveDelegate UserSaveEvent;

    /// <summary>
    /// とりま処理は待つのでawait.
    /// </summary>
    /// <param name="disconPhoton"></param>
    public async Task QuitApplication()
    {
        Debug.Log("Quit");
        if (UserSaveEvent != null) await UserSaveEvent();

        PhotonNetwork.Disconnect();      //photon未接続にする.OnLeftRoom()が呼ばれる
        Application.Quit(); //アプリケーションを終了します
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

/// <summary>
/// Single2.
/// Scene切替時のEnemyのPhotonViewIDの取得とEnemyの生成&HP同期.
/// </summary>
public class EnemySetPhotonViewId : MonoBehaviour, IEnemySetPhotonViewId
{

    //EnemyにPhotonViewを割り当てる.
    public void SetPhotonView(GameObject obj, int? viewId)
    {
        // obj.AddComponent<PhotonView>();
        if (!PhotonNetwork.IsMasterClient) obj.GetComponent<PhotonView>().ViewID = (int)viewId;      //UserのみviewIDを割り当て
    }


}
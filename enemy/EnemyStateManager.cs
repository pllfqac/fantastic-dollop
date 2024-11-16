using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Enemy.状態変化,Enemyの再出現の管理.
/// </summary>
public class EnemyStateManager : MonoBehaviour, IEnemyStateManager
{

    private EnemyEasyAI eEasyAi;
    private IEnemyMyAnimation eAnimation;
    private IEndUccCondition endUccCondition;
    private ResultValueQueue ResultValueQueue;

    private void Awake()
    {
        eEasyAi = GetComponent<EnemyEasyAI>();
        eAnimation = GetComponent<IEnemyMyAnimation>();
        ResultValueQueue = GetComponent<ResultValueQueue>();
    }

    private void Start()
    {
        //      view = this.GetComponent<PhotonView>();         //MasterがEnemy生成するときViewはaddで後付けなのでAwakeでは取れない.
        if (PhotonNetwork.IsMasterClient) endUccCondition = GetComponent<IEndUccCondition>();
        else endUccCondition = GetComponent<IEndUccCondition>();

    }

    public void HP0()
    {
        ResultValueQueue.ClearBufferQueue();
        endUccCondition.ForceEndUCondition();
        eAnimation.DeadAnimationPlay();
        eEasyAi.DiadEnemy();
    }

}

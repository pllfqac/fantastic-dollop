using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockEnemyEasyAI : MonoBehaviour, IMockEnemyEasyAI
{
   // private EnemyEasyAI easyAi;

    private void Start()
    {
      //  easyAi = GetComponent<EnemyEasyAI>();
    }


    public Transform SyncEnemyStateRPC(Transform target, Transform attackedTarget)
    {
    //        Debug.Log("発見!!");
            return target;
     }

}

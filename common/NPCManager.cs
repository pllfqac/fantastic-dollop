using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Single2.
/// NPC Tapで呼ばれる.
/// NPCとの会話とかシナリオイベントとか.
/// </summary>
public class NPCManager : MonoBehaviour
{

    /// <summary>
    /// NPCタグ別のシナリオ、会話定義表.
    /// </summary>
    [SerializeField]
    private NpcTagAndScenarioDefineTable npcTable = null;



    /// <summary>
    /// NPCをTapした時,ShootRayで呼ばれる.
    /// </summary>
    /// <param name="tag">TapしたNPCのTag.</param>
    public void HitRayNPC(string tag)
    {
        Debug.Log("NPC tag:" + tag);
        GetComponent<IChapterProgress>().StartNPC(npcTable.GetNpcData(tag));
    }


    
}

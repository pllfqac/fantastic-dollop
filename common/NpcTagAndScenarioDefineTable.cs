using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// ScriptableObject.
/// NPCにつけたTag別のシナリオターゲット&会話定義表
/// </summary>
[CreateAssetMenu(menuName = "MyGame/Create NPCTable", fileName = "NpcTable")]

public class NpcTagAndScenarioDefineTable : ScriptableObject
{

    [SerializeField]
    private List<NpcData> npcDatas;



    public NpcData GetNpcData(string tag)
    {
        if (npcDatas.Any(x => x.Tag == tag)) return npcDatas.First(x => x.Tag == tag);
        else return new NpcData();          //もし定義していなければnullのを送る.
    }
}

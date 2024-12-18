using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public interface IComposition
{

    /// <summary>
    /// Itemの合成.
    /// </summary>
    /// <param name="receiveData">Userが選択した合成の原料とKnowledgeLevelの各文字列をsimpleClassにまとめてjsonした文字列.</param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task CompositionItem(string receiveData, int userId);

    
}
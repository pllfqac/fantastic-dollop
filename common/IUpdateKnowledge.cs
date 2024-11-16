using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// KnowledgeのUpdate.
/// DBへのUpdateはMasterOnly.  =>>Userに変更
/// </summary>
public interface IUpdateKnowledge
{


    /// <summary>
    /// DBへのKnowledgeのUpdate.
    /// 全要素を上書きする.
    /// </summary>
    /// <returns></returns>
    Task UpdateKnowledgeTable();
}
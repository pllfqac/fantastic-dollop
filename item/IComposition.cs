using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public interface IComposition
{

    /// <summary>
    /// Item�̍���.
    /// </summary>
    /// <param name="receiveData">User���I�����������̌�����KnowledgeLevel�̊e�������simpleClass�ɂ܂Ƃ߂�json����������.</param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task CompositionItem(string receiveData, int userId);

    
}
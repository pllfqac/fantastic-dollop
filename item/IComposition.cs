using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public interface IComposition
{

    /// <summary>
    /// Item‚Ì‡¬.
    /// </summary>
    /// <param name="receiveData">User‚ª‘I‘ğ‚µ‚½‡¬‚ÌŒ´—¿‚ÆKnowledgeLevel‚ÌŠe•¶š—ñ‚ğsimpleClass‚É‚Ü‚Æ‚ß‚Äjson‚µ‚½•¶š—ñ.</param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task CompositionItem(string receiveData, int userId);

    
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// NPCとの会話.
/// </summary>
public interface ISelectionConversation 
{

    /// <summary>
    /// NPCと「会話」が選択されたとき,どの会話を再生するか.
    /// </summary>
    /// <param name="chapterProgress">シナリオ進捗度.</param>
    /// <param name="conversations">Scriptableで定義した会話集.</param>
    /// <returns>再生する会話.</returns>
    string SelectConversation(int chapterProgress, List<string> conversations);
}

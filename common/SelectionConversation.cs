using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Single2.
/// 進捗度やキャラTypeによる会話の選択.
/// </summary>
public class SelectionConversation : MonoBehaviour,ISelectionConversation
{
    //Test
  //  public NpcTagAndScenarioDefineTable table;


    //会話の選択についてはノート「NPC」を参照.

    /// <summary>
    /// NPCと「会話」が選択されたとき,どの会話を再生するか.
    /// </summary>
    /// <param name="progress">シナリオ進捗度.</param>
    /// <param name="conversations">Scriptableで定義した会話集.</param>
    /// <returns>再生する会話.先頭の※を除いた,Excelに定義したTargetと同じ文字列.</returns>
    public string SelectConversation(int progress, List<string> conversations)
    {
        if (conversations == null || conversations.Count == 0) return "default";

        //進捗度に適合するもののList
        List<string> preConv = new List<string>();


        //文字列の2つめの＠から後ろの数値を取得.最初は同等のProgressと比較して無ければ1づつ小さい値と比較していく.
        for (int SubtractionValue = 0; SubtractionValue < progress; SubtractionValue++)
        {
            if (preConv.Count != 0) break;
            Debug.Log("SubtractionValue:" + SubtractionValue);

            //Subtraction:減算
            foreach (string conv in conversations)
            {
                int value = int.Parse(conv.Substring(conv.LastIndexOf("@") + 1));
                Debug.Log("Selected Conversation Target Number:" + value);
                if ((progress- SubtractionValue) == value  ) preConv.Add(conv);
            }
        }

        foreach (var p in preConv)
        {
            Debug.Log("Selected Conv :" + p);
        }

        if (preConv.Count == 0) return "default";
        return preConv[Random.Range(0, preConv.Count)];     //選択された文字列のListからランダムに選ぶ.
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Single2.
/// �i���x��L����Type�ɂ���b�̑I��.
/// </summary>
public class SelectionConversation : MonoBehaviour,ISelectionConversation
{
    //Test
  //  public NpcTagAndScenarioDefineTable table;


    //��b�̑I���ɂ��Ă̓m�[�g�uNPC�v���Q��.

    /// <summary>
    /// NPC�Ɓu��b�v���I�����ꂽ�Ƃ�,�ǂ̉�b���Đ����邩.
    /// </summary>
    /// <param name="progress">�V�i���I�i���x.</param>
    /// <param name="conversations">Scriptable�Œ�`������b�W.</param>
    /// <returns>�Đ������b.�擪�́���������,Excel�ɒ�`����Target�Ɠ���������.</returns>
    public string SelectConversation(int progress, List<string> conversations)
    {
        if (conversations == null || conversations.Count == 0) return "default";

        //�i���x�ɓK��������̂�List
        List<string> preConv = new List<string>();


        //�������2�߂́�������̐��l���擾.�ŏ��͓�����Progress�Ɣ�r���Ė������1�Â������l�Ɣ�r���Ă���.
        for (int SubtractionValue = 0; SubtractionValue < progress; SubtractionValue++)
        {
            if (preConv.Count != 0) break;
            Debug.Log("SubtractionValue:" + SubtractionValue);

            //Subtraction:���Z
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
        return preConv[Random.Range(0, preConv.Count)];     //�I�����ꂽ�������List���烉���_���ɑI��.
    }
}

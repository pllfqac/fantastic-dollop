using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Single3.
/// �p��\�L����{��ɕϊ�����.
/// �|��ł͂Ȃ���`���Ă�����̂����蓖�Ă�.
/// </summary>
public static class ConverttoJapanese
{
    private static string[] UnitTypeJpg = new string[] { "����", "�d������", "�|��", "�y�R��", "�d�R��", "�|�R��", "���p��", };

    
    /// <summary>
    /// UnitType����{��ɕϊ�����.
    /// </summary>
    public static string ToStringByConvertToJapanese(this UnitStatus.UnitType unitType)
    {
        //Debug.Log("UnitType:" + unitType.ToString()+"    (int) "+(int)unitType);
        return UnitTypeJpg[(int)unitType-1];            //����=1 ����n�܂�̂�
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Single3.
/// 英語表記を日本語に変換する.
/// 翻訳ではなく定義してあるものを割り当てる.
/// </summary>
public static class ConverttoJapanese
{
    private static string[] UnitTypeJpg = new string[] { "歩兵", "重装歩兵", "弓兵", "軽騎兵", "重騎兵", "弓騎兵", "魔術兵", };

    
    /// <summary>
    /// UnitTypeを日本語に変換する.
    /// </summary>
    public static string ToStringByConvertToJapanese(this UnitStatus.UnitType unitType)
    {
        //Debug.Log("UnitType:" + unitType.ToString()+"    (int) "+(int)unitType);
        return UnitTypeJpg[(int)unitType-1];            //歩兵=1 から始まるので
    }

}

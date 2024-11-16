using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Single3.
/// ‰pŒê•\‹L‚ğ“ú–{Œê‚É•ÏŠ·‚·‚é.
/// –|–ó‚Å‚Í‚È‚­’è‹`‚µ‚Ä‚ ‚é‚à‚Ì‚ğŠ„‚è“–‚Ä‚é.
/// </summary>
public static class ConverttoJapanese
{
    private static string[] UnitTypeJpg = new string[] { "•à•º", "d‘••à•º", "‹|•º", "Œy‹R•º", "d‹R•º", "‹|‹R•º", "–‚p•º", };

    
    /// <summary>
    /// UnitType‚ğ“ú–{Œê‚É•ÏŠ·‚·‚é.
    /// </summary>
    public static string ToStringByConvertToJapanese(this UnitStatus.UnitType unitType)
    {
        //Debug.Log("UnitType:" + unitType.ToString()+"    (int) "+(int)unitType);
        return UnitTypeJpg[(int)unitType-1];            //•à•º=1 ‚©‚çn‚Ü‚é‚Ì‚Å
    }

}

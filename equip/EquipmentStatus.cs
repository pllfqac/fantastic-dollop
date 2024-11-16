using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Userが所持する装備のパラメータ.
/// ベース値±α(increMaxValueは除く).
/// </summary>
[Serializable]
public class EquipmentStatus  {

    public byte epwr, edex, edef, emat, emde, eagi,  increvalue,incremaxvalue;     //DBに合わせるため全小文字にした.
    /// <summary>
    /// EquipのLevel.合成でUpする.
    /// 今のところ全Equip初期値1.
    /// </summary>
    public byte level=1;
    /// <summary>
    /// この装備を装備することが出来るLevel.
    /// 上記メンバ「level」=1の時,常に0.
    /// </summary>
    public byte canEquipLevel;

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

/// <summary>
/// Single2.
/// 装備のStatusを決定する.
/// </summary>
public class EquipmentStatusGenerator : MonoBehaviour, IEquipmentStatuGenerator
{
    [SerializeField]
    private ParameterTable eBaseStatusTable = null;         //EquipmentLevel=1のEquipとその他のItem.
    [SerializeField]
    private EquipmentStatusDefinitionbyLevelTwoOver twoOverTable = null;    //Equiplevel<=2のEquipのみここで定義.


    //Masterがαを算出してStatusを決めるVer.
    public EquipmentStatus GenerateEquipStatus(byte absNum, byte equipmentLevel = 1)
    {
        if (!PhotonNetwork.IsMasterClient) return new EquipmentStatus();
        EquipmentStatus eStatus = new EquipmentStatus();


        EquipmentBaseStatusDefinition baseStatus;
        if (equipmentLevel == 1) baseStatus = eBaseStatusTable.eBaseStDefTable.FirstOrDefault(x => x.AbsoluteNumber == absNum); ////例外発生時はAll 0のEquipmentStatus返す>
        else baseStatus = twoOverTable.GetStatusDefinitionTwoOver(absNum, equipmentLevel);

        if (baseStatus == null) return eStatus;

        eStatus.epwr = CalcStatus(baseStatus.eqBasePwr, StaticMyClass.CharaStatusType.Power);
        eStatus.edex = CalcStatus(baseStatus.eqBaseDex, StaticMyClass.CharaStatusType.Dexerity);
        eStatus.edef = CalcStatus(baseStatus.eqBaseDef, StaticMyClass.CharaStatusType.Defense);
        eStatus.emat = CalcStatus(baseStatus.eqBaseMat, StaticMyClass.CharaStatusType.MagicAttack);
        eStatus.emde = CalcStatus(baseStatus.eqBaseMde, StaticMyClass.CharaStatusType.MagicDefense);
        eStatus.eagi = CalcStatus(baseStatus.eqBaseAgi, StaticMyClass.CharaStatusType.Agility);
        eStatus.increvalue = CalcIncreValue(baseStatus.equipmentMaxIncrement);
        eStatus.incremaxvalue = baseStatus.equipmentMaxIncrement;
        eStatus.level = equipmentLevel;
        eStatus.canEquipLevel = baseStatus.CanEquipLevel;
        return eStatus;
    }

    //255を超えないようにする.第二引数はDebug用
    private byte CalcStatus(byte baseStatus, StaticMyClass.CharaStatusType type)
    {
        int tempValue = baseStatus + CalcAlpha();
        if (tempValue > byte.MaxValue) tempValue = byte.MaxValue;
        if (tempValue < 0) tempValue = 0;

        return (byte)tempValue;
    }

    //Baseに加算するα値の算出.
    private int CalcAlpha()
    {
        return UnityEngine.Random.Range(StaticMyClass.equipmentStatusAddValueMin, StaticMyClass.equipmentStatusAddValueMax + 1);       //-2～10の範囲のランダム値
                                                                                                                                       //   Debug.Log("Base値に足す値:" + randomValue);
                                                                                                                                       //  return randomValue;
    }

    //インクリ回数の算出.0～Maxまでの値(案)
    private byte CalcIncreValue(byte baseincreMax)
    {
        return (byte)UnityEngine.Random.Range(0, baseincreMax + 1);
    }



    /// <summary>
    /// 定義した値をそのままEquipmentStatusにコピーして取得するVer.
    /// </summary>
    /// <param name="abNum"></param>
    /// <param name="equipmentLevel">指定しなければ1になる.</param>
    /// <returns></returns>
    public EquipmentStatus GetbaseEquipmentStatus(byte absNum, byte equipmentLevel = 1)
    {
        EquipmentStatus eStatus = new EquipmentStatus();
        EquipmentBaseStatusDefinition bStatus;

        if (equipmentLevel == 1) bStatus = eBaseStatusTable.eBaseStDefTable.FirstOrDefault(x => x.AbsoluteNumber == absNum);   //例外発生時はAll 0のEquipmentStatus返す>
        else bStatus = twoOverTable.GetStatusDefinitionTwoOver(absNum, equipmentLevel);         //例外発生時はそのまま

        if (bStatus == null) return eStatus;

        eStatus.epwr = bStatus.eqBasePwr;
        eStatus.edex = bStatus.eqBaseDex;
        eStatus.edef = bStatus.eqBaseDef;
        eStatus.emat = bStatus.eqBaseMat;
        eStatus.emde = bStatus.eqBaseMde;
        eStatus.eagi = bStatus.eqBaseAgi;
        eStatus.increvalue = bStatus.eqIncrementValue;
        eStatus.incremaxvalue = bStatus.equipmentMaxIncrement;
        eStatus.level = equipmentLevel;
        eStatus.canEquipLevel = bStatus.CanEquipLevel;
        return eStatus;
    }
}

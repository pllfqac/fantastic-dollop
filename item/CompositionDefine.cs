using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ひとつのItem合成の定義.
/// </summary>
[Serializable]
public class CompositionDefine
{

    /// <summary>
    /// 合成の結果,生成されるアイテムのAbs.
    /// 生成される個数は必ず1.
    /// </summary>
    [SerializeField]
    private int _resultItemAbs;
    public int ResultItemAbs => _resultItemAbs;
    /// <summary>
    /// 合成されたEquipアイテムのEquipLevel.EquipLevelUp以外は0にすること.
    /// </summary>
    [Tooltip("合成の結果のEquipLevel.EquipLevelUp以外は0にすること.")]
    public byte equipmentLevel;

    /// <summary>
    /// 合成成功確率.1～100.
    /// ここで定義した値よりランダム値が大きい場合「失敗」とみなす.
    /// </summary>
    [Tooltip("合成成功確率.1～100")]
    public int compositionSuccessProbabilityContant;

    /// <summary>
    /// 合成に必要なお金.
    /// </summary>
    public int compositionCost;

    /// <summary>
    /// 単純強化でのeStatusに付与する値の種類と上下値の定義.
    /// </summary>
    [SerializeField]
    private EnhancementClass _EnhancementValue;
    public EnhancementClass Enhancement => _EnhancementValue;


    /// <summary>
    /// 合成の原料.
    /// 必要な原料数が複数個の場合に対応するためにSimpleInt2Class.
    /// SimpleInt2Class.dにItemAbs,d2に必要個数を入力.
    /// </summary>
    public List<SimpleInt2Class> materials;




}


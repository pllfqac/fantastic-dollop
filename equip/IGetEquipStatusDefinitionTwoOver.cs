using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 対応するStatus定義を返す.
/// </summary>
public interface IGetEquipStatusDefinitionTwoOver
{
    EquipmentBaseStatusDefinition GetStatusDefinitionTwoOver(int abs, int equipLevel);
}

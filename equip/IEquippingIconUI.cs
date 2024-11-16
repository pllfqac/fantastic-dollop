using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 所持アイテムリスト上の「装備した」アイテムに,装備中であることを示す「E」マーク(仮)を付ける.
/// </summary>
public interface IEquippingIconUI
{

    void AddEquippingIcon(RectTransform createNode);
    void DeleteEquippingIcon(RectTransform deleteTargetNode);

}

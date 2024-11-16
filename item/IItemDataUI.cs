using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ItemDataPanelの表示.
/// </summary>
public interface IItemDataUI
{

    void ShowItemData(ItemNodeProp nodeProp, int instanceId, int tapPosition, string tapNodeTagName);      //装備用(GUID付).



    /// <summary>
    /// ItemDataPanelの表示.
    /// その他ItemVer.
    /// </summary>
    /// <param name="definition"></param>
    /// <param name="instanceId">TapされたitemNodeのgameoblect.GetInstanceID.同Nodeの再Tap検知に利用.</param>
    /// <param name="tapPosition">TapされたNodeのRectTransform.Position.x. この値を元にItemDataPanelの表示位置を決定する</param>
    /// <param name="tapNodeTagName">TapされたNodeのTag.</param>
    void ShowItemData(ItemNodeProp nodeProp, ItemDefinition definition, int instanceId, int tapPosition, string tapNodeTagName);                           //その他Item.
}

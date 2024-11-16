using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Itemを捨てるとき.
/// </summary>
public interface IItemDelete
{

    /// <summary>
    /// 所持Itemを捨てる.
    /// 装備中のアイテムは捨てれない仕様.
    /// </summary>
    /// <param name="nodeData">捨てるItem<</param>
    /// <param name="deleteCount">捨てるItem<の数</param>
    void Delete(INodeData nodeData, byte deleteCount);

    /// <summary>
    /// User用.
    /// Hash1つのみの場合Ver.
    /// </summary>
    /// <param name="oneAbs"></param>
    void Delete(OneAbsItemClass oneAbs);

    /// <summary>
    /// 所持アイテムを削除する.
    /// </summary>
    /// <param name="deleteItem">削除するItem.</param>
    void Delete(Dictionary<int, OneAbsItemClass> deleteItem);
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// User所持アイテムが所持アイテムでなくなるとき(消費,売却,捨てetc.)に呼ばれる.
/// </summary>
public interface IItemOut  {

    /// <summary>
    /// Itemの使用.
    /// 使用と消費を区別する.
    /// </summary>
    /// <param name="ItemNode">使用するアイテムのINode参照</param>
    void UseItem(INodeData ItemNode);

    /// <summary>
    /// userOwnTabelから売るItemを取り除く.
    /// </summary>
    /// <param name="sellItemLis">売る予定のItem.</param>
    /// <returns>MasterへのRaise用</returns>
    Dictionary<int,OneAbsItemClass> DeleteSelectedSellItems(List<ItemNodeProp> sellItemLis);

    /// <summary>
    /// Itemの消去.1つのみの場合Ver.
    /// </summary>
    /// <param name="oneAbs"></param>
    void Delete(OneAbsItemClass oneAbs);


}

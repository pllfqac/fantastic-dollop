using System;

/// <summary>
/// 1つのItemにつくItem固有データClass.
/// GUID無しの場合はつかない.
/// </summary>
[Serializable]
public class OneItemClass  {

    public string GUID;     //このItemのGUID(世界で唯一の値).

    //その他このアイテム固有のパラメータ.equipのとき再考する.
    //コンストラクタ呼び出し時に省略可能.


 //   public int? ItemLevel;
 //   public int? parameter2;

    /// <summary>
    /// このItemがEquipmentの場合の装備ステータス.
    /// 装備でないならnull.
    /// </summary>
    public EquipmentStatus eStatus = null;

    [NonSerialized]
    public EquipmentDefinition eDefinition = null;

    public OneItemClass(string guid/*,int? p1=1,int? p2=null*/)
    {
        this.GUID = guid;
     //   ItemLevel = p1;
     //   parameter2 = p2;
    }
}

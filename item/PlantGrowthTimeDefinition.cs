using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// 植物の成長時間の定義
/// </summary>
[Serializable]
public class PlantGrowthTimeDefinition
{

    public PlantGrowthTimeDefinition(int abs, int growthTime/*,MySpace.Attribute.Element element*/)
    {
        this.Abs = abs;
        this.GrowthTime = growthTime;
       // this.element = element;
    }


    [SerializeField]
    private int _abs;
    /// <summary>
    /// ItemDefinition.ItemType.SeedのitemAbs.
    /// </summary>
    public int Abs
    {
        get { return _abs; }
        set { _abs = value; }
    }



    [SerializeField]
    private int _growthTime;
    /// <summary>
    /// 植物の成長時間.
    /// 播種してから収穫までの時間.
    /// </summary>
    public int GrowthTime
    {
        get { return this._growthTime; }
        set { this._growthTime = value; }
    }

    //Class名と合ってないけどここで定義.
    [SerializeField]
    private MySpace.Attribute.Element element;
    /// <summary>
    /// この作物の収穫時の報酬の要素.
    /// Manor.FoodElementsのどの値に関わるか.
    /// </summary>
    public MySpace.Attribute.Element Element
    {
        get { return element; }
        set { element = value; }
    }
}

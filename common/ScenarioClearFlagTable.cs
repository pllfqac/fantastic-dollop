using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Scriptable.
/// シナリオを先に進めることが出来るフラグのクリア条件を定義したTable.
/// </summary>
[CreateAssetMenu(menuName = "MyGame/Create ScenarioClearFlagTable", fileName = "ClearFlagTable")]

public class ScenarioClearFlagTable : ScriptableObject
{
    /// <summary>
    /// シナリオ別のクリア条件(シナリオ再生条件)を定義したTable.
    /// </summary>
    [SerializeField]
    private List<ScenarioItemClass> scenarioClearConditions;

    /// <summary>
    /// シナリオのクリア条件を取得する.
    /// 無ければnull.
    /// </summary>
    /// <param name="progress">シナリオ進捗度.</param>
    /// <param name="cType"></param>
    /// <returns>クリア条件が無ければnull.</returns>
    public ScenarioItemClass GetScenarioClearDefine(int progress,StaticMyClass.CharaType cType)
    {
        List<ScenarioItemClass> sc;
        //CP番号のみで検索.次のシナリオ(+1)か否か.
        sc = scenarioClearConditions.Where(x => int.Parse(x.ScenariTarget.Substring(0, x.ScenariTarget.IndexOf("@"))) == (progress+1)).ToList();
        if (sc==null ||  sc.Count == 0) return null;
        //CharaTypeも加味.自キャラのcTypeが含まれたシナリオターゲットか否か
        //文字列内に指定された文字列があるかを調べるだけであれば、IndexOfメソッドが0以上の整数を返すかを調べるだけで十分です。
        if (sc.Any(x => x.ScenariTarget.IndexOf(cType.ToString()) >= 0)) return sc.First(x => x.ScenariTarget.IndexOf(cType.ToString()) >= 0);
        else return null;
    }
}

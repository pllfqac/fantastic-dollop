using System.Collections;
using System.Collections.Generic;
using Random = Unity.Mathematics.Random;
using UnityEngine;



public class RandomSelectIndex
{

    private Random random;

    /// <summary>
    /// 初期化.
    /// 引数にシード値を指定.シード値が小さいと出力も小さい?ので注意.
    /// </summary>
    /// <param name="seed">シード値.uint型.0は例外が出るので注意.</param>
    public RandomSelectIndex(uint seed)
    {
        random = new Random(seed);
    }


    /// <summary>
    /// 重みによるランダム選択（復元抽出）.
    /// 線形探索.
    /// 重みの合計値を算出して、その範囲内で乱数値を生成し、重みリストを順に見ていって対応する要素を引っ張ってきます。
    /// 計算量はO(N)ですが、追加の記憶領域を必要としません
    /// </summary>
    /// <param name="weights"></param>
    /// <returns></returns>
    public int RandomSelectIndexByWeight(List<float> weights)
    {
        var total = CalcTotalWeight(weights);
        // var fv = UnityEngine.Random.value * total;      //unityAPIはメインスレッドのみ!   0.0 (この値を含む) と 1.0 (この値を含む) の範囲のランダムな数を返します
        float fv = random.NextFloat(1)*total;     //random.NextFloat(1) => 0以上，1未満の小数 [0,1)
       // Debug.Log("random.NextFloat  " + fv);
        var index = ResolveIndex(weights, fv);
        return index;
    }

    private float CalcTotalWeight(List<float> weights)
    {
        var length = weights.Count;

        var total = 0.0f;
        for (var i = 0; i < length; ++i)
        {
            var weight = weights[i];
            if (weight > 0.0f)
            {
                total += weight;
            }
        }

        return total;
    }

    private int ResolveIndex(List<float> weights, float random)
    {
        var length = weights.Count;

        var index = -1;
        var cumulative = 0.0f;      //累積的な
        for (var i = 0; i < length; ++i)
        {
            var weight = weights[i];
            if (weight > 0.0f)
            {
                cumulative += weight;
                if (cumulative >= random)
                {
                    index = i;
                    break;
                }
            }
        }

        return index;
    }
}

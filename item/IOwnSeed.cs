using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOwnSeed  {

    void SetOwnSeed(int TotalUserOwnSeed);


    /// <summary>
    /// 所持Seedの値を取得.
    /// </summary>
    /// <returns>所持Seedの数</returns>
    int ReadOwnSeed();

    /// <summary>
    /// Seedを取得.
    /// </summary>
    /// <param name="inSeedValue">取得するSeedの数.</param>
    void SeedIn(int inSeedValue);

    /// <summary>
    /// Seedを消費.
    /// </summary>
    /// <param name="outSeedValue"></param>
    void SeedOut(int outSeedValue);
}

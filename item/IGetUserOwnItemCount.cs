using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Userが所持しているItemの個数(Abs数)の取得.
/// </summary>
public interface IGetUserOwnItemCount
{
    /// <summary>
    /// 所持アイテム数(Abs数)を取得する.
    /// </summary>
    /// <returns></returns>
    int GetOwnItemCount();

    /// <summary>
    /// 指定したAbsのItemの所持数を返す.
    /// 持っていなければ0.
    /// </summary>
    /// <param name="targetAbs"></param>
    /// <returns></returns>
    int CountOwnItem(int targetAbs);
}
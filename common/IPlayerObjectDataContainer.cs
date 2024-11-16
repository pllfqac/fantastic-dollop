using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Room切替後にPlayerオブジェに展開するデータの入れ物.
/// </summary>
public interface IPlayerObjectDataContainer
{
    /// <summary>
    /// GetLoginClass
    /// </summary>
    GetLoginClass GetLoginClass { get; }
    /// <summary>
    /// ゲーム開始後最初のCRのPlayerInstant中のみTrue.
    /// それ以降はfalse.
    /// </summary>
    bool IsLogin { get; set; }
    string toMasterData { get; }
    /// <summary>
    /// 新規UserのみTrue.
    /// </summary>
    bool IsFirstLogin { get; }
}

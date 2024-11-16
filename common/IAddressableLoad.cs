using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

/// <summary>
/// (ゲーム開始時の)依存関係をDLする.
/// </summary>
public interface IAddressableLoad
{
    /// <summary>
    /// //ダウンロード中の処理.
    /// </summary>
    event Action<float> downloadAction;
    //event Action DLEnd;


    /// <summary>
    /// Downloadサイズを取得する.
    /// </summary>
    /// <param name="targetStr">Loadの対象となるAASに付けたLabelを指定する.e.g "Enemy","Scene".</param>
    /// <returns>byte単位</returns>
    Task<long> CheckDownloadSizeAsync(string targetStr);

    /// <summary>
    /// 依存関係DL.
    /// </summary>
    /// <param name="loadTargetString">Loadの対象となるAASに付けたLabelを指定する.e.g "Enemy","Scene".</param>
    Task LoadFromMyServerAsync(string loadTargetString);
}

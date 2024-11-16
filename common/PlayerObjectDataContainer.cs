using Secure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Single1.
/// PlayerInstantのみで使用.
/// Room移動でPlayerObjが破壊されるので,破壊されたくないデータを入れておく==> Login後はMyPlayerDataRefに置いてるのでそれ参照
/// </summary>
public class PlayerObjectDataContainer:MonoBehaviour,IPlayerObjectDataContainer
{

    public GetLoginClass GetLoginClass { get; private set; }

    
    /// <summary>
    /// Userを介してMasterへ送られるデータのTemp.暗号化ShaMDN+IV.
    /// </summary>
    public string toMasterData { get; private set; }

    /// <summary>
    /// ゲーム開始後最初のCRのPlayerInstant中のみTrue.
    /// それ以降はfalse.
    /// </summary>
    public bool IsLogin { get; set; }
  
    /// <summary>
    /// 新規UserのみTrue.
    /// </summary>
    public bool IsFirstLogin { get; private set; }



    /// <summary>
    /// Loginした時のデータ保持.
    /// </summary>
    /// <param name="glc"></param>
    /// <param name="isFirstLogin">新規Userの場合のみTrue.既存UserLoginはFalse.</param>
    /// <param name="toMasterData">Userを介してMasterへ送られるデータ</param>
    public void SetLoginClass(GetLoginClass glc,bool isFirstLogin,string toMasterData)
    {
        GetLoginClass = glc;
        IsLogin = true;
        IsFirstLogin = isFirstLogin;
        this.toMasterData = toMasterData;
    }

}
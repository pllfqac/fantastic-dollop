using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// UserのRoomの変更.
/// </summary>
public interface IChangeRoom
{

    /// <summary>
    /// バトルRoomからCRへの移動.
    /// </summary>
    public void ChangeToCommonRoom();




    /// <summary>
    /// BattleRoomに移動.
    /// 自身がBRマスタクライアントとなる.
    /// 「戦闘開始」Buttonから呼ばれる.
    /// </summary>
    /// <param name="requestRei">援軍を要請していた場合True.</param>
    public void ChangeToBattleRoom(bool requestRei);

    /// <summary>
    ///  BattleRoomに移動.
    ///  他UserがBRマスタクライアントとなる.
    ///  「援軍に向かう」ボタンから呼ばれる.
    /// </summary>
    /// <param name="otherUserCreatedBRName">他Userが作成したBR名.</param>
    public void ChangeToBattleRoom(string otherUserCreatedBRName);

}
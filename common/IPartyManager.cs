using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPartyManager
{
    /// <summary>
    /// 自身がいずれかのPartyに所属しているかチェック.
    /// </summary>
    /// <returns></returns>
    bool CheckJoinedParty();
    /// <summary>
    /// 対象のIDがPartyに含まれるかどうか確認.含まれていればTrue.
    /// </summary>
    bool CheckJoinedParty(int id);

    /// <summary>
    /// BRへ遷移した(する)場合はTrue.それ以外はfalse.
    /// </summary>
    /// <param name="userId">一意のUserId.</param>
    /// <returns></returns>
    bool IsChangeToBRFlagUpped(string userId);
    /// <summary>
    /// 指定したコライダがPartyMemberMapに含まれているか確認.
    /// </summary>
    /// <param name="collider"></param>
    /// <returns>含まれているならTrue.</returns>
    bool ContainCollider(Collider collider);

    /// <summary>
    /// PartyめんばのコライダListを取得する.
    /// </summary>
    /// <returns></returns>
    IReadOnlyCollection<Collider> GetPartyMemberColliders();

    /// <summary>
    /// 指定したUserをPartyMemberListから除く.
    /// </summary>
    /// <param name="userId"></param>
    void RemoveLeaveMember(int userId);


    /// <summary>
    /// BRマスタクライアントとなるUserが他PTメンバへBR名を送る.
    /// </summary>
    /// <param name="battleRoomName">BR名</param>
    void SendBattleRoomName(string battleRoomName);
}
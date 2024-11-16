using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// single3.
/// 援軍を要請する.
/// PTメンバが存在すればPTメンバのみに要請.
/// PTメンバが存在しなければCR全Userに要請.
/// </summary>
public class CallForReinforcements : MonoBehaviour
{

    private IPartyManager pm;
    private IRaiseEventClass raise;

    private void Start()
    {
        GameObject single2 = GameObject.FindWithTag("single2");
        pm = single2.GetComponent<IPartyManager>();
        raise = single2.GetComponent<IRaiseEventClass>();
    }

    /// <summary>
    /// 援軍を要請する.
    /// </summary>
    /// <param name="targetBattleRoomName">自身が作成したBR名.</param>
    public void CallforReinforcements(string targetBattleRoomName)
    {
        if (pm.CheckJoinedParty())
        {
            //PTメンバのみに援軍要請する.
            pm.SendBattleRoomName(targetBattleRoomName);
            
        }
        else
        {
            //CR全員へ(仮)援軍要請する.
            Photon.Realtime.RaiseEventOptions op = new Photon.Realtime.RaiseEventOptions()
            {
                CachingOption = Photon.Realtime.EventCaching.DoNotCache,
                Receivers=Photon.Realtime.ReceiverGroup.Others,
            };
            raise.StartRaise((byte)RaiseEventClass.EEventType.SendBattleRoomName, targetBattleRoomName,op);
        }
    }
}

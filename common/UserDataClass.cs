using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player.==> Login時にnewしてMasterAllMemberTableに参照持たせておく.
/// その端末のPlyerとMaster.==>UniMasOnly.
/// そのPlayerのdata保持Class.
/// </summary>
public class UserDataClass /*: MonoBehaviour */
{

    public int SerialNumber { get; set; }

	/// <summary>
	/// MasterOnly.
	/// User一意識別子.UserLogin時に取得.
	/// </summary>
	public string Shauid { get; set; }


}

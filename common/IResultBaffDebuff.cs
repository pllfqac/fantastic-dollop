using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IResultBaffDebuff
{

	/// <summary>
	/// MasterからのRPC受信後.
	/// </summary>
	void ReceiveUCC(byte condi, byte flucNum,int skillUserId);

	/// <summary>
	/// UCCのmissの場合.
	/// </summary>
	void ReceiveUccMiss(int skillUserId);

	/// <summary>
	/// UCC終了.
	/// </summary>
	void ReceiveUccEnd(byte condi);
}

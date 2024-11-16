using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/// <summary>
/// 他端末とのEquipmentの同期.
/// </summary>
public interface IEquipmentSync 
{
	int RaiseWaitCount { set; }


	/// <summary>
	/// Equip同期.
	/// 装備した時.
	/// 自端末から他端末に向けてのRPC.
	/// UniMasterを除く.
	/// </summary>
	void EquipSyncByRPC(byte itemAbs);

	/// <summary>
	/// Equip同期.
	/// 装備はずした時.
	/// </summary>
	/// <param name="unequipItemAbs"></param>
	void UnEquipSyncByRPC(byte unequipItemAbs);

	/// <summary>
	/// Area遷移時の装備同期.
	/// 送る対象の箇所は Head,Arm1,Arm2.
	/// byte配列にしてItemABsを送る.
	/// 装備していなければ0送るので注意.
	/// </summary>
	void EnterAreaEquipmentSyncByRPC();

	/// <summary>
	/// Equip同期.
	/// RPCあとに自端末自キャラのこのメソッドが呼ばれる.
	/// </summary>
	void ResponseEquipSync(int ownerId);

	/// <summary>
	/// EquipSyncByRaise()で飛んできたRaiseの受信後の処理.
	/// </summary>
	/// <param name="vs">itemAbs配列.順番はHead,arm1,arm2の順で固定しているので注意.</param>
	void ReceiveEquipSyncRaise(byte[] vs);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public interface ISafetyNet  {


	/// <summary>
	/// SafetyNetでの検証.
	/// 結果はeventで受け取る.検証に問題なければDB_Key.問題があればNull.
	/// </summary>
	Task SafetyNetAuth();


	/// <summary>
	/// SafetyNet処理終了(JWT検証終了)で呼ぶメソッドを登録.
	/// </summary>
	event SafetyNetDel SafetyNetEndEvent;
}

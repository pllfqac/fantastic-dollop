using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// アクセストークンの有効期限の設定等.
/// </summary>
public interface IExpiryConfig  {


	void ReStartTimer();




	/// <summary>
	/// 1h後の時間を文字列型で取得.
	/// </summary>
	/// <returns></returns>
	//string GetOneHourLaterExcpiry();
}

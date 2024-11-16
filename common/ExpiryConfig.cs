using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;


/// <summary>
/// Single1.
/// アクセストークンの有効期限の設定等.
/// </summary>
public class ExpiryConfig :MonoBehaviour, IExpiryConfig {

	//c#側のdatetimeはc#側で完結する.つまりDB側で保存したdatetimeと違う値でも「有効期限(1h後)」がわかればok.なお,DBからは1h過ぎてもアプリ側に更新を要求しない.

	[SerializeField]
	private AccessPreparation AccessPreparation=null;



	//	private const float timeOut = 3600000;       //1[h]=3600000[ms]
	private  float testTimeOut = 60*1000;       //1[min]


	/// <summary>
	/// 経過時間.
	/// </summary>
	private float timeElapsed;

	/// <summary>
	/// TrueでUpdate()内Timer有効.
	/// timeOutになった場合でもfalse.
	/// </summary>
	private bool ExpiryTimerEnable;


	private void Update()
	{
		if (!ExpiryTimerEnable) return;

		timeElapsed += Time.deltaTime;
		if (timeElapsed >= testTimeOut)
		{
			UpdateAccessToken();
			timeElapsed = 0.0f;
			ExpiryTimerEnable = false;
			Debug.Log("ExpiryTimerEnable = false");
		}
	}

	/// <summary>
	/// タイマーを0[s]から起動します.
	/// </summary>
	public void ReStartTimer()
	{
		timeElapsed = 0.0f;
		ExpiryTimerEnable = true;
	}


	/*トークン更新は,UpdateAccessToken.phpにアクセス */

	/// <summary>
	/// ServerにAccessToken更新を依頼する.
	/// </summary>
	/// <returns>更新成功で新しいアクセストークン.</returns>
	private async void UpdateAccessToken()
	{
		string accessToken = PlayerPrefs.GetString("at");
		Debug.Log("アクセストークン更新依頼! 送信Token:" + accessToken);
		//UnityWebRequestを生成
		WWWForm wWWForm = AccessPreparation.AccessPreparationUniServer(true);
		wWWForm.AddField("at", accessToken);
		UnityWebRequest unityWebRequest = UnityWebRequest.Post(StaticMyClass.UpdateAccessTokenEndPoint, wWWForm);
		await unityWebRequest.SendWebRequest();


		if (unityWebRequest.result==UnityWebRequest.Result.ConnectionError || unityWebRequest.result==UnityWebRequest.Result.ProtocolError)
		{
			//エラー確認
			Debug.Log("webRequest.error   " + unityWebRequest.error);
			throw new Exception();  
		}
		else
		{
			//結果確認
			TradeServerClass tradeServerClass = JsonUtility.FromJson<TradeServerClass>(unityWebRequest.downloadHandler.text);
			//更新依頼の為のアクセストークンの有効期限が切れていた場合も例外発生でUserを落とす.
			if (tradeServerClass.mn == "-10" && !string.IsNullOrEmpty( tradeServerClass.data)) throw new Exception();
			else
			{
				Debug.Log("AccessToken更新成功!  :" + tradeServerClass.data);
				PlayerPrefs.SetString("at", tradeServerClass.data);                 //Lambdaから取得したアクセストークンをPlayerPrefsにsave.
				PlayerPrefs.Save();
				ReStartTimer();
			}

		}
	}


	/*
	/// <summary>
	/// 1h後の時間を文字列型で取得.
	/// </summary>
	/// <returns></returns>
	public string GetOneHourLaterExcpiry()
	{
		DateTime dateTime = DateTime.Now;
		DateTime oneHourlater = dateTime.AddHours(1.0f);            //1h後の時刻.
		Debug.Log(oneHourlater.ToString());
		return oneHourlater.ToString();
	}
	*/


}

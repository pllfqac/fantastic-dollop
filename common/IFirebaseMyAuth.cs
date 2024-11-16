using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public interface IFirebaseMyAuth  {



    /// <summary>
    /// Firebaseへ[Email&Pass]でLogin.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns>Login出来たらTrue.</returns>
    Task<bool> LoginWithEmailAsync(string email, string password);



	/// <summary>
	/// IDトークン取得と,ApacheサーバでのIDトークン認証.
	/// </summary>
	/// <returns>アクセストークン+expity.失敗なら例外発生.</returns>
	Task<string> AuthIDTokenbyMyServerAsync();

	/// <summary>
	/// [Email&Pass]でFirebase新規User作成.
	/// </summary>
	/// <param name="email"></param>
	/// <param name="password"></param>
	/// <returns>作成成功でTrue.</returns>
	Task<bool> CreateUserWithEmailAsync(string email, string password);

	/// <summary>
	/// Userにメールを送信する.
	/// </summary>
	void SendEmail();


	/// <summary>
	/// FirebaseへGoogleでLogin or 新規作成.
	/// </summary>
	/// <returns>Json形式のアクセストークン.キャンセルor失敗ならnull.</returns>
	Task<string> GoogleLoginAsync();

	/// <summary>
	/// Firebase で認証する
	/// </summary>
	/// <param name="idToken"></param>
	/// <returns>OKならtrue.キャンセルorエラーならfalse.</returns>
	Task<bool> FirebaseCredentialAsync(string idToken);

	/// <summary>
	/// ユーザーのプロファイルを取得する.
	/// </summary>
	/// <returns></returns>
	//Firebase.Auth.FirebaseUser GetUserProfile();
	/// <summary>
	/// UserIDを返す.
	/// 一意の文字列.
	/// </summary>
	/// <returns></returns>
	string GetUserID();


    /// <summary>
    /// localTest用.
    /// [Email&Pass]のApacheサーバでのIDトークン認証のﾃｲの処理.
    /// </summary>
    /// <param name="dummyUID"></param>
    /// <returns></returns>
    Task<string> DummyAuth(string dummyUID);
}

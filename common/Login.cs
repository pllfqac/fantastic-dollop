using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// Single1.
/// </summary>
public class Login : MonoBehaviour {


    public IncrementSequenceNumber seqnum;
    public AccessPreparation accessPre;
    public ExclusiveEncrypt Xor;
    public _SHA256 sha2;            //sha256ハッシュ変換スクリプト
    public Receiving_result_branch receiving_result;      //データベース受信処理



	/// <summary>
	/// サーバに保存したMyデータを取得する.
	/// Firebaseログインで取得したアクセストークンでアクセスする.
	/// </summary>
	/// <returns></returns>
	public async Task GameLogin()
    {
        IAES aes = GetComponent<_AES2>();
        WWWForm form = accessPre.AccessPreparationUniServer(StaticMyClass.loginEnd);

        aes.CreateKey();      //ランダム値を元にAES_Key生成

        //--------------------------暗号化するクラスの処理--------------------------
        LoginClass lc = new LoginClass();   //Angou_bun用クラスのインスタンス生成
      //  lc.mdn = PlayerPrefs.GetString("at");  //MDN代入→アクセストークンに変更
        lc.mac_key = sha2.GetMACKey;    //MAC用共有キー代入
		lc.rosumi = StaticMyClass.sikibetunum01;// sikibetunum01;      //ログイン用の識別値代入
        lc.id = seqnum.SequenceNumber;       //シーケンス番号代入    サーバ側は復号後にこの値を確認＆保存

        //MDN,MAC共有キー,(有れば)パスをAESで暗号化してその暗号文とAES_Key,AES_IV をJsonでまとめる
        string st = JsonUtility.ToJson(lc);         //暗号化するクラスをJson形式に変換
        Result_AES cyp = aes.Encrypt(st);			//JsonをAESで暗号化


        //--------------------------サーバに送るクラスの処理--------------------------
        TradeServerClass gs = new TradeServerClass();
        gs.a = cyp.aes_data;
        gs.AI = cyp.IV/*aes.siv*/ + "." + aes.aesKey;	//pass有無にかかわらずAES_Keyは最初のみ送る       AES_IVの後ろに .AES_Key の形で付ける
        gs.DB_Key = StaticMyClass.DB_KEY;
		gs.at= PlayerPrefs.GetString("at");
		gs.mn = sha2.CreateMacNum(st);
        string go = JsonUtility.ToJson(gs);     //Jsonでまとめる

        form.AddField("SendMyData", go);
        LoginAndNewCreateAddForm(form);     //GamePlay毎のログイン or 新規作成時の時のみ送る

        UnityWebRequest unityWebRequest = UnityWebRequest.Post(StaticMyClass.LoginFlowEndPoint/*db_con.loginFlowEndPoint*/, form);
        await unityWebRequest.SendWebRequest();                                                                             //ここでawait.

		//順番注意.503を先に捕捉する.エラーあれば終了.
		if (unityWebRequest.responseCode==503)  //前回のGame起動時に新規作成時間が長くてJWTの有効期限を過ぎた場合やPhoton接続が切れた場合、新規作成する.
		{
			Debug.Log("503");
            unityWebRequest.Dispose();
			SceneManager.LoadScene("NewUniState_scene");
		}
		else if (unityWebRequest.result==UnityWebRequest.Result.ConnectionError || unityWebRequest.result==UnityWebRequest.Result.ProtocolError || unityWebRequest.responseCode == 500)
		{
			//NG
			Debug.Log(unityWebRequest.error);
            unityWebRequest.Dispose();
            throw new MyUniException(StaticError.ErrorType.NetworkAccessError);
		}
		else
        {
            //ok
            Debug.Log(unityWebRequest.downloadHandler.text);
            await receiving_result.Login_Process(unityWebRequest.downloadHandler.text);    //受信処理 Receiving_result_branchへ
            unityWebRequest.Dispose();
        }

    }


    //GamePlay毎のログイン or 新規作成時の時のみ送るForm   これでログイン済みかどうかサーバ側は識別
    private void LoginAndNewCreateAddForm(WWWForm form)
    {
		form.AddField("lncaf",StaticMyClass.loginKey);
    }
}
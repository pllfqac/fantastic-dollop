#pragma warning disable 0649        //JsonClassの初期値がnullになってるワーニングを消してる

using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;


//データベース受信結果処理スクリプト　ログイン or 新規登録 のみ　　　　通常はReceivingReadData使う
//DB_connectionで受け取ったサーバからの結果を処理する
public class Receiving_result_branch : MonoBehaviour
{


    private PUNController punCtrl;
    private IAES aes;
    private _SHA256 sha2;

    [NonSerialized]
    public NewSettingManager newSetMgn;    //NewSettingScene遷移で入る
    private SceneFadeCtrl fade;
    private void Start()
    {
        Debug.LogWarning("RRB Start");
        fade = GameObject.FindWithTag("FadeCanvas").GetComponent<SceneFadeCtrl>();
    }


    //================================================ログイン時のみの処理(区分による場合分け)  =====================================================
    public async Task Login_Process(string resultText)
    {
        punCtrl = GetComponent<PUNController>();
        sha2 = GetComponent<_SHA256>();
        aes = GetComponent<_AES2>();
        if (!StaticMyClass.loginEnd)
        {
            TradeServerClass jsonClassObj = JsonUtility.FromJson<TradeServerClass>(resultText);      //サーバからのJsonデータをTradeServerClassでデコード
            Result_AES decr = aes.Decrypt(jsonClassObj.a, jsonClassObj.AI);       //Angou_bunをAES復号                                                                                                        //    string hukugou = decr.hukugou_data;
            var jsonBaseClassObj = JsonUtility.FromJson<GetLoginClass>(decr.hukugou_data);    //区分確認の為にデコード


            switch (jsonBaseClassObj.kubun)
            {               //TitleCtrlスクへ
                case 100:       //ログイン処理　MDN未登録だった 新規登録へ
                    Debug.Log("-----100----- : chage newstate");
                    NewCreate_100(decr.hukugou_data, jsonClassObj.mn);
                    break;
                case 101:       //ログイン処理　新規Login
                    Debug.Log("-----101----- : 新規Login");
                    await Login_successful_102(decr.hukugou_data, jsonClassObj.mn, jsonClassObj.data, true);
                    break;
                case 102:       //ログイン成功　既存User Login   DBから受け取ったデータを変数に入れる
                    Debug.Log("-----102----- : 既存Login");
                    await Login_successful_102(decr.hukugou_data, jsonClassObj.mn, jsonClassObj.data, false);
                    break;

                case 104:       //ログイン失敗 新規登録でのキャラネーム重複・禁止ワード含んでる等のエラー　NewSettingManagerスクに失敗の旨を知らせる.
                    Debug.Log("-----104-----");
                    newSetMgn.Err104();             //業務エラー.
                    break;
                default:
                    //103など　たぶん不正
                    Debug.Log("-----default-----");
                    throw new MyUniException(StaticError.ErrorType.TabunHusei);
            }
        }
    }

    //ログイン処理　MDN未登録だった(100) MAC値確認して 新規登録へ
    private void NewCreate_100(string hukugou, string mac)
    {
        //調べたいJson形式データは一度オブジェクトにデコードしてから再度JsonTextに変換  MAC値が正しく出ない
        JsonbaseClass jbc = JsonUtility.FromJson<JsonbaseClass>(hukugou);
        string hukugouText = JsonUtility.ToJson(jbc);
        if (CheckHash(hukugouText, mac))
        {
            GameObject.FindWithTag("Canvas").SetActive(false);
            fade.FadeOut();
            SceneManager.LoadScene("NewUniState_scene");
        }
        else
        {
            //改竄検知
            Debug.Log("ハッシュNG");
            throw new MyUniException(StaticError.ErrorType.LoginHashError);
        }
    }

    //ログイン時のみの処理(ログイン成功 102 詳細)
    private async Task Login_successful_102(string hukugouJsonText, string mac, string simpleClassJson, bool isFirstLogin)
    {
        //AES復号されたAngou_bun JsonTextからオブジェクトを作成します
        var myjson_obj = JsonUtility.FromJson<GetLoginClass>(hukugouJsonText);
        //調べたいJson形式データは一度オブジェクトにデコードしてから再度JsonTextに変換  MAC値が正しく出ない
        string jsonText = JsonUtility.ToJson(myjson_obj);
        //改竄検知用MAC値チェック
        if (CheckHash(jsonText, mac))   //第1引数　AES復号して取得したAngou_bun Jsonデータ　　第2引数　サーバ側で生成した改竄検知用ハッシュ
        {
            StaticMyClass.SetLoginEnd();
            GetComponent<PlayerObjectDataContainer>().SetLoginClass(myjson_obj, isFirstLogin, simpleClassJson);

            Destroy(GetComponent<Login>());         //Loginしたら用済み
            Destroy(GetComponent<SaveNewCharcterData>());//Loginしたら用済み

#if  UNITY_EDITOR ||　UNITY_ANDROID ||  UNITY_IOS || UNITY_STANDALONE_WIN

            //ここでLSB関連をFetch
            IFetchUnitStatus fetchUst = GetComponent<IFetchUnitStatus>();
            IFetchTerritorisData fetchTerr = GetComponent<IFetchTerritorisData>();
            ILoadUnitLeaders fetchUls = GetComponent<ILoadUnitLeaders>();
           Task t1 = fetchUst.FetchUnitsStatus();
            Task t2 = fetchTerr.FetchData();
            Task t3 = fetchUls.LoadUnitLeadersFromMyServer();
            var allTask= Task.WhenAll(t1, t2, t3);     //まとめて取得.失敗したら例外出て終了.Fetchしたら保持しておき,MainScene遷移後に各自展開する.

            try
            {
                await allTask;
            }
            catch (Exception ex)
            {
                if (allTask.Exception is AggregateException age)
                {
                    age.InnerExceptions.ToList().ForEach((ages) => Debug.LogError(ages.GetType()));        
                    throw new Exception();      //本番環境では落とす.
                }
            }

            Debug.Log("LSB DL End");
            AdvanceDownloadbyAAS advanceDL = GetComponent<AdvanceDownloadbyAAS>();
            advanceDL.DownloadEnd += Next;
            Debug.Log("======> Start S3");
            await advanceDL.DownloadDependenciesOfTheSpecifiedLabelFromMyServer();

#elif  UNITY_STANDALONE_LINUX
            Debug.Log("===Photon Start!===");
            punCtrl.ConnectPhoton();                                            //Photo  Room入室 & Mainシーン遷移
            Destroy(this);
#endif
        }
        else
        {
            //改竄検知
            Debug.Log("ハッシュNG");
            throw new MyUniException(StaticError.ErrorType.LoginHashError);
        }
    }

    //改竄検知用MAC値チェック
    /*引数に 調べたいJson形式データ(Angou_bun)と、送られてきたハッシュ値　　ハッシュ値が同じならtrue 違えばfalseを返す
      調べたいJson形式データは一度オブジェクトにデコードしてから再度JsonTextに変換したものを渡す(無駄なものがついてUnity側のMAC値が変わるっぽい)
     */
    private bool CheckHash(string getjson, string mac)
    {
        Debug.Log("MACチェック");
        Debug.Log("Server MAC:" + mac);
        bool ch = sha2.CheckMacNum(getjson, mac);
        return ch;
    }

    /// <summary>
    /// S3からのDL終了で呼ばれる.
    /// </summary>
    private void Next()
    {
        Debug.Log("Next");
        fade.QuickDarken();
        punCtrl.ConnectPhoton();
        Destroy(this);
    }


}
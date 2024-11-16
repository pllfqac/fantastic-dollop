using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

//新規登録         DB_connectionから分離した    リファクタリングを試みたが力不足だった
public class SaveNewCharcterData : MonoBehaviour
{


    //    public DB_connection db_con;
    public IncrementSequenceNumber seqnum;
    public AccessPreparation accessPre;
    // public ExclusiveEncrypt Xor;
    public _SHA256 sha2;            //sha256ハッシュ変換スクリプト
    public Receiving_result_branch receiving_result;      //データベース受信処理




    public async Task NewCharcterData(string cname, int cNumber)
    {
        IAES aes = GetComponent<_AES2>();
        WWWForm form = accessPre.AccessPreparationUniServer(StaticMyClass.loginEnd);
        //Jsonに格納して暗号化するクラスのメンバーに、渡したいデータを代入
        NewSetDataClass nsdc = new NewSetDataClass();
        nsdc.charname = cname;
        nsdc.charaNumber = cNumber;
        nsdc.rosumi = StaticMyClass.sikibetunum02;  //新規登録専用識別値　php側と同じにする
        nsdc.mac_key = sha2.GetMACKey;
        nsdc.id = seqnum.SequenceNumber;
        string str = JsonUtility.ToJson(nsdc);         //暗号化するクラスをJson形式に変換
        Debug.Log("Save data Json:" + str);

        aes.CreateKey();      //ランダム値を元にAES_Key生成
        Result_AES aesp = aes.Encrypt(str);     //JsonをAESで暗号化

        TradeServerClass gsn = new TradeServerClass();
        gsn.a = aesp.aes_data;
        gsn.AI = aesp.IV/*aes.siv*/ + "." + aes.aesKey;
        gsn.DB_Key = StaticMyClass.DB_KEY;
        gsn.at = PlayerPrefs.GetString("at");
        gsn.mn = sha2.CreateMacNum(str);       //改竄防止用MAC値生成,取得
        string go = JsonUtility.ToJson(gsn);     //Jsonでまとめる

        form.AddField("SendMyData", go);
        form.AddField("lncaf", StaticMyClass.loginKey);     //初回ログイン or 新規作成時の時のみ送る



        UnityWebRequest unityWebRequest = UnityWebRequest.Post(StaticMyClass.LoginFlowEndPoint/*db_con.loginFlowEndPoint*/, form);
        await unityWebRequest.SendWebRequest();

        switch (unityWebRequest.result)
        {
            case UnityWebRequest.Result.InProgress:
                Debug.Log("InProgress");
                break;
            case UnityWebRequest.Result.Success:
                await receiving_result.Login_Process(unityWebRequest.downloadHandler.text);    //受信処理 Receiving_result_branchへ
                break;
            case UnityWebRequest.Result.ConnectionError:
            //break;
            case UnityWebRequest.Result.ProtocolError:
            //break;
            case UnityWebRequest.Result.DataProcessingError:
            // break;
            default:
                Debug.LogWarning("responseCode " + unityWebRequest.responseCode + "  backError:" + unityWebRequest.downloadHandler.text);
                throw new MyUniException(StaticError.ErrorType.UniServerError);
                //  break;
        }
    }

}
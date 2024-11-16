using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//DB受信後の処理
/// <summary>
/// Serverから受信したJsonをデコード,AES復号,MAC値確認
/// </summary>
public class ReceivingReadData : MonoBehaviour
{

    //   public _AES aes;
    private IAES _aes;
    private IAES aes
    {
        get
        {
            if (_aes == null) _aes = GetComponent<_AES2>();
            return _aes;
        }
    }

    public _SHA256 sha2;

    public void ReceivingData<T>(string resultText)
    {
        TradeServerClass jc = JsonUtility.FromJson<TradeServerClass>(resultText);      //サーバからのJsonデータをデコード    
        Result_AES decr = aes.Decrypt(jc.a, jc.AI);               //Angou_bunをAES復号

        //All処理はまだ
        ReadOneClass<T> readDataobj = JsonUtility.FromJson<ReadOneClass<T>>(decr.hukugou_data);  //復号したものをデコード
        string re_json = JsonUtility.ToJson(readDataobj);
        bool checkh = CheckHash(re_json, jc.mn);
    }



    /* 改竄検知用MAC値チェック
     引数に 調べたいJson形式データ(Angou_bun)と、送られてきたハッシュ値　　ハッシュ値が同じならtrue 違えばfalseを返す
      調べたいJson形式データは一度オブジェクトにデコードしてから再度JsonTextに変換したものを渡す(無駄なものがついてUnity側のMAC値が変わるっぽい)
     */
    private bool CheckHash(string getjson, string mac)
    {
        bool ch = sha2.CheckMacNum(getjson, mac);
        return ch;
    }
}

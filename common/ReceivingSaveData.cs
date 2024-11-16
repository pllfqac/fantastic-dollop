using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//Single1.
public class ReceivingSaveData : MonoBehaviour
{

    //   public _AES aes;
    private IAES _aes;
    private IAES Aes
    {
        get
        {
            if (_aes == null) _aes = GetComponent<_AES2>();
            return _aes;
        }
    }

    public _SHA256 sha2;

    public void ReceivingData(string result)
    {
        TradeServerClass jc = JsonUtility.FromJson<TradeServerClass>(result);      //サーバからのJsonデータをデコード    
        Result_AES decr = Aes.Decrypt(jc.a, jc.AI);               //Angou_bunをAES復号
        ReceiveSaveClass rsc = JsonUtility.FromJson<ReceiveSaveClass>(decr.hukugou_data);   //復号したものをデコード

        string re_json = JsonUtility.ToJson(rsc);
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



    //例外
    private void GetException(Exception e)
    {
        Debug.Log("<color=Red>例外発生!</color>");
        Debug.Log(e.ToString());
    }
}

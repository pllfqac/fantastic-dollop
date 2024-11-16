using UnityEngine;
using System.Collections;
using System.Text;

//SHA256を扱うクラス  サーバと共有キーを持つのでMACと呼べる　はず
public class _SHA256 : MonoBehaviour
{
    private string pub_Key;          //MAC用の共有キー　　サーバと共有する　　このKeyはサーバ専用のSHA_keyとは関係ない

    //パスワードに使用する文字
    private static readonly string passwordChars = "0123456789abcdefghijklmnopqrstuvwxyz";
    //生成する文字列の長さ
    private const int len = 12;

    void Start()
    {
        pub_Key = GeneratePassword(len);        //MAC用の共有キー生成
        Debug.Log("publicKey:" + pub_Key);
        
        Encoding sjisEnc = Encoding.GetEncoding("UTF-8");
        int num = sjisEnc.GetByteCount("aiueo");
        Debug.Log("UTF-8 ByteNum:" + num);
    }

    //MAC用の共有キーを他のクラスから取得する　　プロパティにしてみました
    public string GetMACKey
    {
        get
        {
            return pub_Key;
        }
    }

    //一回だけのHash処理
    public string Sha256(string planeStr, string key)
    {
        // バイト化
        //Unicode 文字の UTF-8 エンコードを表します
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        //GetBytes(String):派生クラスでオーバーライドされた場合、指定した文字列に含まれるすべての文字をバイト シーケンスにエンコードします
        byte[] planeBytes = ue.GetBytes(planeStr);
        byte[] keyBytes = ue.GetBytes(key);


        // SHA256化
        //HMACSHA256(Byte[]):キー データを指定して、HMACSHA256 クラスの新しいインスタンスを初期化します。
        System.Security.Cryptography.HMACSHA256 sha256 = new System.Security.Cryptography.HMACSHA256(keyBytes);
        //ComputeHash(Byte[]):指定したバイト配列のハッシュ値を計算します。
        byte[] hashBytes = sha256.ComputeHash(planeBytes);


        // 文字列化
        string hashStr = "";
        foreach (byte b in hashBytes)
        {
            //string.Format:指定された形式に基づいてオブジェクトの値を文字列に変換し、別の文字列に挿入します。
            //string.Format("{0,0:x2}", b):数値を16進数表示
            hashStr += string.Format("{0,0:x2}", b);
        }
        return hashStr;
    }

    //改竄検知用MAC値チェック　　　引数に jsonし直したJson形式データ(調べたいデータ)と、送られてきたハッシュ値
    //ハッシュ値が同じならtrue 違えばfalseを返す
    public bool CheckMacNum(string checkData,string getData)
    {
        string st = checkData;
        for (int i = 0; i < 7; i++)
        {
            st = Sha256(st, pub_Key);         //jsonデータをハッシュ化  MAC用共有キーを使う
        }
        Debug.Log("Unity    MAC:" + st);

        return (st == getData ? true : false);
    }


    /// <summary>
    /// 改竄検知用MAC値生成.
    /// </summary>
    /// <param name="json">MAC値生成の対象となるJson文字列</param>
    /// <returns>MAC値</returns>
    public string  CreateMacNum(string json)
    {
        string g = json;
        for (int i = 0; i < 7; i++)
        {
            g = Sha256(g, pub_Key);         //jsonデータをハッシュ化  MAC用共有キーを使う
        }
        Debug.Log("Unity生成 MAC:" + g);
        return g;
    }

    /// <summary>
    /// 改竄検知用MAC値生成.鍵必要var.
    /// </summary>
    /// <param name="json">MAC値生成の対象となるJson文字列</param>
    /// <param name="publicKey">共通鍵</param>
    /// <returns></returns>
    public string CreateMacNum(string json,string publicKey)
    {
        string g = json;
        for (int i = 0; i < 7; i++)
        {
            g = Sha256(g, publicKey);         //jsonデータをハッシュ化  MAC用共有キーを使う
        }
        Debug.Log("Unity生成 MAC:" + g);
        return g;
    }
    /// <summary>
    /// MAC用の共有キーを生成する
    /// 文字をあらかじめ用意しておき、乱数でどれかの文字を選んでいくことによって、作成しています
    /// </summary>
    /// <param name="length">生成する文字列の長さ</param>
    /// <returns>生成された文字列</returns>
    private string GeneratePassword(int length)
    {
        StringBuilder sb = new StringBuilder(length);
        System.Random r = new System.Random();

        for (int i = 0; i < length; i++)
        {
            //文字の位置をランダムに選択
            int pos = r.Next(passwordChars.Length);
            //選択された位置の文字を取得
            char c = passwordChars[pos];
            //パスワードに追加
            sb.Append(c);
        }

        return sb.ToString();
    }

    
}

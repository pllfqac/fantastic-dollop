using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;


/// <summary>
/// Single1.
/// 暗号・復号をするクラス.
/// </summary>
public class _AES2 : MonoBehaviour, IAES
{
 


    private string _aesKey;
    /// <summary>
    /// DBと共有する共通鍵.
    /// クライアントが共有鍵(Key)を(自動)生成します.
    /// </summary>
    public string aesKey
    {
        get { return _aesKey; }
        private set
        {
            _aesKey = value;           //value:文字列にエンコードしたKey.ログイン毎に変わる.
        }
    }


    //CBCの初期化ベクトル（IV）は毎回違うものを生成しないといけないので、暗号化したデータと一緒にIVの値も渡さないと復号できない。
    //IV値は，公開しても構わないっぽい．暗号したものとセットでサーバーに渡す.


        /*Master,User共用の共通Keyについて
         key size=32bit固定なのでkeysizeが足りないとき(ownerIdとか使ってる),その値を元にhashだして32bitにしてそれをKeyとする.
         */

    //共通鍵(Key)の作成(自動生成)
    public void CreateKey()
    {
        // AES暗号サービスを生成
        var csp = new RijndaelManaged();
        csp.BlockSize = 128;   // 128bit 固定
        csp.KeySize = 256;     // 128/192/256bit から選択.256固定.
        csp.Mode = CipherMode.CBC;
        csp.Padding = PaddingMode.PKCS7;        //php側opensslはデフォでPKCS7らしいので合わせる.

        //鍵 を自動生成
        csp.GenerateKey();
        // 鍵を出力
        aesKey = Convert.ToBase64String(csp.Key);
    }


    #region Encrypt

    /// <summary>
    /// 暗号化.
    /// Keyは生成済みであること.
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public  Result_AES Encrypt(string plainText)
    {
        Result_AES result = new Result_AES();

        byte[] key = Convert.FromBase64String(this.aesKey);

        RijndaelManaged rijndael = new RijndaelManaged();     //AESでの暗号化は、RijndaelManagedというクラスを使います
        rijndael.Padding = PaddingMode.PKCS7;
        rijndael.Mode = CipherMode.CBC;
        rijndael.KeySize = 256;                                  //暗号化の鍵長とブロック長を設定します. Key:32byte
        rijndael.BlockSize = 128;
        // IV ( Initilization Vector ) は、Managedにつくらせる
        rijndael.GenerateIV();
        byte[] iv = rijndael.IV;

       // siv = Convert.ToBase64String(iv);              //送信用IVをstring型保存
        result.IV = Convert.ToBase64String(iv);              //送信用IVをstring型保存

        //新しいMemoryStreamオブジェクトをインスタンス化して、暗号化されたバイトを格納します
        MemoryStream memoryStream = new MemoryStream();

        // Aesオブジェクトから新しい暗号化装置をインスタンス化する.
        ICryptoTransform aesEncryptor = rijndael.CreateEncryptor(key,iv);

        // データを処理してメモリストリームに書き込むために、新しいCryptoStreamオブジェクトをインスタンス化します
        CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

        // plainText文字列をバイト配列に変換します
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        // 入力された平文文字列を暗号化します
        cryptoStream.Write(plainBytes, 0, plainBytes.Length);

        // 暗号化プロセスを完了します
        cryptoStream.FlushFinalBlock();

        // 暗号化されたデータをMemoryStreamからバイト配列に変換します
        byte[] cipherBytes = memoryStream.ToArray();

        // MemoryStreamとCryptoStreamの両方を閉じます
        memoryStream.Close();
        cryptoStream.Close();

        // 暗号化されたバイト配列をbase64でエンコードされた文字列に変換します
        string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

        result.aes_data = cipherText;
        return result;
    }


    /// <summary>
    /// 暗号化.引数に共通Keyを指定するver.
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="keyBaseStr">共通key(の元になる文字列).32bitに変換される.</param>
    /// <returns></returns>
    public Result_AES Encrypt(string plainText, string basekey)
    {
        Result_AES result = new Result_AES();
        SHA256 mySHA256 = SHA256.Create();
        byte[] key = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(basekey));
        byte[] aesKey = new byte[32];                       //Key:32byte固定         
        Array.Copy(key, 0, aesKey, 0, 32);

        RijndaelManaged rijndael = new RijndaelManaged();     //AESでの暗号化は、RijndaelManagedというクラスを使います
        rijndael.Padding = PaddingMode.PKCS7;
        rijndael.Mode = CipherMode.CBC;
        rijndael.KeySize = 256;                                  //暗号化の鍵長とブロック長を設定します. Key:32byte
        rijndael.BlockSize = 128;
        // IV ( Initilization Vector ) は、Managedにつくらせる
        rijndael.GenerateIV();
        byte[] iv = rijndael.IV;

        //siv = Convert.ToBase64String(iv);              //送信用IVをstring型保存
        result.IV = Convert.ToBase64String(iv);              //送信用IVをstring型保存


        //新しいMemoryStreamオブジェクトをインスタンス化して、暗号化されたバイトを格納します
        MemoryStream memoryStream = new MemoryStream();

        // Aesオブジェクトから新しい暗号化装置をインスタンス化する.
        ICryptoTransform aesEncryptor = rijndael.CreateEncryptor(aesKey, iv);

        // データを処理してメモリストリームに書き込むために、新しいCryptoStreamオブジェクトをインスタンス化します
        CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptor, CryptoStreamMode.Write);

        // plainText文字列をバイト配列に変換します
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        // 入力された平文文字列を暗号化します
        cryptoStream.Write(plainBytes, 0, plainBytes.Length);

        // 暗号化プロセスを完了します
        cryptoStream.FlushFinalBlock();

        // 暗号化されたデータをMemoryStreamからバイト配列に変換します
        byte[] cipherBytes = memoryStream.ToArray();

        // MemoryStreamとCryptoStreamの両方を閉じます
        memoryStream.Close();
        cryptoStream.Close();

        // 暗号化されたバイト配列をbase64でエンコードされた文字列に変換します
        string cipherText = Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

        result.aes_data = cipherText;
        return result;
    }

    #endregion







    #region Decrypt

    /// <summary>
    /// 復号化.
    /// </summary>
    /// <param name="cipherText">復号したいもの</param>
    /// <param name="siv"></param>
    /// <returns></returns>
    public Result_AES Decrypt(string cipherText, string siv)
    {
        Result_AES hukugouAES = new Result_AES();

        RijndaelManaged rijndael = new RijndaelManaged();
        rijndael.KeySize = 256;
        rijndael.BlockSize = 128;
        rijndael.Padding = PaddingMode.PKCS7;
        rijndael.Mode = CipherMode.CBC;

        rijndael.Key = Convert.FromBase64String(aesKey);
        rijndael.IV = Convert.FromBase64String(siv);

        // 復号化
        ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        byte[] fromEncrypt = new byte[cipherBytes.Length];

        MemoryStream memoryStream = new MemoryStream(cipherBytes);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        try
        {
            cryptoStream.Read(fromEncrypt, 0, fromEncrypt.Length);
            hukugouAES.hukugou_data = (Encoding.UTF8.GetString(fromEncrypt));
        }
        finally
        {
            memoryStream.Close();
            cryptoStream.Close();
        }
        return hukugouAES;
    }



    /// <summary>
    /// 復号化.引数に共通Keyを指定するver.
    /// </summary>
    /// <param name="prm_text_to_decrypt">復号したい文字列</param>
    /// <param name="iv">Initialize Vector</param>
    /// <param name="basekey">共通鍵(の元になる文字列)</param>
    /// <returns></returns>
    public Result_AES Decrypt(string cipherText, string iv, string basekey)
    {
        Result_AES hukugouAES = new Result_AES();

        SHA256 mySHA256 = SHA256.Create();
        byte[] key = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(basekey));
        byte[] aesKey = new byte[32];                       //Key:32byte固定         
        Array.Copy(key, 0, aesKey, 0, 32);

        RijndaelManaged rijndael = new RijndaelManaged();
        rijndael.KeySize = 256;
        rijndael.BlockSize = 128;
        rijndael.Padding = PaddingMode.PKCS7;
        rijndael.Mode = CipherMode.CBC;

        rijndael.Key = aesKey;
        rijndael.IV = Convert.FromBase64String(iv);

        // 復号化
        ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        byte[] fromEncrypt = new byte[cipherBytes.Length];

        MemoryStream memoryStream = new MemoryStream(cipherBytes);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        try
        {
            cryptoStream.Read(fromEncrypt, 0, fromEncrypt.Length);
            hukugouAES.hukugou_data = (Encoding.UTF8.GetString(fromEncrypt));
        }
        finally
        {
            memoryStream.Close();
            cryptoStream.Close();
        }
        return hukugouAES;
    }

   
    #endregion

}

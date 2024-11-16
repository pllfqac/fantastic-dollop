using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAES
{

    string aesKey { get; }
   // string siv { get; }


    /// <summary>
    /// 共通鍵(Key)の作成.
    /// </summary>
    void CreateKey();

    //===================暗号化===================
    /// <summary>
    /// 暗号化.
    /// </summary>
    /// <param name="prm_text_to_encrypt"></param>
    /// <returns>生成された暗号文とIVはここに入る</returns>
    Result_AES Encrypt(string prm_text_to_encrypt);
    /// <summary>
    /// 共通Keyを指定するver.
    /// </summary>
    /// <param name="prm_text_to_encrypt">暗号化したい平文.</param>
    /// <param name="keyBaseStr">Key.</param>
    /// <returns></returns>
    Result_AES Encrypt(string prm_text_to_encrypt, string keyBaseStr);


    //===================復号化===================
    /// <summary>
    /// 復号.
    /// </summary>
    /// <param name="prm_text_to_decrypt"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    Result_AES Decrypt(string prm_text_to_decrypt, string iv);
    /// <summary>
    /// 共通Keyを指定するver.
    /// </summary>
    /// <param name="prm_text_to_decrypt">復号したい文字.</param>
    /// <param name="iv"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    Result_AES Decrypt(string prm_text_to_decrypt, string iv, string key);

}

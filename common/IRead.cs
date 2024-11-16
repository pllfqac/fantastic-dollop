using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public interface IRead 
{

    //Status取得.
    void ReadOne<T>(string ReadName);       //T is String or int
    void ReadAll();

    /// <summary>
    /// UserOnly.
    /// endPointで指定したPHPファイルへアクセストークンでアクセスしデータを取得する.
    /// クエリ情報を使用.
    /// </summary>
    /// <param name="endPoint">URL. StaticMyClassで定義してるはず.</param>
    /// <returns>何らかの文字列.</returns>
    Task<string> ReadData(string endPoint);

    /// <summary>
    /// KnowledgeデータをDBから取得する.
    /// クエリ情報を使用.
    /// </summary>
    /// <returns></returns>
    Task<string> ReadKnowledge(string id);

    /// <summary>
    /// 全領土情報・ 全SquadのStatusを取得する.
    /// ぞれぞれEndPointが違う.
    /// </summary>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    Task<string> ReadLSBData(string endPoint);
    /// <summary>
    /// 所持する全てのUnitLeaderを取得する.
    /// </summary>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    Task<string> ReadULData(string endPoint);

}

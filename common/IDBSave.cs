using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public interface IDBSave 
{

    /// <summary>
    /// DBテーブルのUpdate.
    /// Postで送信する.送信データはAESで暗号化される.
    /// </summary>
    /// <param name="endPoint">接続先PHPファイル.</param>
    /// <param name="saveData">DB Saveしたい何らかの文字列.暗号化される.</param>
    /// <returns>okでもPHP側でエラー発生でもnull.</returns>
    Task<string> DBUpdate(string endPoint, string saveData);
}

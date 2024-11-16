using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
public interface IKnowledge {


    /// <summary>
    /// PHP側から送られてきた「待ち時間の残り[s]+どの本(bookのAbs) [2要素]+  knowledgeLevelをピリオドで連結した文字列[現状6要素]」をKnowledgeに展開する.
    /// </summary>
    /// <param name="str">Knowledgeの値のみをピリオドで連結した文字列</param>
    void Init(string str);
    /// <summary>
    /// 読もうとする本が今の自分のレベルと適正か(現在Level+1 == 読もうとする本のLevel)の確認.
    /// </summary>
    /// <param name="book">読もうとする本.</param>
    /// <returns>適正ならTrue.適正でなければfalse.</returns>
    bool CanReadBook(BookProperty book);

    /// <summary>
    /// 本を読むのには時間がかかる.このTimerが作動中は他の本を読むことが出来ない.
    /// </summary>
    void StartReading(BookProperty book);

    /// <summary>
    /// MasterのDB Update用にStringに変換.
    /// DB側は全部上書きされる.
    /// </summary>
    /// <returns></returns>
     string GetParameterbyString();

    int GetNowLevelbyKnowledgeType(StaticMyClass.KnowledgeType type);

    /// <summary>
    /// 知識LevelからJsonを作成する.
    /// </summary>
    string CreateSerializeKnowledgeLevel();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Userへの通知.
/// </summary>
public interface INotificationToUser
{
	/// <summary>
	/// LevelUpしたとき
	/// </summary>
	void NoticeLevelUp(int level);

	/// <summary>
	/// Skill LevelUPしたとき.
	/// </summary>
	void NoticeSkillLevelUp(string sName,int skilllevel);
	/// <summary>
	/// KnowledgeLevelUpしたとき
	/// </summary>
	/// <param name="Level"></param>
	/// <param name="type">SimpleInt2Classに(int Level, int knowledgeTypeInt)でまとめてJsonで送られてくる.KnowledgeTypeに変換する.</param>
	void NoticeKnowledgeLevelUp(int newLevel, StaticMyClass.KnowledgeType type);


	/// <summary>
	/// レアドロ,ガチャでアイテムを取得した時の表示.
	/// </summary>
	/// <param name="itemName">取得したItem名.</param>
	void NoticeGetItem(string itemName);
	void NoticeGetItem(IReadOnlyList<string> itemNames);
	/// <summary>
	/// Expを取得した時.
	/// </summary>
	/// <param name="exp"></param>
	void NoticeGetExp(int exp);



	/// <summary>
	/// OverTextへそのまま表示.
	/// </summary>
	/// <param name="message"></param>
	void ShowMessage(string message);

}

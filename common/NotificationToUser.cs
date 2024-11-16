using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using TMPro;
/// <summary>
/// Player.
/// Userへの各お知らせ.
/// Chat,Effect,通知板,SEの使用.
/// </summary>
public class NotificationToUser : MonoBehaviour, INotificationToUser
{
	[NonSerialized]
	public ConsoleTextCore chat;
	private IPanelSlider panelSlider;
	//Text
	[NonSerialized]
	public NotificationTextCtrl notificationText;

	[NonSerialized]
	public TextMeshProUGUI overText;
	//Effect
	private IParticleAction particleAction;
	//SE



	private void Start()
	{
		panelSlider = GameObject.FindWithTag("NoticePanel").GetComponent<IPanelSlider>();
		particleAction = transform.Find("Particles_Common").GetComponent<IParticleAction>();
	}



	public void NoticeLevelUp(int level)
	{
		chat.ShowContentText("Level UP!!");
		particleAction.LevelUpParticlePlay();
		notificationText.ShowLevelUpText();
		//SE
	}
	

	/// <summary>
	/// Skill Level Upしたときの通知.
	/// </summary>
	/// <param name="abs"></param>
	/// <param name="skilllevel"></param>
	public void NoticeSkillLevelUp(string sName, int skilllevel)
	{
		//Debug.Log("<color=red>Skill Abs:" + abs + "  が" + skilllevel + "　にLevel Upしました!!</color>");
		chat.ShowContentText("Skill :" + sName + "  が " + skilllevel + StaticMyClass.noticeMessage2);
		notificationText.ShowSkillLevelUpText();
		//SE
	}

	/// <summary>
	/// Knowledge Level Upした時. MasterからRaiseされる.
	/// </summary>
	/// <param name="json"></param>
	public void NoticeKnowledgeLevelUp(int newLevel,StaticMyClass.KnowledgeType type)
    {
		chat.ShowContentText("知識:" + type.ToString() + " が " + newLevel + StaticMyClass.noticeMessage2);
    }


	/// <summary>
	/// レアドロ,ガチャでアイテムを取得した時.看板&Chat欄への表示.
	/// </summary>
	/// <param name="itemName"></param>
	public void NoticeGetItem(string itemName)
    {
		Debug.Log("NoticeGetItem ");

		chat.ShowContentText(itemName + StaticMyClass.noticeMessage1);
		panelSlider.NoticeRareDropAnimation(itemName);
    }

	/// <summary>
	/// レアドロ,ガチャでアイテムを取得した時,Chat欄への表示.
	/// 複数Ver.
	/// </summary>
	/// <param name="itemNames"></param>
	public void NoticeGetItem(IReadOnlyList<string> itemNames)
    {
		Debug.Log("NoticeGetItem Lis");
		foreach(string str in itemNames)
        {
			chat.ShowContentText(str + StaticMyClass.noticeMessage1);
        }

		panelSlider.NoticeRareDropAnimation(itemNames);
    }

	/// <summary>
	/// Expを取得した時.
	/// </summary>
	/// <param name="exp"></param>
	public void NoticeGetExp(int exp)
    {
		chat.ShowContentText( exp.ToString() + StaticMyClass.noticeMessage3);
    }


	public void ShowMessage(string message)
    {
		StartCoroutine(ShowText(message));
	}

	private IEnumerator ShowText(string mess)
	{
		overText.text = mess;
		overText.enabled = true;
		yield return new WaitForSeconds(StaticMyClass.TextShowTime);
		overText.enabled = false;
	}


}

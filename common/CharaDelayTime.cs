using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Player.
/// Skill,Item使用時のDelayTime.
/// </summary>
public class CharaDelayTime : MonoBehaviour,ICharaDelayTime
{

	[System.NonSerialized]
	public IShowIconDelayTime showIconDelayTime;        //instat.UI表示用.		
	private ItemBoxManager ItemBoxManager;              //ロジック側での入力許可制御用.
	//DelayTime中にParalizeになった場合,DelayTime終了でItemBoxManager.AcceptItemboxButton()による入力解禁を防ぐための確認.
	private ICheckParalize checkParalize;              

	private void Start()
	{
		ItemBoxManager = GetComponent<ItemBoxManager>();
		checkParalize = GetComponent<ICheckParalize>();
	}

	public async Task StartDelayTime(float delayTime)
	{
		ItemBoxManager.NotAcceptItemboxButton();
		showIconDelayTime.StartDelayTimeAnimation(delayTime);		//UI表示
		Debug.Log(" 前Delay Start: " + delayTime + " s.    ミリ秒変換→　"+ (int)(delayTime * 1000));
		await Task.Delay((int)(delayTime*1000));        //s→millis
		if(!checkParalize.IsParalized) ItemBoxManager.AcceptItemboxButton();	//Paralize状態のときはParalize解除RPC受信で入力禁止解禁
	}

	
}

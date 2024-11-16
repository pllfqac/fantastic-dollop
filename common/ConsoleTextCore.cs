using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System.Linq;
using Photon.Pun;

//Main_Scene->Canvas-ChatPaneel-SendMessageInputField.

/// <summary>
/// Photon ChatのUI部分.
/// InputFieldへの入力とChat表示用Textの管理.
/// </summary>
public class ConsoleTextCore : MonoBehaviour
{
	//SelectSnedTargetPanel->Toggle GroupのAllow Switch Off=True の場合入力を無視する仕様.

	/// <summary>
	/// 送信メッセージ入力.
	/// </summary>
	[SerializeField]
	private InputField _inputField=null;

	/// <summary>
	/// 受信メッセージ表示用.
	/// </summary>
	[SerializeField]
	private Text ConsoleText=null;
	[SerializeField]
	private ScrollRect ScrollRect=null;
	private IMyPhotonChatManager myPhotonChatManager = null;
	private ChatTargetSelectUI chatTarget;                  //メッセージ送信グループの選択

	/// <summary>
	/// メッセージの表示が何もない時のみfalse.
	/// </summary>
	private bool entered = false;


	void Start()
	{
		myPhotonChatManager = GameObject.FindWithTag("single2").GetComponent<IMyPhotonChatManager>();
		var punCtrl = GameObject.FindWithTag("single1").GetComponent<PUNController>();
		if (PhotonNetwork.IsMasterClient && punCtrl.TargetRoom== PUNController.Room.CommonRoom) return;		//UniMas Only
		chatTarget = transform.parent.GetComponent<ChatTargetSelectUI>();
		InitInputField();
	}

	
	//ChatのInputField入力完了で呼ばれる.
	public void OnInput()
	{
		if (myPhotonChatManager.IsInterval || string.IsNullOrEmpty(_inputField.text)) return;
		string str = CheckMessageMaxLength(_inputField.text);
		string target = null;
		MyPhotonChatManager.TargetMessageType messageType = SelectDestinationMessageType();

		switch (messageType)
		{
			case MyPhotonChatManager.TargetMessageType.SceneMessage:
				target = GetDestinationSceneChannel();
				myPhotonChatManager.SendSceneMessage(target, str);
				break;
			case MyPhotonChatManager.TargetMessageType.PartyMessage:
				myPhotonChatManager.SendPartyMessage(str);
				break;

			case MyPhotonChatManager.TargetMessageType.PrivateMessage:
				target = GetDestinationOneToOneTargetName(out str, str);        //targetとmessageに分割して,outでmessageを取得する
				if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(str)) break;
				myPhotonChatManager.SendPrivateMessage(target, str);
				//1:1Chatのときは自身の表示用に成形する.
				str = target + ":" + str;
				break;
			case MyPhotonChatManager.TargetMessageType.none:str = null;
				break;
		}

		if (!entered) PlaceholderTextClear();
		InitInputField();

	}

	/// <summary>
	/// 送信できる最大文字数オーバーの確認.
	/// </summary>
	/// <param name="msg"></param>
	/// <returns>オーバーしてたら切り捨てた文字列.オーバーしてなければそのまま.</returns>
	private string CheckMessageMaxLength(string msg)
	{
		if (msg.Length >StaticMyClass.maxShowMessageLength) return msg.Substring(0,StaticMyClass.maxShowMessageLength);  //先頭から最大表示可能文字列ぶんのみを取得して返す.
		else return msg;
	}

	/// <summary>
	/// メッセージの表示.
	/// </summary>
	/// <param name="text"></param>
	public void ShowContentText(string text)
	{
		//念のため悪質なUser対策のため受信側でも入力文字列を確認.
		if (!CheckReceiveMessage(text)) return;

		ConsoleText.text += "\n" + text;

		//行数制限確認.改行の数を取得
		int lineCount = ConsoleText.cachedTextGenerator.lineCount;
		Debug.Log("line Count:" + lineCount);

		if (lineCount >= StaticMyClass.chatMaxNumberOfLines)
		{
			int newLineIndex = ConsoleText.text.IndexOf("\n", 1);
			Debug.Log("NewLineIndex:" + newLineIndex);
			ConsoleText.text = ConsoleText.text.Remove(0, newLineIndex);
		}

		ConsoleText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
		ScrollRect.verticalNormalizedPosition = 0;  //ここでスクロールを一番下にする
		if (!entered) PlaceholderTextClear();
	}

	/// <summary>
	/// 念のため悪質なUser対策のため受信側でも入力文字列を確認.
	/// 禁止ワード確認もここで?.
	/// </summary>
	/// <param name="mess">受信メッセージ.</param>
	/// <returns>問題なければTrue.</returns>
	private bool CheckReceiveMessage(string msg)
    {
		if (msg.Length > StaticMyClass.maxShowMessageLength) return false;

		return true;
	}


	private void InitInputField()
	{		
		_inputField.text = "";						// 値をリセット
		ScrollRect.verticalNormalizedPosition = 0;	//ここでスクロールを一番下にする
	}

	/// <summary>
	/// Messageが表示されたときのPlaceholderのText(Enter text...)の消去.
	/// </summary>
	private void PlaceholderTextClear()
    {
		this.transform.Find("Placeholder").gameObject.SetActive(false);//GetComponent<Text>().enabled = false;
		entered = true;
    }

	/// <summary>
	/// 送信先選択Sliderの状態から送信対象を定義したEnumを返す.
	/// </summary>
	private MyPhotonChatManager.TargetMessageType SelectDestinationMessageType()
	{
		switch (chatTarget.selectedValue/*sendMessageTargetSelectSlider.value*/)
		{
			case 1: return MyPhotonChatManager.TargetMessageType.SceneMessage;
			case 2: return MyPhotonChatManager.TargetMessageType.PartyMessage;
			case 3: return MyPhotonChatManager.TargetMessageType.PrivateMessage;
			default: return MyPhotonChatManager.TargetMessageType.none; 
		}
		/*
		if (sceneToggle.isOn) return MyPhotonChatManager.TargetMessageType.SceneMessage;
		else if (partyToggle.isOn) return MyPhotonChatManager.TargetMessageType.PartyMessage;
		else if (oneToOneToggle.isOn) return MyPhotonChatManager.TargetMessageType.PrivateMessage;
		else return MyPhotonChatManager.TargetMessageType.none;/// 全Toggleがoffの場合noneを返す. */
	}


	//====================================Channel名 or Target名取得====================================

	/// <summary>
	/// SceneChatの場合の送信先Channeの取得.
	/// 今自身が居るScene名を取得して,それをChannel名とする
	/// </summary>
	/// <returns></returns>
	private string GetDestinationSceneChannel()
	{
		return SceneManager.GetActiveScene().name;					//AASのSceneで使用できる?  <-たぶんおｋ
	}



	/// <summary>
	/// 1:1Chatの場合の送信先Userキャラ名の取得.
	/// </summary>
	/// <param name="sendMessage">送信メッセージ文字列.</param>
	/// <param name="inputStr">InputFieldに入力された文字列</param>
	/// <returns>送信先Userキャラ名 or Null.</returns>
	private string GetDestinationOneToOneTargetName(out string sendMessage,string inputStr)
	{
		if (!inputStr[0].Equals('@')) {
			Debug.Log("１文字目は@で始めます");
			sendMessage = null;
			return null;     //1:1が選択されていても最初の文字列が「/」でなければ1:1通信とみなさずnullを返す.
		}

		Debug.Log("-");

		//Split(Char[], Int32) 第2引数->返される最大の部分文字列数
		string[] tokens = inputStr.Split(new char[] { ' ' },2);     //tokens[0]に送信先Userキャラ名.tokens[1]にメッセージが格納されてる　はず

		if (tokens[0].Equals("@help"))
		{
			//隠しコマンド的な
			Debug.Log("help");
			sendMessage = null;
			return null;
		}

		//	if (string.IsNullOrEmpty(tokens[1]))  //文字列によってはIndexOutOfRangeExceptionがでる
		if(tokens.Length<2)
		{
			//メッセージ本文がない場合もNull返す
			Debug.Log("--------");
			sendMessage = null;
			return null;
		}

		Debug.Log("1:1Chat 送信先:" + tokens[0] + "   Message:" + tokens[1]);
		sendMessage = tokens[1];
		return  tokens[0].Substring(1);        //送信先キャラ名の先頭についている「\」を削除し,送信したいキャラ名を返す
	}

	//============================================================================================================

}

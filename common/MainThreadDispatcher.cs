using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Single2.
/// コルーチンを処理するためのオブジェクト
/// </summary>
public class MainThreadDispatcher : MonoBehaviour
{
	private static MainThreadDispatcher instance;

	public static MainThreadDispatcher Instance
	{
		get
		{
			if (instance == null)
			{
				//とりあえず雑に
				instance =GameObject.FindObjectOfType<MainThreadDispatcher>();
			}
			return instance;
		}
	}

	/// <summary>
	/// 登録されたコルーチンを実行する
	/// </summary>
	/// <param name="coroutine">対象のコルーチン</param>
	/// <param name="callback">終了時のコールバック</param>
	public void RegisterCoroutione(IEnumerator coroutine, Action callback)
	{
		StartCoroutine(WorkCoroutine(coroutine, callback));
	}

	/// <summary>
	/// コルーチンを実行し、終了時にコールバック関数を実行する
	/// </summary>
	private IEnumerator WorkCoroutine(IEnumerator target, Action callback)
	{
		yield return target;
		callback();
	}
}

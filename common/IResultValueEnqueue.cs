using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Hitしたタイミングで表示するための調整用Queueへの値の追加.
/// </summary>
public interface IResultValueEnqueue
{
	//==================Damage==================

	/// <summary>
	/// Damage用Queueに追加.
	/// </summary>
	/// <param name="damageInfoClass"></param>
	void OneDamageInfoClassEnqueue(IReadOnlyList<OneDamageInfoClass> damageInfoClass);

	/// <summary>
	/// Damage用Queueに追加.
	/// </summary>
	void OneDamageInfoClassEnqueue(int? value, bool isCritical);


	//==================UCC==================

	/// <summary>
	/// UCC用Queueに追加.
	/// </summary>
	void UccEnqueue(UccInfoClass uic);
}
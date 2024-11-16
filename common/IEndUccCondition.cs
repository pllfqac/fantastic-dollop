using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 全状態異常強制終了.
/// 実装スクUCCとUCUは実行時にどちらかがDestroyされるので,このInterface参照をGetcomponentするときはどちらかのClassを明示的に取得しなければならない
/// </summary>
public interface IEndUccCondition
{
	void ForceEndUCondition();
}

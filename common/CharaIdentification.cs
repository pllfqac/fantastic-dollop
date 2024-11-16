using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player.
/// このコンポーネントを付けたキャラオブジェクトがどのCharaTypeかの定義.
///	PhotonInstntiateされたキャラのTypeを知る事が可能.
/// </summary>
public class CharaIdentification : MonoBehaviour {

	//CharaType+CharaStyle　の値をCharaNumberとする.

	//インスぺから設定する.
	[SerializeField]
	private StaticMyClass.CharaType CharaType = StaticMyClass.CharaType.A;


	public StaticMyClass.CharaType GetCharaType()
	{
		return CharaType;
	}

}

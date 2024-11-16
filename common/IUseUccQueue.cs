using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUseUccQueue 
{

	/// <summary>
	/// UCCキューからデキューする.
	/// </summary>
	void UseUccQueue(UccInfoClass uccInfoClass,StaticMyClass.SendStatus sendStatus);
}

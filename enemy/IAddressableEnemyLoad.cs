using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


/// <summary>
/// Addressable Assets System対応のEnemyをS3からLoadする.
/// </summary>
public interface IAddressableEnemyLoad
{
	// Task LoadEnemyFromServer();

	//S3からダウンロードしたものをセットする.
	Task EnemyLoadAndSetScriptable();
}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System;

/// <summary>
/// Single1.
/// プレイヤーがゲームを最初に起動したときに依存関係のダウンロードをする
/// </summary>
public class AddressableLoad : MonoBehaviour,IAddressableLoad
{
	public event Action<float> downloadAction;
	//public event Action DLEnd;
	/*
	 結論的には
	LoadAssetAsync 後に Instantiate したオブジェクトは Destroy で消せばOK。
	ただし、アセットはメモリにロードしたままになるので、
	必要がなくなったら、Addressales.Release()で解放しましょう。
	 */


	/// <summary>
	/// S3にAASの更新データがあるか確認する.
	/// </summary>
	/// <param name="targetStr"></param>
	/// <returns>更新データのサイズ.[byte]</returns>
	public async Task<long> CheckDownloadSizeAsync(string targetStr)
	{
		var size = await Addressables.GetDownloadSizeAsync(targetStr).Task;
		Debug.Log("Size:" + size);          //Load Path が LocalLoadPath の場合は容量が常に 0 になる っぽい
		return size;
	}

    /// <summary>
    /// 事前ダウンロード.
	/// AssetBundles を早期にキャッシュすると、LoadAssetAsync などの最初の呼び出しで、
	/// 操作の一部としてバンドルをダウンロードする必要があった場合のパフォーマンスの向上につながります
    /// ダウンロードされた AssetBundle は、エンジンの AssetBundle キャッシュに格納されます
    /// </summary>
    /// <param name="targetLabel"></param>
    /// <returns></returns>
    public async Task LoadFromMyServerAsync(string targetLabel)
	{
		Debug.Log("DownloadDependenciesAsync:" + targetLabel);
		/*第2引数に true を渡すとアセットバンドルダウンロード直後にアンロードする.事前ダウンロードしたアセットバンドルをダウンロード直後に使用する場合は false を渡す*/
		// 依存関係の読み込み.MainThreadOnly.
		AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(targetLabel, true);    //ラベルを指定してダウンロードする.True指定でハンドルを自動的に解放
        handle.Completed += LoadResult;           //ダウンロード完了時の処理を登録
		StartCoroutine(DownloadWait(handle, targetLabel));
		await handle.Task;
		Debug.Log("-");
	}

	private IEnumerator DownloadWait(AsyncOperationHandle download, string targetStr)
	{
		while (!download.IsDone)
		{
			//ダウンロード中の処理を書く
	//		Debug.Log("※" + targetStr + " ダウンロード中 " + (download.PercentComplete * 100) + "  %  ");
			if (downloadAction != null) downloadAction(download.PercentComplete);                   //UI表示
			yield return null;
		}
	}

	//ダウンロード完了時の処理
	private void LoadResult(AsyncOperationHandle handle)
	{
		if (AsyncOperationStatus.Succeeded == handle.Status)
		{
			//ダウンロード成功
			Debug.Log("ダウンロード成功! ");
			//if (DLEnd != null) DLEnd();
		}
		else
		{
			//ダウンロード失敗
			throw new MyUniException(StaticError.ErrorType.EnemyLoadError);
		}
	}


	/*
	 事前にダウンロードする
		Addressables.DownloadDependenciesAsync（）メソッドを呼び出すと、渡したアドレスまたはラベルの依存関係がロードされます。通常、これはアセットバンドルです。
		この呼び出しによって返されるAsyncOperationHandle構造体には、ダウンロードの進行状況を監視および表示するために使用できるPercentComplete属性が含まれています。
		コンテンツがロードされるまでアプリを待機させることもできます。

		ダウンロードする前にユーザーに同意を求める場合は、Addressables.GetDownloadSize（）を使用して、指定されたアドレスまたはラベルからコンテンツを
		ダウンロードするために必要な容量を返します。
		これは、Unityのアセットバンドルキャッシュに残っている以前にダウンロードしたバンドルを考慮に入れることに注意してください。

		アプリのアセットを事前にダウンロードすることは有利ですが、そうしないことを選択する場合があります。例えば：
		アプリに大量のオンラインコンテンツがあり、一般的にユーザーがその一部のみを操作することを期待している場合。
		機能するためにオンラインで接続する必要があるアプリがあります。アプリのすべてのコンテンツが小さなバンドルに含まれている場合、
		必要に応じてコンテンツをダウンロードすることを選択できます。
		パーセント完了値を使用してコンテンツがロードされるまで待つのではなく、プリロード機能を使用してダウンロードが開始されたことを示してから続行できます。
		この実装では、必要な時間までにアセットの読み込みが完了していないインスタンスを処理するための読み込み画面または待機画面が必要になります。
		 */

}
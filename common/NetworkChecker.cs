using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/*インターネットのアクセス状態を安価に判断します。
 *アクセス状態に応じて　bool値を返す
 * SingletonManager1に所属
 */
public class NetworkChecker : MonoBehaviour
{

    private readonly string checkUrl = "http://" + StaticMyClass.ApacheIP + "/connect_p/NetworkChecker.php";

    /// <summary>
    /// インターネットに接続されている確認する.
    /// </summary>
    /// <returns>接続されているならTrue.繋がってなければFalse.</returns>
    public async Task<bool> Network_flag()
    {
        bool result = false;
        // ネットワークの状態を出力
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                Debug.Log("ネットワークには到達不可");
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                Debug.Log("Wifiまたはケーブル経由で到達可能");
                /*internetReachabilityは接続の設定を確認しているだけなので、実は本当にインターネットに接続出来るかを確認する事は出来ません.さらにモバイル端末用*/
                result = await Check();
                break;
        }
        Debug.Log(result);
        return result;
    }


    public async Task<bool> Check(int timeOut = 3)
    {
        UnityWebRequest request = UnityWebRequest.Get(checkUrl);
        request.timeout = 3;
        Debug.Log("CHECK");
        await request.SendWebRequest();

        switch (request.result)
        {
            case UnityWebRequest.Result.InProgress: Debug.Log("InProgress"); return false;        //?
            case UnityWebRequest.Result.Success:
                Debug.Log("インターネット接続確認成功");
                Debug.Log(request.downloadHandler.text);
                return true;
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.Log("インターネット接続確認失敗");
                return false;
            default: return false;
        }
    }


}

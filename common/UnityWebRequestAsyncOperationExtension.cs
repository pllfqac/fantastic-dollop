using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// UnityWebRequestをAwaitableにするための拡張.
/// </summary>
public static class UnityWebRequestAsyncOperationExtension  {
    
     // SendWebRequestの返り値はUnityWebRequestAsyncOperation。Awaitableにするには、これにAwaiterを取得するGetAwaiterメソッドを実装する必要がある。
    
        
    public static UnityWebRequestAsyncOperationAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOperation)
    {
        return new UnityWebRequestAsyncOperationAwaiter(asyncOperation);
    }
    
}

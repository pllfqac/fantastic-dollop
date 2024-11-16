using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.Networking;
using System;

public class UnityWebRequestAsyncOperationAwaiter : INotifyCompletion
{
    UnityWebRequestAsyncOperation _asyncOperation;

    public bool IsCompleted
    {
        get { return _asyncOperation.isDone; }
    }

    public UnityWebRequestAsyncOperationAwaiter(UnityWebRequestAsyncOperation asyncOperation)
    {
        _asyncOperation = asyncOperation;
    }


    public void GetResult()
    {
        // NOTE: 結果はUnityWebRequestからアクセスできるので、ここで返す必要性は無い
    }

    public void OnCompleted(Action continuation)        //引数で与えられるcontinuationは、awaitの後の処理がActionとしてパックされたようなもの
    {
        _asyncOperation.completed += _ => { continuation(); };
    }
}

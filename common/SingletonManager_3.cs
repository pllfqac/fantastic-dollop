using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonManager_3 : MonoBehaviour
{
    //static: 新しくインスタンス化しても変数の中身を保持する
    static public SingletonManager_3 instance3;

    //MainSceneに戻ったときに呼ばれる.
    void Awake()
    {
        //SingletonManager_3インスタンスがなかったら
        if (instance3 == null)
        {
            //このSingletonManager_3をインスタンスとする
            instance3 = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //SingletonManager_2インスタンスが存在したら 今回インスタンス化したSingletonManager_2を破棄
            Destroy(this.gameObject);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonManager_LoadingCircleCanvas : MonoBehaviour
{
    //static: 新しくインスタンス化しても変数の中身を保持する
    static public SingletonManager_LoadingCircleCanvas circle;

    //MainSceneに戻ったときに呼ばれる.他スクでAwakeが再び呼ばれる.
    void Awake()
    {
        //なかったら
        if (circle == null)
        {
            //これをインスタンスとする
            circle = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //存在したら 今回インスタンス化したのを破棄
            Destroy(this.gameObject);
        }

    }
}

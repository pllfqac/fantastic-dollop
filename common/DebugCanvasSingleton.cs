using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCanvasSingleton : MonoBehaviour {


    //static: 新しくインスタンス化しても変数の中身を保持する
    static public DebugCanvasSingleton canvas_instance;


    void Awake()
    {
        //Singleton_CanvasManagerインスタンスがなかったら
        if (canvas_instance == null)
        {
            //このSingleton_CanvasManagerをインスタンスとする
            canvas_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Singleton_CanvasManagerインスタンスが存在したら 今回インスタンス化したSingleton_CanvasManagerを破棄
            Destroy(this.gameObject);
        }
    }

}

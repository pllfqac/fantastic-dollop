using UnityEngine;
using System.Collections;

//シングルトンパターンを利用してSceneを遷移してもデータ保持するスクリプト  Canvas版
public class Singleton_CanvasManager : MonoBehaviour {

    //static: 新しくインスタンス化しても変数の中身を保持する
    static public Singleton_CanvasManager canvas_instance;


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

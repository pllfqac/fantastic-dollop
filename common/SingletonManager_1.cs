using UnityEngine;
using System.Collections;

//シングルトンパターンを利用してSceneを遷移してもデータ保持するスクリプト　　　Title画面からのSingletonManager_1オブジェクト用
public class SingletonManager_1 : MonoBehaviour {


    //static: 新しくインスタンス化しても変数の中身を保持する
    static public SingletonManager_1 instance;


    void Awake()
    {
        //SingletonManagerインスタンスがなかったら
        if (instance == null)
        {
            //このSingletonManagerをインスタンスとする
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //SingletonManagerインスタンスが存在したら 今回インスタンス化したSingletonManagerを破棄
            Destroy(this.gameObject);
        }

    }
   

}

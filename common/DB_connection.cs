using UnityEngine;


/// <summary>
/// Single1.
/// Test or 本番を選択してデータベース接続先を選択する.
/// </summary>
public class DB_connection : MonoBehaviour
{

    /// <summary>
    /// TrueでLocalhostに接続.
    /// falseでリモートに接続.
    /// </summary>
    [SerializeField]
    private bool isTest;

    //Test4に設定したEndPointか StaticMyClassに設定したEndPointか 
    public string AgriEndPoint
    {
        get { return isTest ? GameObject.FindWithTag("single3").GetComponent<Test4>().endPoint : StaticMyClass.AgriEndPoint; }
    }



}
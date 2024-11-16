using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace MySpace
{

    /// <summary>
    /// CloudBuildしたものの簡易的な確認.
    /// </summary>
    public class Version : MonoBehaviour
    {
        //Inspectorから変更できるようにするため
        [SerializeField]
        private string MyGameVersion;



        private void Awake()
        {
            Debug.Log("MyGameVersion:" + MyGameVersion);
        }
    }
}


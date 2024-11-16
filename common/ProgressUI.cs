using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Title_Scene or NewSettingScene.
/// ProggressbarPanel.
/// AddressableのDownloadの進捗を表示したり.
/// </summary>
public class ProgressUI : MonoBehaviour
{
    [SerializeField]
    private Slider progressbar;
    [SerializeField]
    private Text text;

    private long[] sizes;              //Task.whenAllで取得したDLサイズ.順番注意.

    /// <summary>
    /// DL中のLabelのindex.
    /// </summary>
    public int taskIndex { private get; set; }


    private void Start()
    {
        GameObject.FindWithTag("single1").GetComponent<AdvanceDownloadbyAAS>().progressUIPanel = this.gameObject;     //参照を持たせておく.
        this.gameObject.SetActive(false);
        Debug.Log("progress Start");
    }



    public void ShowProgressMessage(string message  )
    {
        text.text = message;
    }

    /// <summary>
    /// S3から取得するデータのサイズをセットする.
    /// </summary>
    /// <param name="sizes">各要素(e.g."Enemy","Equipment")の更新サイズ.</param>
    public void SetTotalSize(long[] sizes)
    {
        this.sizes = sizes;
    }



    /// <summary>
    /// DL進捗率のUI表示.
    /// 複数要素を順にDLしてそれを一つのDLと見せるように表示できる.
    /// </summary>
    /// <param name="progressValue">(このindexの)現在の進捗率.0～1のfloat値が入ることを想定.</param>
    public void ChengeSlider(float progressValue)
    {
        float currentProgress = sizes[taskIndex] * progressValue;
        //前の要素がDL済みならそれも加算する.
        for (int i = 0; i < taskIndex; i++)
        {
            currentProgress += sizes[i];
        }

        progressbar.value = currentProgress / sizes.Sum();
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Single1.
/// Game開始時に行うAASによる事前ダウンロード.
/// </summary>
public class AdvanceDownloadbyAAS : MonoBehaviour
{

    /// <summary>
    /// S3から事前DLしたいAssetsのLabelをInspeで指定する.
    /// </summary>
    [SerializeField]
    private List<string> labels = new List<string>();

    /// <summary>
    /// すべてのDL終了で呼ぶ.
    /// </summary>
    public Action DownloadEnd;

    [NonSerialized]
    public GameObject progressUIPanel = null;
    private ProgressUI progressUI;
    private IAddressableLoad load;

    /// <summary>
    /// 各DLサイズ.
    /// </summary>
    private long[] sizes;



    /// <summary>
    /// 指定したLabelのすべての依存関係をダウンロードする.
    /// </summary>
    /// <returns></returns>
    public async Task DownloadDependenciesOfTheSpecifiedLabelFromMyServer()
    {
#if  UNITY_STANDALONE_LINUX
        DownloadEnd();
#else
        progressUIPanel.SetActive(true);
        progressUI = progressUIPanel.GetComponent<ProgressUI>();
        //S3からダウンロード.
        Debug.Log("S3からダウンロード");

        //まず更新データの確認
        progressUI.ShowProgressMessage(StaticMyClass.progressMsg1);
        load = this.gameObject.GetComponent<IAddressableLoad>();

        //Inspeで指定したLabel名を元にサバから各Sizeの取得.
        List<Task<long>> longs = new List<Task<long>>();
        foreach (var labelName in labels)
        {
            Debug.Log("<color=blue>Label Name:</color>" + labelName);
            longs.Add(load.CheckDownloadSizeAsync(labelName));
        }

        sizes = await Task.WhenAll(longs.ToArray());
        Debug.Log("<color=orange>Size Sum:</color> " + sizes.Sum());
        //更新データがあれば取得する.
        progressUI.SetTotalSize(sizes);
        load.downloadAction += progressUI.ChengeSlider;

        foreach (var (label, index) in labels.Select((label, index) => (label, index)))
        {
            Debug.Log("Index:" + index + "  Label:" + label);
            progressUI.taskIndex = index;
            if (sizes[index] != 0) await load.LoadFromMyServerAsync(label);
        }
        DownloadEnd();
#endif
    }

}

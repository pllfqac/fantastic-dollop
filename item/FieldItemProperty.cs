using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// Prefabs-Objects-FieldItem.
/// Game開始時は無効?
/// </summary>
public class FieldItemProperty : MonoBehaviour, IFieldItemProperty
{
    /// <summary>
    /// Field Itemの光ってるパーティクル本体.このスクの子のはず.
    /// </summary>
    [SerializeField]
    private ParticleSystem particle;

    /// <summary>
    /// 設置個所番号.
    /// 重複は無いように.
    /// </summary>
    [SerializeField, Tooltip("設置個所番号.重複は不可.")]
    private int InstallationLocationNumber;
    [SerializeField]
    private int ItemAbs;
    /// <summary>
    /// このFI取得に必要なItem.
    /// なければ0でおｋ.
    /// e.g.つるはし,オノ,釣り竿
    /// </summary>
    [SerializeField, Tooltip("このFieldItemを取得するのに必要なアイテム.なければ0でおｋ.")]
    private int RequiredItem;

    /// <summary>
    /// 再び同じSceneに入ってもTimerが生きている
    /// </summary>
    private FieldItemSingleton waitMaster;

    //User側の再生までのRandom待ち時間中はTrue.最短180秒(仮)
    private bool waiting;


    private void Awake()
    {
        waitMaster = GameObject.FindWithTag("single2").GetComponent<FieldItemSingleton>();
    }

    private void Start()
    {

        if (waitMaster.ExistWaitList(this.InstallationLocationNumber)) WaitTimeStart();     //すでにListに存在する場合は新しいwaitTimeに取り換え.
        else if (Random.Range(0, 2) == 0) WaitTimeStart();                                  //Scene開始直後に取得できるFIの一部は遅れて取得可能とする.
        else Restart();
    }

    /// <summary>
    /// このFieldItemObjectがTapされたとき.
    /// 設定されたFieldItemのデータを取得する.
    /// </summary>
    /// <returns>isWait:待ち時間がある時はTrue.</returns>
    public (bool isWait, int installationLocationNum, int abs, int requiredItem) GetFieldItemProp()
    {
        bool current = this.waiting;
        if (!waiting) WaitTimeStart();    //Itemが取得可能なら次の再生までのTimer開始.
        return (current, this.InstallationLocationNumber, this.ItemAbs, this.RequiredItem);
    }

    public void Restart()
    {
        this.waiting = false;
        if (particle != null) particle.Play();
    }

    //Scene切替で再生待ち時間がリセットされてしまうのでTask.Run
    public async void WaitTimeStart()
    {
        this.waiting = true;
        particle.Stop();
        int randomWaitValue = Random.Range(StaticMyClass.ShortestReproductionTIme, StaticMyClass.MaximumReproductionTIme);  //[s]
        await waitMaster.FieldItemWait(this.InstallationLocationNumber, randomWaitValue);
        Restart();
    }



}

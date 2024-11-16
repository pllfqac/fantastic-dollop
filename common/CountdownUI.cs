using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
/// <summary>
/// Canvas.(Prefabs-UI-Slider_HealthBar_Default)
/// 待ち時間系のUI層.
/// UIの処理だけで実際のTimerとは別のCountDownをしている.
/// 似た名前のProgressUIとは無関係.
/// </summary>
public class CountdownUI : MonoBehaviour,ICountdownUI
{
    private float countDownTime {  get; set; }

    private float tempMaxTime;  //割合を出すためのTemp.
    [SerializeField]
    private TextMeshProUGUI countText = null;
    [SerializeField]
    private Slider timeSlider = null; 
   // [SerializeField]
    private bool d;
  //  private TimeSpan timeSpan;

    /// <summary>
    /// インスペでも設定.
    /// Trueだと「時間経過で非Active」にならない
    /// </summary>
    public bool disableHide;

    /// <summary>
    /// UIを動かし始める.
    /// </summary>
    /// <param name="countDownTime">[s]</param>
    public void StartTimerUI(float countDownTime)
    {
        if (countDownTime < 0) return;
        this.gameObject.SetActive(true);
        this.countDownTime = countDownTime;
        tempMaxTime = countDownTime;
        countText.enabled = true;
        timeSlider.enabled = true;
       // timeSpan = new TimeSpan();
        d = true;
    }

    /// <summary>
    /// UIを動かし始める.
    /// Timerを中断した場合用.全体の時間も知る必要があるため.
    /// </summary>
    /// <param name="remainingTime">残り時間[s].</param>
    /// <param name="allTime">全部の残り時間[s].</param>
    public void StartTimerUI(float remainingTime,float allTime)
    {
        if (remainingTime < 0) return;
        Debug.Log("Start Timer UI");
        this.gameObject.SetActive(true);
        this.countDownTime = remainingTime;
        tempMaxTime = allTime;
        countText.enabled = true;
        timeSlider.enabled = true;
        d = true;
    }

    /// <summary>
    /// UITimer停止.
    /// </summary>
    public void StopTimerUI()
    {
        d = false;
    }

    private void Update()
    {
        if (!d) return;
        countText.text = new TimeSpan(0, 0, (int)countDownTime).ToString();//String.Format("{0:00}", countDownTime);
        countDownTime -= Time.deltaTime;

        timeSlider.value = countDownTime / tempMaxTime;
        if (countDownTime <= 0.0f)
        {
            countDownTime = 0;
            d = false;
           if(!disableHide) Invoke(nameof(CountdownUI.HideObject),3);               //少し待って非Active
        }        
    }

    //時間経過で非Active
    private void HideObject()
    {
        d = false;
        timeSlider.enabled = false;
        countText.enabled = false;
        this.gameObject.SetActive(false);       //消える前に何かエフェクト,SE
    }

    //別の農地のTimerUI実行中なら非表示にしたいため.
    private void OnDisable()
    {
        this.gameObject.SetActive(false);
    }
}

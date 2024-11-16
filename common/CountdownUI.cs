using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
/// <summary>
/// Canvas.(Prefabs-UI-Slider_HealthBar_Default)
/// �҂����Ԍn��UI�w.
/// UI�̏��������Ŏ��ۂ�Timer�Ƃ͕ʂ�CountDown�����Ă���.
/// �������O��ProgressUI�Ƃ͖��֌W.
/// </summary>
public class CountdownUI : MonoBehaviour,ICountdownUI
{
    private float countDownTime {  get; set; }

    private float tempMaxTime;  //�������o�����߂�Temp.
    [SerializeField]
    private TextMeshProUGUI countText = null;
    [SerializeField]
    private Slider timeSlider = null; 
   // [SerializeField]
    private bool d;
  //  private TimeSpan timeSpan;

    /// <summary>
    /// �C���X�y�ł��ݒ�.
    /// True���Ɓu���Ԍo�߂Ŕ�Active�v�ɂȂ�Ȃ�
    /// </summary>
    public bool disableHide;

    /// <summary>
    /// UI�𓮂����n�߂�.
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
    /// UI�𓮂����n�߂�.
    /// Timer�𒆒f�����ꍇ�p.�S�̂̎��Ԃ��m��K�v�����邽��.
    /// </summary>
    /// <param name="remainingTime">�c�莞��[s].</param>
    /// <param name="allTime">�S���̎c�莞��[s].</param>
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
    /// UITimer��~.
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
           if(!disableHide) Invoke(nameof(CountdownUI.HideObject),3);               //�����҂��Ĕ�Active
        }        
    }

    //���Ԍo�߂Ŕ�Active
    private void HideObject()
    {
        d = false;
        timeSlider.enabled = false;
        countText.enabled = false;
        this.gameObject.SetActive(false);       //������O�ɉ����G�t�F�N�g,SE
    }

    //�ʂ̔_�n��TimerUI���s���Ȃ��\���ɂ���������.
    private void OnDisable()
    {
        this.gameObject.SetActive(false);
    }
}

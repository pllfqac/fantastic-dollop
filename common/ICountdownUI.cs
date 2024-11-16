using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 待ち時間系UIの制御.
/// </summary>
public interface ICountdownUI
{
    /// <summary>
    /// 最初からTimerを始める用.
    /// </summary>
    /// <param name="countDownTime">[s]</param>
    void StartTimerUI(float countDownTime);
    /// <summary>
    /// 途中からTimerを始める用.
    /// </summary>
    /// <param name="remainingTime">[s]</param>
    /// <param name="allTime">[s]</param>
    void StartTimerUI(float remainingTime, float allTime);

}
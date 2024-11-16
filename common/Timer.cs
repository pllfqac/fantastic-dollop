using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Threading;



namespace MySpace
{
    /// <summary>
    /// 待ち時間汎用Timer.
    /// </summary>
    public class Timer : IMyTimer
    {
        private System.Threading.Timer t;           //途中経過を知るために必要.

        private DateTime startTime;

        /// <summary>
        /// Timer終了までの待ち時間.
        /// 途中で止めた場合は再開した時からの時間が入る.
        /// </summary>
        private float tempWaitTime;       //[ms]
        /// <summary>
        ///  Timer動作中のみTrue.
        /// </summary>
        public bool isRunning { get; private set; }

        /// <summary>
        /// Timerを作成してStartする.
        /// </summary>
        /// <param name="waitTime">Timer終了までの時間[s].</param>
        /// <param name="endAction">Timer終了後に行いたい処理.この処理はMainThreadで実施される</param>
        public void StartTimer(float waitTime, Action endAction)
        {
            var context = SynchronizationContext.Current;

            //タイマー終了で呼ばれる
            TimerCallback tc = state => {
             //   Debug.Log(" タイマー終了!!  スレッドID:" + Thread.CurrentThread.ManagedThreadId);
                // タイマーを停止
                t.Change(Timeout.Infinite, Timeout.Infinite);
                t.Dispose();
                t = null;
                isRunning = false;
                //MainThreadで実施.
                context.Post(_ =>
                {
                    endAction();
                }, null);
            };

            isRunning = true;
            startTime = DateTime.Now;
            tempWaitTime = waitTime*1000;

          //  Debug.Log("スレッドID:" + Thread.CurrentThread.ManagedThreadId);
            // タイマーの生成&開始
            t = new System.Threading.Timer(tc, null, (int)(waitTime*1000), 0);
        }
    

        public void StartIntervalTimer(float waitTime,Action intervalAction)
        {
            var context = SynchronizationContext.Current;

            //インターバルで呼ばれる
            TimerCallback tc = state => {
               // Debug.Log(" インターバル終了!!  スレッドID:" + Thread.CurrentThread.ManagedThreadId);              
                //MainThreadで実施.
                context.Post(_ =>
                {
                    intervalAction();
                }, null);
            };
            isRunning = true;
            startTime = DateTime.Now;
            tempWaitTime = waitTime * 1000;
            
            t = new System.Threading.Timer(tc, null, (int)(waitTime * 1000), (int)(waitTime * 1000));    // タイマーの生成&開始
        }

        /// <summary>
        /// 残り時間[s]を取得する.
        /// </summary>
        public float GetRemainingTime()
        {
            TimeSpan ts = DateTime.Now - startTime;         //経過した時間.
            double tsmilliSec = ts.TotalMilliseconds;
            //   Debug.Log("経過 milliSec[ms] " + tsmilliSec);
            float ms = (float)(tempWaitTime - tsmilliSec);   //待ち時間から経過時間を引く.
            return ms / 1000;                                //ミリ秒から秒に変換する
        }

        /// <summary>
        /// 途中で止めて,残り時間[s]を取得.
        /// 止める前に動いていないときは0.
        /// </summary>
        /// <returns>残り時間[s].</returns>
        public float StopTimer()
        {
            if (!isRunning) return 0;

            tempWaitTime = GetRemainingTime()*1000;    //残り時間を更新
            isRunning = false;
            t.Change(Timeout.Infinite, Timeout.Infinite);
            //小数点以下四捨五入.
            return GetRemainingTime(); 
        }

        /// <summary>
        /// 再開する.停止中のみ受け付ける.
        /// </summary>
        public void ReStartTimer()
        {
            if (isRunning || tempWaitTime == 0) return;
            startTime = DateTime.Now;
            t.Change((int)tempWaitTime, 0);
            isRunning = true;
        }



        //↓allTimeは間違いの元になるので以下メソッドは廃
     /*   /// <summary>
        /// 残り時間[s]を取得する.
        /// </summary>
        /// <returns>
        /// remainingTime:残り時間[s],allTime:Timer全時間[s].
        /// allTimeはStartTimerを呼んだ時に指定した値なので,作業再開時に呼んだときは定義時間と変わることに注意.
        /// Timer全時間は農地の作業・育成の定義時間を想定.残り時間の割合を出すために全時間が必要なため.</returns>
              public (float remainingTime, float allTime) GetRemainingTime()
              {
                  TimeSpan ts = DateTime.Now - startTime;         //経過した時間.
                  double tsmilliSec = ts.TotalMilliseconds;
               //   Debug.Log("経過 milliSec[ms] " + tsmilliSec);
                  float ms= (float)(tempWaitTime - tsmilliSec);   //最初に指定した待ち時間から経過時間を引く.
                  return (ms / 1000,tempWaitTime/1000);                               //ミリ秒から秒に変換する
              }
      */
    }
}
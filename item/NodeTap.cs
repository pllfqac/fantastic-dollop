using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



/// <summary>
/// Prefab-ItemNode.
/// Item用.
/// 長押しの管理.
/// </summary>
public class NodeTap : MonoBehaviour, IPointerDownHandler,IPointerUpHandler, IPointerClickHandler
{
    //<仕様>
    //長押し確定だとDragは出来ない.逆に,Dragする場合は長押しが出来ない.

    private DragMoveObject dragObj;
    private INodeData node;
    [NonSerialized]
    public ItemPanelManager ipm;        //Node生成時
   

    //StopCoroutineのためにCoroutineで宣言しておく
    private  Coroutine PressCorutine;
    private  bool isPressDown = false;

    /// <summary>
    /// 長押し確定中はTrue.
    /// </summary>
    public bool IsLongPressRunning { get; private set; }

 
    private void Start()
    {
        dragObj = GetComponent<DragMoveObject>();
        node = GetComponent<INodeData>();
        IsLongPressRunning = false;
    }

 

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        //連続でタップした時に長押しにならないよう前のCoroutineを止める
        if (PressCorutine != null)
        {
            StopCoroutine(PressCorutine);
        }
        //StopCoroutineで止められるように予め宣言したCoroutineに代入
        PressCorutine = StartCoroutine(TimeForPointerDown());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        if ((dragObj!=null && !dragObj.isDragging) || dragObj==null)
        {
            Debug.Log("Short Press Done");
            //お好みの短押し時の挙動をここに書く(無い場合は書かなくても良い)
           if(!IsLongPressRunning) node.TapNode();
        }
        IsLongPressRunning = false;
        isPressDown = false;
    }


  /*  public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        if (isPressDown && !dragObj.isDragging)
        {
            Debug.Log("Short Press Done");
            isPressDown = false;

            //お好みの短押し時の挙動をここに書く(無い場合は書かなくても良い)
            if (!IsLongPressRunning) node.TapNode();
        }
        IsLongPressRunning = false;
        Debug.Log("IsLongPressRunning = false");
    }*/

    //長押しコルーチン
    private IEnumerator TimeForPointerDown()
    {
        //プレス開始
        isPressDown = true;

        //待機時間
        yield return new WaitForSeconds(StaticMyClass.PressTime);

        //押されたまま  && Dragしていない状態なら長押しの挙動
        if ((isPressDown && dragObj!=null && !dragObj.isDragging) || dragObj==null)
        {
            Debug.Log("Long Press Done");
            IsLongPressRunning = true;
            //お好みの長押し時の挙動をここに書く
            if (ipm == null)
            {
                isPressDown = false;
                yield break;
            }
            else ipm.ShowItemThrowAwayVerificationPanel(node);
        }
        //プレス処理終了
      //  isPressDown = false;
    }


    //使わなくても要るっぽい.これがないとOnDropが呼ばれなくなる.
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
    }
}

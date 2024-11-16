using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Canvas-Configurations-ItemCountChangePanel.
/// ItemShop or ItemDelete or 合成でのUserによるItem個数の選択UI処理.
/// </summary>
public class ItemCountChangePanelCtrl : MonoBehaviour {

    /// <summary>
    /// 個数入力用Panel(ItemCountChangePanel)のOKボタンが押されたときに呼ばれる.
    /// </summary>
    public UnityAction<byte> itemCountEntered;

    [SerializeField]
    private TextMeshProUGUI itemCountText=null;     //個数表示用UI.

    /// <summary>
    /// Select in the state of "buy" or "sell".
    /// Sellの場合itemCountの最大値はそのアイテムの所持数になる(既にD&Dで選択されている場合それも加味).
    /// </summary>
    private byte maxValue=1;     

    /// <summary>
    /// 個数の実体.「購入」時最大256まで.「売却」時最大値=User所持数.
    /// </summary>
    private byte _itemCount;
    public byte itemCount
    {
        get
        {
            return _itemCount;
        }
       private set
        {
            if (value >= maxValue) _itemCount = maxValue;
            else if (value <= 0) _itemCount = byte.MinValue;
            else _itemCount = value;
        //    Debug.Log("get set  value:" + value);
            itemCountText.text = _itemCount.ToString();     //UI変更.
        }
    }


    private void Awake()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// このPanelが呼び出されるときに初期化.デフォ=1.
    /// </summary>
    private void OnEnable()
    {
     //   Debug.Log("OnEnable");
        itemCountText.text = 1.ToString();
        itemCount = 1;
    }

    private void OnDisable()
    {
        itemCountEntered = null;
    }

    /// <summary>
    /// このPanel上の選択できるアイテム個数の最大値をセット.
    /// </summary>
    /// <param name="maxvalue"></param>
    public void Init(byte? maxvalue)
    {
        maxValue = maxvalue == null ?  byte.MaxValue : (byte)maxvalue;
    }

    /// <summary>
    /// CountUPボタンイベント.いずれSwipeに変更予定.
    /// </summary>
    public void OnCountUpButton()
    {
        ++itemCount;
    }

    public void OnCountDownButton()
    {
        --itemCount;
    }
    
    /// <summary>
    /// 「OK」buttonが押されたとき.
    /// </summary>
    public void OnOkButton()
    {
        itemCountEntered(itemCount);
    }
}

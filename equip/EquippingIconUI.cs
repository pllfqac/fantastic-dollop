using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Canvas.
public class EquippingIconUI : MonoBehaviour, IEquippingIconUI
{

  /// <summary>
  /// 装備中であることを示す「E」マーク(仮)を設定したimageオブジェクトプレファブ.
  /// </summary>
  [SerializeField]
  private RectTransform equippingIcon = null;

  //===============Master debug用.==================
  //  [SerializeField]
  //  private ItemScrollController itemScrlCtrl=null;
  //================================================



  /// <summary>
  ///  所持アイテムリスト上の「装備した」アイテムに,装備中であることを示す「E」マーク(仮)を付ける.
  ///  ItemNode-NodeImage　の子に,EquippingIconプレファブを生成する.
  /// </summary>
  /// <param name="targetItemNode">「E」マーク(仮)を付けたいItemNode</param>
  public void AddEquippingIcon(RectTransform targetItemNode)
  {
    RectTransform eq = Instantiate(equippingIcon) as RectTransform;
    eq.name = equippingIcon.name;           /*プレハブから作成されたGameObjectは「EquippingIcon(Clone)」と(Clone)がつくため注意*/
    eq.transform.SetParent(targetItemNode.transform.Find("NodeImage").transform);       //NodeImageの子にする.
                                                                                        //  eq.anchoredPosition = new Vector2(0, 0);
    eq.localPosition = new Vector3(0, 0, 0);
  }


  /// <summary>
  /// ItemNode-NodeImage　の子の,EquippingIconオブジェを削除する.
  /// </summary>
  /// <param name="deleteTargetNode">EquippingIconオブジェを削除したいItemNode.
  /// Unequip時その装備のGUIDをkeyに,所持リストからitemNodeを探してそれを指定.</param>
  public void DeleteEquippingIcon(RectTransform deleteTargetNode)
  {
    if (deleteTargetNode == null) return;
    Transform eqIconObj = deleteTargetNode.Find("NodeImage/EquippingIcon");
    if (eqIconObj == null) return;
    Destroy(eqIconObj.gameObject);
  }


}

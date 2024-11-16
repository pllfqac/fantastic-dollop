using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Null Object パターン用.
/// </summary>
public class NullUserItemOwnTable : IUserOwnItemTable {

    private int serialNum;

    public NullUserItemOwnTable(int i)
    {
        serialNum = i;              //LogingようにUserSerialを取得.
    }

    public IGetEquipStatusDefinitionTwoOver twoOver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event UpdateItemCountDelegate UpdateItemCountEvent;

    public bool ContainItem(Dictionary<int, OneAbsItemClass> checkItemTable)
    {
        throw new NotImplementedException();
    }

    public OneAbsItemClass GetOneAbs(int abs)
    {
        throw new NotImplementedException();
    }

    public OneItemClass GetOneItemClass(string guid)
    {
        throw new NotImplementedException();
    }

    /*  public bool ContainItem(OneAbsItemClass checkItem)
      {
          Debug.Log("NullObject.COntain");
          return false;                                   //何が正しいか不明.
      }*/

    public Dictionary<int, OneAbsItemClass> GetUserOwnItemTable()
    {
        return null;
    }

    public void SetEvent(UpdateItemDelegate newAdd, UpdateItemDelegate deleteItem, UpdateItemDelegate updateItem)
    {
        throw new NotImplementedException();
    }

    public void SetOwnItemTable(string json)
    {
        
    }

    public bool UpdateItem(Dictionary<int, OneAbsItemClass> Itemtable)
    {
        //もし呼ばれたらエラーロギング.
        if (PhotonNetwork.IsMasterClient) ErrorLoging.ExceptionLogingForMaster(serialNum, StaticError.ErrorType.CallNullObject);
        return false;
    }

    public void UpdateItem(OneAbsItemClass oai)
    {
        throw new NotImplementedException();
    }
}

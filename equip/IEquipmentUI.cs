using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 装備用UI.
/// </summary>
public interface IEquipmentUI  {

    //「装備する」
    void EquipSetUI(ItemNodeProp eNodeProp);
    //「外す」
    void UnequipSetUI(CharacterEquipmentPlace.EquipPlaceType placeType);
}

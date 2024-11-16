using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ItemNode-EquipmentLevelStarPanel.
/// EquipLevelを表す5つの★の表示管理.
/// </summary>
public class EquipLevelStarUI : MonoBehaviour
{
    [SerializeField]
    private ItemNodeProp myProp = null;

    [SerializeField]
    private  Image[] stars = new Image[5];



    public void OnEnable()
    {
        //自身のNodeのItemNodePropからEquipなら★の表示をする
        if (myProp!=null && myProp.oneAbs != null &&  myProp.oneAbs.definition!=null && myProp.oneAbs.definition.itemType == ItemDefinition.ItemType.Equipment 
            && myProp.oneItem != null && myProp.oneItem.eStatus != null) ShowEquipLevelStart(myProp.oneItem.eStatus.level);
        else ShowEquipLevelStart(0);
    }

    /// <summary>
    /// EquipLevelに対応する★の表示.
    /// eLevel1で無印の点に注意.
    /// </summary>
    /// <param name="showLevel">Equip Level.1～6</param>
    public void ShowEquipLevelStart(int showLevel)
    {
        switch (showLevel)
        {
            case 1: Equiplevel1(); break;
            case 2: Equiplevel2(); break;
            case 3: Equiplevel3(); break;
            case 4: Equiplevel4(); break;
            case 5: Equiplevel5(); break;
            case 6: Equiplevel6(); break;
            default:Equiplevel1();break;
        }
    }

    //EquipのLevelは1以上から(つまりeLevel=0のEquipは存在しない)

    private void Equiplevel1()
    {
        stars[0].enabled = false;
        stars[1].enabled = false;
        stars[2].enabled = false;
        stars[3].enabled = false;
        stars[4].enabled = false;
    }

    private void Equiplevel2()
    {
        stars[0].enabled = true;
        stars[1].enabled = false;
        stars[2].enabled = false;
        stars[3].enabled = false;
        stars[4].enabled = false;
    }
    private void Equiplevel3()
    {
        stars[0].enabled = true;
        stars[1].enabled = true;
        stars[2].enabled = false;
        stars[3].enabled = false;
        stars[4].enabled = false;
    }
    private void Equiplevel4()
    {
        stars[0].enabled = true;
        stars[1].enabled = true;
        stars[2].enabled = true;
        stars[3].enabled = false;
        stars[4].enabled = false;
    }
    private void Equiplevel5()
    {
        stars[0].enabled = true;
        stars[1].enabled = true;
        stars[2].enabled = true;
        stars[3].enabled = true;
        stars[4].enabled = false;
    }
    private void Equiplevel6()
    {
        stars[0].enabled = true;
        stars[1].enabled = true;
        stars[2].enabled = true;
        stars[3].enabled = true;
        stars[4].enabled = true;
    }

}

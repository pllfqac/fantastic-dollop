using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// シナリオ上で取得できるアイテムの作成.
/// </summary>
public interface IMasterScenarioItemCreate
{

    /// <summary>
    /// シナリオアイテムの作成.
    /// </summary>
    /// <param name="playerActorNum"></param>
    /// <returns></returns>
    OneAbsItemClass ScenarioItemCreate(int createItemAbs);
}
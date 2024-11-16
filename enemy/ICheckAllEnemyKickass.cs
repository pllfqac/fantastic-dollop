using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Master用.
/// シナリオシーンでEnemyを全部倒したか否かの確認.
/// </summary>
public interface ICheckAllEnemyKickass
{

    /// <summary>
    /// シナリオシーンでEnemyを全部倒したか否かの確認.
    /// </summary>
    /// <returns>全部倒したらUser(関連するPTメンバを含む)へ撃破完了Raise.</returns>
    void  CheckAllEnemyKickass(int sceneIndex, int enemyViewId);
}
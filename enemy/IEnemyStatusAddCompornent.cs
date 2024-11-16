using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyStatusAddCompornent  {

    //このInterfaceを実装したClassのメンバ変数をcscインスに入れる.パラメータのCommonStatusClassインスはpoolから生成して使う.
    void GetEnemyStatus(CommonStatusClass csc);
    EnemySkillClass GetEnemySkills();

  //  int GetSceneIndex();

    byte _sceneIndex { get; set; }
}

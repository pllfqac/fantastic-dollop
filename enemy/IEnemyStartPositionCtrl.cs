using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyStartPositionCtrl
{
    Vector2 GetESP(int enemyId, int sceneIndex);
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IEnemyDelayAction
{
    event Action enemyDeadAction;
}
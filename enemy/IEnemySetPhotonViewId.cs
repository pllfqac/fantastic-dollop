using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemySetPhotonViewId  {
    void SetPhotonView(GameObject obj, int? viewId);
}

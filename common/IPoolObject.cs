using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プールするオブジェクトはこれを継承する.
public interface IPoolObject  {

    void Init(PoolBody poolbody,RectTransform myRect);
    void RePool();
}

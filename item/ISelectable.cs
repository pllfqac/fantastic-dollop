using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    //ItemScrlCtrlスクのitemNodeListからGUIDをKeyにItemNodePropを取得する.
    ItemNodeProp SelectNodeProp(string GUID);

}

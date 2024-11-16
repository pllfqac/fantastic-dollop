using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// List<T>をJsonで送れるようにするClass.
[Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}

/*使い方 例
    ①SimpleIntClassを必要な分オブジェクトにしてデータを入れる.
        SimpleIntClass mc = new SimpleIntClass();
        mc.id = 1;
        mc.value = 1;

        SimpleIntClass mc2 = new SimpleIntClass();
        mc2.id = 12;
        mc2.value = 12;

     ② ①で作成したオブジェを List<SimpleIntClass>に加える.
     var lis = new List<SimpleIntClass>();
            lis.Add(mc);
            lis.Add(mc2);

    ③ ②のListを元にSerializationインスタンス作成してそれをJsonにする.
            Serialization<SimpleIntClass> ss = new Serialization<SimpleIntClass>(lis);
            string st = JsonUtility.ToJson(ss);
            Debug.Log(st);

    ④  Json文字列 -> List<T> に戻すときは以下のようにする.
        List<myclass> mcl = JsonUtility.FromJson<Serialization<myclass>>(st).ToList();

*/

// Dictionary<TKey, TValue>
[Serializable]
public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;

    Dictionary<TKey, TValue> target;
    public Dictionary<TKey, TValue> ToDictionary() { return target; }

    public Serialization(Dictionary<TKey, TValue> target)
    {
        this.target = target;
    }

    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public void OnAfterDeserialize()
    {
        var count = Math.Min(keys.Count, values.Count);
        target = new Dictionary<TKey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            target.Add(keys[i], values[i]);
        }
    }
}


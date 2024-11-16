using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class CopyHelper
{
    //https://tomisenblog.com/c-sharp-deepcopy/
    //使い方: 変数.DeepCopy()と書くことでコピーすることができます。
    //var destMember = srcMember.DeepCopy();



    /// <summary>
    /// DeepCopy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="src"></param>
    /// <returns></returns>
    public static T DeepCopy<T>(this T src)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, src);
            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }
    }
}
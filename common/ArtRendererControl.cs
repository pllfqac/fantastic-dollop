using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Single2.
/// fps低下防止のため,ConfigurationPanel等を開いたときにArtとGroundのレンダリングを消す.
/// </summary>
public class ArtRendererControl : MonoBehaviour,IArtRendererControl {


    /// <summary>
    /// そのSceneのArt(GroundとかWall).
    /// 1Sceneに1つのはず.
    /// </summary>
    public GameObject Art {private get; set;}
    //そのSceneのTerrain.
    public Terrain Terrain { private get; set; }

    /// <summary>
    /// Artオブジェクト以下のMeshRendererを表示or非表示にする.
    /// </summary>
    /// <param name="renderBool">True:表示.false:非表示</param>
    public void ArtRendererCtrl(bool renderBool)
    {
        if (Art == null) return;
        var renderers = Art.GetComponentsInChildren<MeshRenderer>();
        if (renderers != null)
        {
            Debug.Log("MeshRendererの表示非表示切替!");
            foreach(MeshRenderer mr in renderers)
            {
                mr.enabled = renderBool;
            }
        }

        if (Terrain != null) Terrain.enabled = renderBool;
    }
}

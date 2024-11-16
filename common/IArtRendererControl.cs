using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Artのレンダリングを消す.
/// </summary>
public interface IArtRendererControl
{

    GameObject Art { set; }
    Terrain Terrain { set; }
    void ArtRendererCtrl(bool renderBool);
}

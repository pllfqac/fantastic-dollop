using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

/// <summary>
/// Userがする明示的Logout.
/// </summary>
public interface IQuitGame {

    /// <summary>
    /// 終了前に行いたい処理.
    /// </summary>
    event UserSaveDelegate UserSaveEvent;
    /// <summary>
    /// ゲーム終了時の処理とゲームの終了.
    /// </summary>
    Task QuitApplication();     
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prefabs-Objects-FieldEventArea.
/// このFEAオブジェクトに持たせたいシナリオターゲットを定義する.
/// </summary>
public class FieldEventAreaProperty : MonoBehaviour
{

    /// <summary>
    /// このFieldEventAreaのもつScenarioTarget.
    /// ExcelのScenarioTargetと合わせること.
    /// 先頭の※は不要.
    /// </summary>
    [SerializeField,Tooltip("必ずいる.先頭の※は不要")]
    private string _scenarioTarget;
    public string ScenarioTarget
    {
        get { return _scenarioTarget; }
        set { _scenarioTarget = value; }
    }

    /// <summary>
    /// シナリオシーン遷移した場合Enemy全撃破のRaise後に開始されるシナリオ.
    /// 無ければnull.
    /// </summary>
    [SerializeField, Tooltip("無ければnullでおｋ.先頭の※は不要")]
    private string _scenarioTargetAfter;
    public string ScenarioTargetAfter
    {
        get { return _scenarioTargetAfter; }
        set { _scenarioTargetAfter = value; }
    }


    /// <summary>
    /// シナリオ発生開始直後の(同Scene上の)遷移先.
    /// 移動がなければNullでおｋ.
    /// </summary>
    [SerializeField,Tooltip("シナリオ発生時の同Scene上の遷移先.移動がなければNullでおｋ.")]
    private Transform _transitionDestination;
    public Transform TransitionDestination
    {
        get { return _transitionDestination; }
        set { _transitionDestination = value; }
    }

    /// <summary>
    /// シナリオ終了時の(同Scene上の)遷移先.
    /// 移動がなければNullでおｋ.
    /// </summary>
    [SerializeField, Tooltip("シナリオ終了時の同Scene上の遷移先.移動がなければNullでおｋ.")]
    private Transform _transitionDestinationAfter;
    public Transform TransitionDestinationAfter
    {
        get { return _transitionDestinationAfter; }
        set { _transitionDestinationAfter = value; }
    }


    /// <summary>
    /// このFEAにコリジョンした場合のシナリオイベントでScene遷移する場合のみ定義する.
    /// 移動がなければNullでおｋ.
    /// このFEAをワープポイントとみなしSceneDefinitionTableの定義したTagNameと合わせること.
    /// </summary>
    [SerializeField,Tooltip("Scene遷移が無ければnullでおｋ.ワープポイントとみなすためSceneDefinitionTableで定義したTagNameを入力")]
    private string _transitionSceneTagName;
    public string TransitionSceneTagName
    {
        get { return _transitionSceneTagName; }
        set { _transitionSceneTagName = value; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Threading;

/// <summary>
/// Single2.
/// FieldEventArea(FEA)のアクティブ化・非アクティブ化の制御
/// </summary>
public class FieldEventManager : MonoBehaviour
{

    /// <summary>
    /// 今いるSceneの全FEA.
    /// Scene切り替え時に取得する.
    /// </summary>
    private List<GameObject> fieldEventAreaInScene;
    private IChapterProgress cp;
    private ITransitionDestinationPositionConfig transitionDestination;

    /// <summary>
    /// シナリオターゲットの一時置き.
    /// 位置も利用するのでTransform
    /// </summary>
    public FieldEventAreaProperty tempFEAProp { get; private set; }
    /// <summary>
    /// シナリオシーンから元のScene遷移後の位置用.
    /// TransitionDestinationPositionConfigはTagやPlayerInstantiatePositionよりもこちらを優先する.
    /// </summary>
    public Vector3 tempV3 { get; set; }
    private void Start()
    {
        cp = GetComponent<IChapterProgress>();
        FindFEA();
        transitionDestination = GetComponent<ITransitionDestinationPositionConfig>();
        Debug.Log("<size=32>FEM Start()!</size>");
    }


    /// <summary>
    /// Sceneが切り替わった時に呼ばれる.
    /// </summary>
    public void FindFEA()
    {
        Debug.Log("FindFEA()");
        fieldEventAreaInScene = GameObject.FindGameObjectsWithTag("FieldEventArea").ToList();   //非アクティブだととれない
        if (fieldEventAreaInScene == null || fieldEventAreaInScene.Count == 0) return;
        foreach (var obj in fieldEventAreaInScene)
        {
            obj.SetActive(false);
            Debug.Log("FieldEventArea 非Active");
        }
        ActivationFieldEventArea();
    }

    /// <summary>
    /// Sceneが切り替わった時やシナリオが進んだ時の"対応する"FEAのアクティブ化.
    /// </summary>
    public void ActivationFieldEventArea()
    {
        if (fieldEventAreaInScene == null || fieldEventAreaInScene.Count == 0) return;
        //取得していたFEAから対応するものを探す
        Debug.Log("ActivationFieldEventArea " + fieldEventAreaInScene.Count);
        foreach (GameObject obj in fieldEventAreaInScene)
        {
            FieldEventAreaProperty feap = obj.GetComponent<FieldEventAreaProperty>();
            cp.FormatingScenarioTarget(feap.ScenarioTarget, out int eventNumber, out string cType);
            if (eventNumber == 0) continue;
            if (cp.IsEndedCheck(cType, eventNumber))
            {
                obj.SetActive(true);
                return;
            }
        }
    }

    /// <summary>
    /// FEAとコリジョンした時.
    /// Player-DetectedArea×FEAのDetectionのDetectionレイヤ同士が反応する.
    /// </summary>
    /// <param name="fea">コリジョンしたFEA.</param>
    /// <param name="collisionPlayer">コリジョンしたPlayer.</param>
    public void OTE(FieldEventAreaProperty fea, Transform collisionPlayer)
    {
        Debug.Log("OTE FEA  scenarioTarget:  " + fea.ScenarioTarget);
        fea.gameObject.SetActive(false);       //FEAは非Active.

        //位置変更がある場合
        /*    if (fea.TransitionDestination != null)
            {
                collisionPlayer.position = fea.TransitionDestination.position;
                collisionPlayer.rotation = fea.TransitionDestination.rotation;
            }*/
        //シナリオイベントによるScene遷移がある場合.
        if (!string.IsNullOrEmpty(fea.TransitionSceneTagName))
        {
            Debug.Log("LoadScene()");
            ISceneController sc = GetComponent<ISceneController>();
            tempV3 = fea.transform.position;                 //元居たシーンに戻る時の位置用.
            tempFEAProp = fea;
            sc.OnChangeScene(fea.TransitionSceneTagName, StartScenariobyChangedScene);  //Scene切替後にシナリオを開始させるように登録する
        }
        else
        {
            Debug.Log("Scene遷移なければそのままシナリオ開始");
            cp.StartScenario(fea.ScenarioTarget, true, null, fea.TransitionDestination, fea.TransitionDestinationAfter);   //Scene遷移なければそのままシナリオ開始.
        }
    }

    /// <summary>
    /// Scene遷移後にシナリオ開始.
    /// </summary>
    private void StartScenariobyChangedScene()
    {
        Debug.Log("StartScenariobyChangedScene()");
        transitionDestination.SceneChangedAfterPositionSetFlug = true;  //Scene遷移後に位置変更するflagを立てておく.
        cp.StartScenario(tempFEAProp.ScenarioTarget, false, null);      //Scene遷移語にシナリオ開始.Scene遷移がある場合はSaveしない(途中で切断したら進めなくなるので)
    }
}

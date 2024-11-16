using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// SkillLevelUp等のTextの表示.
/// 自キャラOnly.
/// MainScene-Canvas.
/// or
/// ManorScene-Canvas.
/// </summary>
public class NotificationTextCtrl : MonoBehaviour
{
    //levelUpTextとしているけど,Food収穫時Animationにも使用.
    [SerializeField]
    private TextMeshProUGUI text1 = null;
    [SerializeField]
    private TextMeshProUGUI text2 = null;
    [SerializeField]
    private TextMeshProUGUI harvestText = null;

    public AnimationCurve curve;        //アニメーションカーブ
    public AnimationCurve colorCurve;   //Text透明度用カーブ



    public void ShowLevelUpText()
    {
        TextMeshProUGUI insObj = Instantiate(text1, this.transform);
        insObj.transform.SetParent(this.gameObject.transform);
        StartCoroutine(Enumerator(insObj));
        StartCoroutine(TextAnimation(insObj));
        StartCoroutine(TextAlphaAnimation(insObj));
    }

    public void ShowSkillLevelUpText()
    {
        TextMeshProUGUI tempObj = Instantiate(text2, this.transform);
        tempObj.transform.SetParent(this.gameObject.transform);
        StartCoroutine(Enumerator(tempObj));
        StartCoroutine(TextAnimation(tempObj));
        StartCoroutine(TextAlphaAnimation(tempObj));
    }

    public void HarvestAddFoodNotice()
    {
        TextMeshProUGUI tempObj = Instantiate(harvestText, this.transform);
        tempObj.transform.SetParent(this.gameObject.transform);
        StartCoroutine(Enumerator(tempObj));
        StartCoroutine(TextAnimation(tempObj));
        StartCoroutine(TextAlphaAnimation(tempObj));
    }


    private IEnumerator Enumerator(TextMeshProUGUI obj)
    {
        yield return new WaitForSeconds(StaticMyClass.TextShowTime);
        Destroy(obj.gameObject);
        Debug.Log("LevelUpText End");
    }

    private IEnumerator TextAnimation(TextMeshProUGUI textMesh)
    {
        float startTime = Time.time;
        while (Time.time - startTime < StaticMyClass.duration)
        {
            //Evaluate()	評価したいカーブ内の時間のカーブの値を返す
            float curveValue = curve.Evaluate((Time.time - startTime) / StaticMyClass.duration);
         //   Debug.Log(curveValue);
            // アニメーションカーブから取得した値を使って、イメージの位置を変更する
            textMesh.transform.position = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y * curveValue+StaticMyClass.positionCoefficient);
            yield return null;
        }
    }

    /// <summary>
    /// 透明度のアニメーションCurve
    /// </summary>
    /// <param name="textMesh"></param>
    /// <returns></returns>
    private IEnumerator TextAlphaAnimation(TextMeshProUGUI textMesh)
    {
        float startTime = Time.time;
        while (Time.time - startTime < StaticMyClass.duration)
        {
            //Evaluate()	評価したいカーブ内の時間のカーブの値を返す
            float curveValue = colorCurve.Evaluate((Time.time - startTime));
        //    Debug.Log(curveValue);
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, curveValue);

            yield return null;
        }
    }


    //==========Food=============
    /// <summary>
    /// 収穫時のFood増加アニメーション.
    /// 各農地の中心から画面右上のFoodTextへ加算量Textが動く.
    /// </summary>
    /// <param name="addFoodValue"></param>
    public void StartFoodAnimation(int addFoodValue)
    {

    }


}

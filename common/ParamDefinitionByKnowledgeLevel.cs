using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/// <summary>
/// KnowledgeLevelによる各ボーナス値(eStatusに加算等)の算出のもとになる値の定義.
/// </summary>
[CreateAssetMenu(menuName = "MyGame/Create paramByKnowledgeLevel", fileName = "ParamByKnowledgeLevel")]
public class ParamDefinitionByKnowledgeLevel : ScriptableObject
{
    [SerializeField]
    private List<ParameterDefinitionByKnowledgeLevel> parameters = new List<ParameterDefinitionByKnowledgeLevel>();

    /// <summary>
    /// 指定したKnowledgeLevelに定義したパラメータ(equipへの加算値など)を定義したClassを返す.
    /// 存在するKnowlegeLevel定義が無ければ最大最小0のみのClassを返す.
    /// </summary>
    /// <param name="targetLevel"></param>
    /// <returns></returns>
    public ParameterDefinitionByKnowledgeLevel GetParameterByLevel(int targetLevel)
    {
        ParameterDefinitionByKnowledgeLevel pd = parameters.FirstOrDefault(p => p.knowledgeLevel == targetLevel);
        if (pd == null) pd = new ParameterDefinitionByKnowledgeLevel() { knowledgeLevel = 0 };

        return pd;
    }
}


/// <summary>
/// KnowledgeLevel別のパラメータ(equipへの加算値など)の最小最大値を定義.
/// </summary>
[System.Serializable]
public class ParameterDefinitionByKnowledgeLevel
{
    public  int knowledgeLevel;
    [SerializeField]
    private int minValue;
    public int MinValue { get { return minValue; } }
    [SerializeField]
    private int maxValue;
    public int MaxValue { get { return maxValue; } }
}

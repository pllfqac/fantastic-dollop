using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �V�i���I�V�[���̓���PT�̂�(�\����)��Enemy�̃��X�g���擾.
/// </summary>
public interface IGetOneEnemyClassList 
{
    List<OneEnemyClass> GetOneEnemyListforScenarioScene(int sceneIndex, int userId);
}

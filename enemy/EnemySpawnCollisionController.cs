using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public delegate void PosTestDel(Vector3 v3);       //Test.

//このスクを付けるEnemyはCapsuleColliderを実装していること.
[RequireComponent(typeof(CapsuleCollider))]

/// <summary>
/// Enemy.
/// Structure内にSpawnされることを防ぐ.
/// EnemySpawn時のみOnTriggerEnterが発生するようにする.
/// OnTriggerEnter発生時Structure外にSpawn位置をずらす.
/// </summary>
public class EnemySpawnCollisionController : MonoBehaviour
{

    //   public event PosTestDel PosEvent;       //Test.

    /*このスクでの(Collider other)は構造物(Structure:map上の家とか穴の開いたところ==EnemyのSpawn出来ない個所)のColliderをさす.   */

    private bool acceptOnTriggerEnter;      //OTE発生後の処理をする場合True.



    void Start()
    {
        acceptOnTriggerEnter = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Structureとの接触のみ.

        if (!acceptOnTriggerEnter) return;

        Vector3 resetPos = ResetSpawnPosition(other);     //Structureの辺上の値を計算.
        acceptOnTriggerEnter = false;
        this.gameObject.transform.position = resetPos;  //自身の位置を変更.
    }


    /// <summary>
    /// OTE処理を実行するかどうか切り返る.
    /// </summary>
    public void OTEAcceptEnd(bool accept)
    {
        Debug.Log("OTE Accept切替 →" + accept);
        acceptOnTriggerEnter = accept;
    }



    /// <summary>
    /// OnTriggerEnter発生時Structure外にSpawn位置をずらす.
    /// </summary>
    private Vector3 ResetSpawnPosition(Collider other)
    {
        float slope = GetSlopeByStructureCenterToSpawnEnemyPosition(other.transform, this.gameObject.transform);    //Local(Structure内)の傾きであることに注意.
                                                                                                                    //      Debug.Log("傾き:" + slope);
        /*傾きとStructureのTrPosからWorldでの切片を算出.b=y-ax*/
        float intercept = CalcWorldIntercept(other.gameObject.transform.position, slope);
        //      Debug.Log("切片:" + intercept);
        int orthant = GetOrthant(other);
        return GetStructureOnTheSide(other, slope, orthant, intercept);
    }

    //構造物の中心位置とEnemy生成位置から傾きを求める.
    private float GetSlopeByStructureCenterToSpawnEnemyPosition(Transform structureTr, Transform spawnEnemyTr)
    {
        float dx = spawnEnemyTr.position.x - structureTr.position.x;
        float dz = spawnEnemyTr.position.z - structureTr.position.z;
        return dz / dx;
    }

    /// <summary>
    /// Spawn位置の象限取得.
    /// </summary>
    /// <param name="other">構造物のCollider</param>
    /// <returns>1～4st Orthant(象限)</returns>
    private int GetOrthant(Collider other)
    {
        //z-Axis.
        if (other.transform.position.x < this.gameObject.transform.position.x)
        {
            //1st,4st Orthant.
            //x-Axis.
            if (other.transform.position.z < this.gameObject.transform.position.z) return 1;    //1st Orthant.
            else return 4;                                                                      //4st Orthant.
        }
        else
        {
            //2st,3st Orthant.
            if (other.transform.position.z < this.gameObject.transform.position.z) return 2;    //2st Orthant.
            else return 3;                                                                      //3st Orthant.
        }
    }

    //Structure中心と生成位置の2点を結ぶ線(z=rad*x)の延長線と交わるStructure辺上の点(移動先Position)を求める.
    private Vector3 GetStructureOnTheSide(Collider other, float slope, int orthant, float intercept)
    {
        Vector3 resetPos;
        /*象限別で処理.順番に意味はないがまずx-Max or Minを式に代入してx-Max線上にあればその座標とする.なければz軸で*/
        switch (orthant)
        {
            case 1: resetPos = ResetPosition1stOrthant(other, slope, intercept); break;
            case 2: resetPos = ResetPosition2ndOrthant(other, slope, intercept); break;
            case 3: resetPos = ResetPosition3rdOrthant(other, slope, intercept); break;
            case 4: resetPos = ResetPosition4thOrthant(other, slope, intercept); break;
            default: return new Vector3();
        }
        //      Debug.Log("算出したStructure辺上の値" + resetPos.ToString());
        return resetPos;
    }

    private float CalcWorldIntercept(Vector3 structurePos, float slope)
    {
        return structurePos.z - (slope * structurePos.x);
    }

    //===============================象限別に処理===============================

    private Vector3 ResetPosition1stOrthant(Collider other, float slope, float intercept)
    {
        float xMaxOnTheSide = xMax(other);
        float _zMax = zMax(other);
        float z = (slope * xMaxOnTheSide) + intercept;
        //       Debug.Log("xMaxOnTheSide:" + xMaxOnTheSide +" z-Max"+_zMax+ "  z:" + z);

        if (z > _zMax)
        {
            //StructureのzMaxの辺上に位置変更.1stOrthant z軸側.
            float xPos = (_zMax - intercept) / slope;                     //y=ax+bの式変形.
            return new Vector3(xPos, this.gameObject.transform.position.y,
                _zMax + this.gameObject.GetComponent<CapsuleCollider>().radius);          //自身の体が完全に外に出るように自身のcolliderのsize分さらに増やす.
        }
        else
        {
            //StructureのxMaxの辺上に位置変更.1stOrthant x軸側.
            return new Vector3(xMaxOnTheSide + this.gameObject.GetComponent<CapsuleCollider>().radius, this.gameObject.transform.position.y, z);
        }
    }

    private Vector3 ResetPosition2ndOrthant(Collider other, float slope, float intercept)
    {
        float xMinOnTheSide = xMin(other);
        float _zMax = zMax(other);
        float z = (slope * xMinOnTheSide) + intercept;
        //      Debug.Log("xMinOnTheSide:" + xMinOnTheSide + " z-Max" + _zMax + "  z:" + z);

        if (z > _zMax)
        {
            //StructureのzMaxの辺上に位置変更.
            float xPos = (_zMax - intercept) / slope;
            return new Vector3(xPos, this.gameObject.transform.position.y,
                _zMax + this.gameObject.GetComponent<CapsuleCollider>().radius);
        }
        else
        {
            return new Vector3(xMinOnTheSide - this.gameObject.GetComponent<CapsuleCollider>().radius, this.gameObject.transform.position.y, z);
        }
    }

    private Vector3 ResetPosition3rdOrthant(Collider other, float slope, float intercept)
    {
        float xMinOnTheSide = xMin(other);
        float _zMin = zMin(other);
        float z = (slope * xMinOnTheSide) + intercept;
        //      Debug.Log("xMinOnTheSide:" + xMinOnTheSide + " _zMin" + _zMin + "  z:" + z);

        if (z < _zMin)
        {
            //StructureのzMinの辺上に位置変更.
            float xPos = (_zMin - intercept) / slope;
            return new Vector3(xPos, this.gameObject.transform.position.y,
                _zMin - this.gameObject.GetComponent<CapsuleCollider>().radius);
        }
        else
        {
            return new Vector3(xMinOnTheSide - this.gameObject.GetComponent<CapsuleCollider>().radius, this.gameObject.transform.position.y, z);
        }
    }

    private Vector3 ResetPosition4thOrthant(Collider other, float slope, float intercept)
    {
        float xMaxOnTheSide = xMax(other);
        float _zMin = zMin(other);
        float z = (slope * xMaxOnTheSide) + intercept;
        //     Debug.Log("xMaxOnTheSide:" + xMaxOnTheSide + " _zMin" + _zMin + "  z:" + z);

        if (z < _zMin)
        {
            //StructureのzMinの辺上に位置変更.
            float xPos = (_zMin - intercept) / slope;
            return new Vector3(xPos, this.gameObject.transform.position.y,
                _zMin - this.gameObject.GetComponent<CapsuleCollider>().radius);
        }
        else
        {
            return new Vector3(xMaxOnTheSide + this.gameObject.GetComponent<CapsuleCollider>().radius, this.gameObject.transform.position.y, z);
        }
    }

    //=============================================================================================

    //x-maxを求める.
    private float xMax(Collider other)
    {
        return other.transform.position.x + (other.transform.localScale.x / 2);
    }

    //z-Max
    private float zMax(Collider other)
    {
        return other.transform.position.z + (other.transform.localScale.z / 2);
    }

    private float xMin(Collider other)
    {
        return other.transform.position.x - (other.transform.localScale.x / 2);
    }

    private float zMin(Collider other)
    {
        return other.transform.position.z - (other.transform.localScale.z / 2);
    }


}

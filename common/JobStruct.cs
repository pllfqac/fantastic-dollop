using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System;


/// <summary>
/// Job定義.
/// </summary>
public struct JobStruct:IJob
{
	//=========ジョブ生成で渡す========

	public Vector3 destination;                //目的地の座標.
	public Vector3 transformPosition;
	private Vector3 velocity;    //現在の移動速度.

	public bool arrived ;            //到着したらTrue.
	public float deltaTime;

	//出力的な CharacterController.Move()の中身を計算して渡す.
	public NativeArray<Vector3> result; 

	/// <summary>
	/// 移動速度.
	/// Enemyの個性を出すためにEnemyによって違う値を設定したい
	/// </summary>
	public float walkSpeed;
//	public bool UpdateStop;                 //EnemyがやられたときTrueにしてUpdate処理しない.
//	private int _addSpeed;       //AddSpeedValueプロパティによって管理されるフィールド.								 
	public int AddSpeedByUcc { get; set; }  //UCCによるTotal速度加算値.Set時に最大最小確認.

	public void Execute()
	{
		//水平方向
		Vector3 destinationXZ = destination;            //destination:行先.目的地の座標.
		destinationXZ.y = transformPosition.y;		//ジョブ生成で渡す.
		//normalized:正規化されたときベクトルは同じ方向は維持したままで長さが 1.0 のものが作成されます.ベクトルが小さすぎる場合は、正規化した場合0 が設定されます
		Vector3 direction = (destinationXZ - transformPosition).normalized;        //direction:方向            
		float sqrDistance = (destinationXZ - transformPosition).sqrMagnitude;  //ベクトルの 2 乗の長さを返します（読み取り専用
		Vector3 currentVelocity = velocity;     //一時沖.

		arrived = sqrDistance < 1.44f? true : false;        // StaticMyClass.stoppingDistance 

		if (arrived) velocity = Vector3.zero;
		else velocity = direction * (walkSpeed + AddSpeedByUcc);      //walkSpeedが5程度だとPCのみEnemyが(ほぼ)動かなかった.

		velocity = Vector3.Lerp(currentVelocity, velocity, Mathf.Min(deltaTime * 5.0f, 1.0f)); //PCのTime.deltaTimeはたぶん0.01くらい
		velocity.y = 0;


		result[0] = velocity;
	}
}


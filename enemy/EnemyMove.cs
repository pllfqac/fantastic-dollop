using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

#pragma warning disable 649


/// <summary>
/// debug用
/// </summary>
/// <param name="v3"></param>
public delegate void enemyMoveHandler(Vector3 v3);

//Enemy.
public class EnemyMove : MonoBehaviour, IChangeMoveSpeed
{
	public event enemyMoveHandler MoveEvent;

	private EnemyEasyAI easyAI;
	private Vector3 destination;                //目的地の座標.

	//  private Vector3 velocity = Vector3.zero;    //現在の移動速度.
	private Vector2 velocity2 = Vector2.zero;


	private CharacterController charaCtlr;
	private IEnemyMyAnimation eAnimation;

	[SerializeField]
	private bool arrived;            //到着したらTrue.

	/// <summary>
	/// 最大の移動速度.
	/// つまりChasing時の移動速度.
	/// Enemyの個性を出すためにEnemyによって違う値を設定する.
	/// 30±10くらいで.
	/// 
	/// </summary>
	[SerializeField, Tooltip("Enemyの個性を出すためにEnemyによって違う値を設定する.30±10くらいで"), Range(20, 40)]
	private float walkSpeed;

	[NonSerialized]
	public bool UpdateStop;                 //EnemyがやられたときTrueにしてUpdate処理しない.
	private int _addSpeed;       //AddSpeedValueプロパティによって管理されるフィールド.
								 //UCCによるTotal速度加算値.Set時に最大最小確認.
	public int AddSpeedByUcc
	{
		get
		{
			return _addSpeed;
		}

		set
		{
			_addSpeed = value;
		}
	}

	//Enemyはこの値を使用しない.
	public float AddSpeedByEquipmentWeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	//charaCtlr.velocity.sqrMagnitudeと比較する値.この値以上ならarrived=Trueとなる.
	//charaCtlr.velocity.sqrMagnitudeは,walkSpeedが5程度の場合PCのような高フレームレートだとほぼ0(3.5e-09とか)になりenmeyは滅多にMoveしなくなってしまう.
	//Editorの場合と端末の場合で数値を変える(携帯端末は0.1fで希望した動き)
	//	private double arrivedValue = 0.1f;



	void Start()
	{
		arrived = true;
		UpdateStop = false;
		charaCtlr = GetComponent<CharacterController>();
		destination = transform.position;
		eAnimation = GetComponent<IEnemyMyAnimation>();
		easyAI = GetComponent<EnemyEasyAI>();

	}

	private void Update()
	{
		if (UpdateStop) return;

		//水平方向
		Vector3 destinationXZ = destination;            //destination:行先.目的地の座標.
		destinationXZ.y = transform.position.y;
		//normalized:正規化されたときベクトルは同じ方向は維持したままで長さが 1.0 のものが作成されます.ベクトルが小さすぎる場合は、正規化した場合0 が設定されます
		Vector3 direction = (destinationXZ - transform.position).normalized;        //direction:方向            
																					//	Vector3 currentVelocity = velocity;     //一時沖.

		arrived = (destinationXZ - transform.position).sqrMagnitude < StaticMyClass.stoppingDistance ? true : false;
		//	Debug.Log("arrived" + arrived+ "  "+ (destinationXZ - transform.position).sqrMagnitude );

		if (arrived)
		{
			//	Debug.Log("arrived");
			velocity2 = Vector2.zero;
		}
		else
		{
			float speed = walkSpeed;
			if (easyAI.state == EnemyEasyAI.State.chasing) speed *= StaticMyClass.enemyRunSpeedCoef;        //Runはwalkの1.5倍のSpeed.
																											//	velocity = direction * ((speed + (float)AddSpeedByUcc)) / 10.0f;      //walkSpeedが5程度だとPCのみEnemyが(ほぼ)動かなかった.
			velocity2 = new Vector2((direction.x * speed + (float)AddSpeedByUcc) / 10.0f, (direction.z * speed + (float)AddSpeedByUcc) / 10.0f);
			//Debug.Log("velocity2 => " + velocity2);
		}
		//	velocity = Vector3.Lerp(currentVelocity, velocity, Mathf.Min(Time.deltaTime * 5.0f, 1.0f)); //PCのTime.deltaTimeはたぶん0.01くらい
		//	velocity.y = 0;


		if (velocity2.sqrMagnitude > 0.01f && !arrived)
		{
			//指定した方向に一気に回転する				
			Vector3 relativePos = destinationXZ - transform.position;
			relativePos.y = 0; //上下方向の回転はしないように制御
			transform.rotation = Quaternion.LookRotation(relativePos);

			//	Quaternion charaTargetRotation = Quaternion.LookRotation(direction);
			//	transform.rotation = Quaternion.RotateTowards(transform.rotation, charaTargetRotation, StaticMyClass.rotationSpeed * Time.deltaTime);
		}


		//じゅうりょく
		/*	velocity += Vector3.down * StaticMyClass.gravityPower * Time.deltaTime;							//じゅうりょくが効かず？charaCtlr.isGroundedがfalseになる問題
			Vector3 snapGround = Vector3.zero;
			if (!charaCtlr.isGrounded) snapGround = Vector3.down;
		*/


		Vector3 v3 = new Vector3(velocity2.x, 0, velocity2.y);
		//	charaCtlr.Move((velocity * Time.deltaTime)+ snapGround);   //CharacterControllerを使って動かす.
		charaCtlr.Move((v3 * Time.deltaTime));   //CharacterControllerを使って動かす.

		//charaCtlr.velocity.sqrMagnitudeは,walkSpeedが5程度の場合PCのような高フレームレートだとほぼ0(3.5e-09とか)になりenmeyは滅多にMoveしないので注意(携帯端末は希望した動き)
		//	Debug.Log("c v s :" + charaCtlr.velocity.sqrMagnitude);
		if (charaCtlr.velocity.sqrMagnitude < StaticMyClass.arrivedValue)
		{
			//	Debug.Log("arrived!!!");
			arrived = true;
			//velocity = Vector3.zero;
			velocity2 = Vector2.zero;
		}

		//Debug.Log("c v s :" + charaCtlr.velocity.sqrMagnitude);         //walkSpeed40で15くらい
		eAnimation.MoveAnimation(charaCtlr.velocity.sqrMagnitude);
	}




	public void SetDestination(Vector3 destination)
	{
		//Debug.Log("Set Destination  "+transform.position+ " → "+destination);
		arrived = false;
		this.destination = destination;
		if (MoveEvent != null) MoveEvent(destination);
	}


	/// <summary>
	/// AttackTargetがいるときは LookAt()で瞬時AttackTarget方向へ回転.
	/// </summary>
	/// <param name="direction"></param>
	public void SetDirection(Vector3 direction)
	{
		this.transform.LookAt(direction);
	}

	public void StopMove()
	{
		destination = transform.position;
		eAnimation.MoveAnimation(0);
	}

	public bool Arrived()
	{
		return arrived;
	}


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

/// <summary>
/// Enemy.Master用.経験値計算に使う.AttackerのID,Damageの履歴を保存.HP0で履歴リセット.
/// </summary>
public class EnemyHistory : MonoBehaviour, IEnemyHistory
{


	[NonSerialized]
	public IMasterAllMemberTable allMemberTable;    //ins
	[NonSerialized]
	public IPlayerTable pTabel;   //ins

	/// <summary>
	/// このEnemyにDamageを与えたPlayerのIDとdamageの割合を保存するTable.
	/// TKey:PlayerID
	/// </summary>
	private Dictionary<int, EnemyDamageInfoClass> DamageRatioTable = new Dictionary<int, EnemyDamageInfoClass>();

	/// <summary>
	/// ExpをPlayer毎に割り振るときの送信先を保存したTable.
	/// TKey:このEnmeyにDamageを与えたPlayerのID
	/// </summary>
	private Dictionary<int, IMasterPlayerExpLog> expLogTable = new Dictionary<int, IMasterPlayerExpLog>();

	private EnemyHP eHp;
	private EnemyStatus eSt;


	public class EnemyDamageInfoClass
	{
		/// <summary>
		/// ダメージ割合(%.加算)
		/// </summary>
		public int damageRatio;
		/// <summary>
		/// AttackerLevel
		/// </summary>
		public int attackerLevel;
		/// <summary>
		/// ダメージを受けた時間.
		/// 一定時間経過でExp取得権消滅.
		/// </summary>
		public DateTime damageDatetime;

		//どの武器でどれだけのダメージ割合を受けたか.TValue:damageRatioの合計.
		public Dictionary<StaticMyClass.WeaponEquipmentType, int> damageRatioByWeaponTypeTable = new Dictionary<StaticMyClass.WeaponEquipmentType, int>();
		//同じ割合になった場合は?
	}

	private void Start()
	{
		if (!PhotonNetwork.IsMasterClient) Destroy(this);
		eSt = GetComponent<EnemyData>().Status;
		eHp = GetComponent<EnemyHP>();

	}

	//攻撃を受けた時.
	public void SaveDamageData(int damage, AttackInfoClass aic, IMasterPlayerExpLog explog)
	{
		int attackerID = aic.Attackerid;
		int ratioDamage = CalcRatioDamage(damage);
		aic.AttackerLevel = int.Parse(/*allMemberTable.GetPlayerObj(aic.Attackerid)*/pTabel.FindUserObject(aic.Attackerid).obj.GetComponent<CharaStatus>().Level);

		if (DamageRatioTable.ContainsKey(attackerID))
		{
			EnemyDamageInfoClass edi = DamageRatioTable[attackerID];
			edi.damageRatio += ratioDamage;                         //既にある場合、加算.
			edi.damageDatetime = DateTime.Now;                      //更新.
			UpdateDamageRatioByWeaponTypeTable(edi, aic, ratioDamage);
		}
		else
		{
			expLogTable[attackerID] = explog;
			EnemyDamageInfoClass enemyDamageInfoClass = new EnemyDamageInfoClass()
			{
				damageRatio = ratioDamage,
				attackerLevel = aic.AttackerLevel,
				damageDatetime = DateTime.Now,
			};
			DamageRatioTable[attackerID] = enemyDamageInfoClass;
			UpdateDamageRatioByWeaponTypeTable(DamageRatioTable[attackerID], aic, ratioDamage);
		}
		Debug.Log("ID :" + attackerID + "  Ratio:" + ratioDamage + "   AttackerLevel:" + aic.AttackerLevel + "  Total Ratio : " + DamageRatioTable[attackerID].damageRatio);
	}



	//Damageの割合を計算.0～100(%).
	//算出したDamageで残りHP計算して0になった場合,そのDamageの割合は残りHP分のみとする.(割合の合計が100%超えてしまうため)
	private int CalcRatioDamage(int damage)
	{
		int damageRatio;
		if ((eHp.HP - damage) < 0)
		{
			damageRatio = (int)(((float)eHp.HP / eSt.maxhp) * 100);

		}
		else
		{
			damageRatio = (int)(((float)damage / eSt.maxhp) * 100);
			//HPが多くDamageが小さい場合はRatioが0になってしまうので1とする.
			if (damageRatio == 0) damageRatio = 1;
		}
		Debug.Log("Damageの割合を計算.0～100(%) :" + damageRatio);
		return damageRatio;
	}


	//Dictionaryを渡す.
	public Dictionary<int, EnemyDamageInfoClass> GetDamageRatioTable()
	{
		return DamageRatioTable;
	}
	//Dictionaryを渡す.
	public Dictionary<int, IMasterPlayerExpLog> GetExpLogTable()
	{
		return expLogTable;
	}

	//EnemyのHPが0になったとき,Historyのデータ消去.
	public void LifeZero()
	{
		DamageRatioTable.Clear();
		expLogTable.Clear();            //未テス
	}


	/// <summary>
	/// このEnmeyにダメージを与えたPlayerの使用していた武器とその与ダメ割合
	/// </summary>
	/// <param name="edi">ここにデータいれる</param>
	/// <param name="skillAbs"></param>
	/// <param name="damageRatio"></param>
	private void UpdateDamageRatioByWeaponTypeTable(EnemyDamageInfoClass edi, AttackInfoClass aic, int damageRatio)
	{
		StaticMyClass.WeaponEquipmentType wType = LoadWeaponType(aic.Attackerid);//allSkillRef.LoadWeaponType(aic.absoluteSkillNum);
		if (edi.damageRatioByWeaponTypeTable.ContainsKey(wType)) edi.damageRatioByWeaponTypeTable[wType] += damageRatio;    //既にあれば加算
		else edi.damageRatioByWeaponTypeTable[wType] = damageRatio;                                                         //なければ追加
	}


	/// <summary>
	/// このEnmeyにダメージを与えたPlayerの使用していた武器Typeを取得する.
	/// </summary>
	/// <param name="attackerPlayerId"></param>
	/// <returns>もし取得できなければnone.NormalAttackなら装備している武器に対応したType.未装備ならUnarmed.</returns>
	private StaticMyClass.WeaponEquipmentType LoadWeaponType(int attackerPlayerId)
	{
		return allMemberTable.GetUserEquippingWeaponType(attackerPlayerId).GetEquippingWeaponType();
	}

}
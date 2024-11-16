using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Secure;


/// <summary>
/// PlayerキャラのStatus.
/// User-> newしてMyPlayerDataRefに置く.
/// UniMas->newしてmamt.
/// </summary>
public class CharaStatus2
{
    public int Level => level.Value;
    public int Exp { get { return exp.Value; } set { exp.Value = value; } }
    public int StatusPoint { get { return statusPoint.Value; } set { statusPoint.Value = value; } }

    // ちから、わざ、ぼうぎょ、まほう攻撃、まほう防御、かいひ
    public int MaxHP => maxHP.Value;
    public int MaxMP => maxMP.Value;
    public int HP { get { return hp.Value; } set { hp.Value = value; } }
    public int MP { get { return mp.Value; } set { mp.Value = value; } }
    public int Pwr => power.Value;
    public int Dex => dexerity.Value;
    public int Def => defense.Value;
    public int Mat => magicAttack.Value;
    public int Mde => magicDefence.Value;
    public int Agi => agility.Value;

    private ISecureValue<int> level;
    private ISecureValue<int> exp;
    private ISecureValue<int> statusPoint;

    private ISecureValue<int> maxHP;
    private ISecureValue<int> maxMP;
    private ISecureValue<int> hp;
    private ISecureValue<int> mp;
    private ISecureValue<int> power;
    private ISecureValue<int> dexerity;
    private ISecureValue<int> defense;
    private ISecureValue<int> magicAttack;
    private ISecureValue<int> magicDefence;
    private ISecureValue<int> agility;

    /// <summary>
    /// 自キャラ用コンストラクタ.
    /// </summary>
    public CharaStatus2(int lv,int exp,int statusPoint,int maxHP,int maxMP,int hp,int mp,int pwr,int dex,int def,int mat,int mde,int agi)
    {
        this.level = new SecureInt(lv);
        this.exp = new SecureInt(exp);
        this.statusPoint = new SecureInt(statusPoint);

        this.maxHP = new SecureInt(maxHP);
        this.maxMP = new SecureInt(maxMP);
        this.hp = new SecureInt(hp);
        this.mp = new SecureInt(mp);
        this.power = new SecureInt(pwr);
        this.dexerity = new SecureInt(dex);
        this.defense = new SecureInt(def);
        this.magicAttack = new SecureInt(mat);
        this.magicDefence = new SecureInt(mde);
        this.agility = new SecureInt(agi);
    }

    /// <summary>
    /// UniMas用コンストラクタ.
    /// </summary>
    /// <param name="exp"></param>
    public CharaStatus2(int exp)
    {
        this.exp = new SecureInt(exp);
    }

    /// <summary>
    /// LvUPによるStatusの更新.
    /// 全て引数に指定した値に上書きされる.
    /// </summary>
    public void UpdateStatusByLvUp(int lv,  int statusPoint, int maxHP, int maxMP/*, int hp, int mp, int pwr, int dex, int def, int mat, int mde, int agi*/)
    {
        level.Value = lv;
        this.statusPoint.Value = statusPoint;
        this.maxHP.Value = maxHP;
        this.maxMP.Value = maxMP;
     /*   this.hp.Value = hp;
        this.mp.Value = mp;
        power.Value = pwr;
        dexerity.Value = dex;
        defense.Value = def;
        magicAttack.Value = mat;
        magicDefence.Value = mde;
        agility.Value = agi;*/

    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

/// <summary>
/// Damage表示の際のDamageUIAnimationスク制御.
/// 自キャラ=>Canvas-MyCharaDamageUICreatePosition.
/// 他キャラ=>Canvas-OtherUserDamageUIGenerator-DamageUIGenerator.
/// Enmey	=>EnmeyNode-DamageUIGenerator.
/// </summary>
public class DamageUIAnimationCtrl : MonoBehaviour,IComboUI
{
	//Damageとつけたがコンボ成功での表示も行う
	//自キャラのSkillと他キャラを区別するためにサイズに差をつけている.
	[SerializeField]
	private DamageUIAnimation ComboAnimation = null;			//自キャラのコンボ成功で自キャラの頭上に表示.
	[SerializeField]
	private DamageUIAnimation CriticalImageAnimation = null;    //Criticalスプライト.

	[SerializeField]
	private DamageUIAnimation[] damageUIs = null;

	private IEnumerator damageUIAnimationCoroutine;

	[System.NonSerialized]
	public IParticleAction particleAction;  //Ins.CallBack.


	/// <summary>
	/// Damage表示したいとき呼ぶ.
	/// particleも呼ばれる.
	/// </summary>
	/// <param name="isMine">自キャラが与えたDamage,ReviのときTrue.サイズに差をつけるため.</param>
	public void StartDamageUIAnimation(int? showValue,bool isCritical,bool isMine)
	{
		Debug.Log("<color=red>値 表示!</color>  "+showValue+ "   DamageUIAnimationCtrl ID:"+this.gameObject.GetInstanceID());
		foreach (var p in damageUIs)
		{
			if (p.used) continue;
			else
			{
				if (showValue == null) p.StartDamageTextAnimation("miss");
				else
				{
					p.StartDamageTextAnimation((int)showValue,isMine);
					particleAction.ReceiveParticlePlay(showValue, isCritical);
					if (isCritical) CriticalImageAnimation.StartCriticalImage();
				}
				break;
			}
		}
	}


	/// <summary>
	/// Damage表示したいとき呼ぶ.
	/// particleも呼ばれる.
	/// </summary>
	public void StartDamageUIAnimation(OneDamageInfoClass odc,bool isMine)
	{
		StartDamageUIAnimation(odc.OneDamage, odc.IsCritical,isMine);
	}

	/// <summary>
	/// こちらで適当な一定間隔で表示するVer.
	/// particleも呼ばれる.
	/// </summary>
	/// <param name="odiLis"></param>
	public void StartDamageUIAnimation(IReadOnlyList<OneDamageInfoClass> odiLis,bool isMine)
	{
		StartCoroutine(enumerator(odiLis,isMine));
	}

	private IEnumerator enumerator(IReadOnlyList<OneDamageInfoClass> odiLis,bool isMine)
	{
		foreach (OneDamageInfoClass odi in odiLis)
		{
			StartDamageUIAnimation(odi,isMine);
			yield return new WaitForSeconds(0.2f);
		}
	}

	
	//EnemyNodeのみ.FadeOutの最後に呼ばれるダメージ表示中に非アクティブになったとき?表示がそのまま残ってしまうので対処
	public IEnumerator WaitDamageUIAnimation()
	{
		while(damageUIAnimationCoroutine!=null)
		{
			yield return null;
		}
		this.gameObject.SetActive(true);
	}

	//Test
	public void OnTest()
	{
		CriticalImageAnimation.StartCriticalImage();
		/*foreach (var p in damageUIs)
		{
			if (p.used) continue;
			else
			{
				p.StartDamageTextAnimation(Random.Range(0,101),false);
				break;
			}
		}*/
	}

	/// <summary>
	/// 自キャラのコンボ成功で呼ばれる.
	/// </summary>
    public void ShowCombo()
    {
		ComboAnimation.StartCriticalImage();
    }
}

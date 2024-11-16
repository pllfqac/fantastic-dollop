using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Player.
/// エリアカリング,シナリオでの他Playerの表示非表示の処理.
/// or
/// 自キャラのSKillでも使用.
/// </summary>
public class PlayerFade : MonoBehaviour,IRendererController
{
	/*「Custom/ShadowReceivableTransparent」以外はFade対象外とする.すぐにOut,遅れてInする仕様.*/


	private IEnumerator coroutine;

	/// <summary>
	/// InspectorでObjectのRendererを指定.
	/// </summary>
	[SerializeField]
	private List<Renderer> rens;

	/// <summary>
	/// Equipの表示・非表示.
	/// </summary>
	private IEquipmentObjectManager equipment;


    private void Start()
    {
		equipment = GetComponent<IEquipmentObjectManager>();
    }

    /// <summary>
    /// 次第にはっきりと.
    /// </summary>
    public void FadeIn()
    {
        Debug.Log("FadeIn()");
        if (coroutine != null) StopCoroutine(coroutine);
        if (!this.gameObject.activeSelf) this.gameObject.SetActive(true);
        coroutine = FadeInCoroutine2();
        StartCoroutine(coroutine);
    }

	/// <summary>
	/// 次第にはっきりと.
	/// </summary>
	private IEnumerator FadeInCoroutine2()
	{
		var rLis = equipment?.GetEquippingObjRenderer();

		var totalRen = new List<Renderer>(rens);
		if (rLis != null) totalRen.AddRange(rLis);

		List<Renderer> nonFadeRenderers = new List<Renderer>(); //Fade非対応のRendererのリスト.
		foreach (Renderer r in totalRen)
		{
			if (r.material.shader.name != StaticMyClass.FadeShaderName) nonFadeRenderers.Add(r);
		}

		Debug.Log("除去前 Total Ren:" + totalRen.Count);
		totalRen.RemoveAll(p => nonFadeRenderers.Contains(p));  //Fade非対応のRendererを取り除く
		Debug.Log("Fade非対応のRenderer数:" + nonFadeRenderers.Count + "   除去後 Total Ren:" + totalRen.Count);


		Color[] colors = new Color[totalRen.Count];

		foreach(var(ren,index)  in totalRen.Select((ren,index)=>(ren,index)))
        {
			//Debug.Log("index:" + index);
			if (ren == null) break;
			colors[index] = ren.material.color;
		}


		//非ActiveならActiveに
		for (float f = 0; f <= 1; f +=StaticMyClass.fadeValue)
		{
            for (int i = 0; i < colors.Length; i++)
            {
				colors[i].a = f;
				if (totalRen[i] == null) break;
				totalRen[i].material.color = colors[i];        //アルファを1にすると非透明になる.
			}
			yield return null;		//コルーチン中は途中でオブジェがなくなってNullRefエラーでる可能性があるっぽい
		}

		foreach (var (ren, index) in totalRen.Select((ren, index) => (ren, index)))
		{
			if (ren == null) break;
			colors[index].a = 1;
			ren.material.color = colors[index];
		}

		//Fade非対応のRendererは後でまとめて表示する
		foreach (Renderer r in nonFadeRenderers)
		{
			r.enabled = true;
		}


		coroutine = null;
	}



	public void FadeOut()
	{
		//if (PhotonNetwork.IsMasterClient) return;
		Debug.Log("FadeOut()");
		if (coroutine != null) StopCoroutine(coroutine);
		if (!this.gameObject.activeSelf ) return;		
		coroutine = FadeOutCoroutine2();
		StartCoroutine(coroutine);
	}


	/// <summary>
	/// 次第に消える.
	/// </summary>
	private IEnumerator FadeOutCoroutine2()
	{
		var rLis = equipment?.GetEquippingObjRenderer();

		var totalRen = new List<Renderer>(rens);
		if(rLis!=null) totalRen.AddRange(rLis);


		List<Renderer> nonFadeRenderers = new List<Renderer>(); //Fade非対応のRendererのリスト.
		foreach (Renderer r in totalRen)
		{
			if (r.material.shader.name != StaticMyClass.FadeShaderName) nonFadeRenderers.Add(r);
		}

		Debug.Log("除去前 Total Ren:" + totalRen.Count);
		totalRen.RemoveAll(p => nonFadeRenderers.Contains(p));  //Fade非対応のRendererを取り除く
		Color[] colors = new Color[totalRen.Count];
		Debug.Log("Fade非対応のRenderer数:"+nonFadeRenderers.Count +	"   除去後 Total Ren:" + totalRen.Count);


		foreach (var (ren, index) in totalRen.Select((ren, index) => (ren, index)))
		{
			Debug.Log("index:" + index);
			colors[index] = ren.material.color;
		}
		//Fade非対応のRendererのみ先に消しておく
		foreach (Renderer r in nonFadeRenderers)
        {
			r.enabled = false;
        }

		for (float f = 1f; f >= 0; f -= StaticMyClass.fadeValue)
		{			
            for (int i = 0; i < colors.Length; i++)
            {
				colors[i].a = f;
				//Debug.Log("α: " + colors[i].a);
				if (totalRen[i] == null) break;
				totalRen[i].material.color = colors[i];        //アルファを0にすれば完全に見えなくなる。
			}
			yield return null;
		}

		foreach (var (ren, index) in totalRen.Select((ren, index) => (ren, index)))
		{
			if (ren == null) break;
			colors[index].a = 0;
			ren.material.color = colors[index];
		}

		if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
		coroutine = null;
	}


	//Class名にFadeと付けたけどすぐに処理するVerも書いた..


	/// <summary>
	/// すぐ消える
	/// </summary>
	public void QuickOut()
	{
		var rLis = equipment?.GetEquippingObjRenderer();

		var totalRen = new List<Renderer>(rens);
		if (rLis != null) totalRen.AddRange(rLis);

		foreach (var p in totalRen)
		{
			//Debug.Log("Quick Out  Ren Obj Name:" + p.transform.name);
			//Fade対象のShderを使っているか否かはShader名で判断
			if (p.material.shader.name==StaticMyClass.FadeShaderName)
            {
				//Fade用Shaderの場合
				Color c = p.material.color;
				c.a = 0;
				p.material.color = c;
            }
            else
            {
				//Fade用でないShaderの場合
				p.enabled = false;
            }
		}
		//影
		transform.Find("Shadow1").gameObject.SetActive(false);
	}


	/// <summary>
	/// すぐ表示する.
	/// </summary>
	public void QuickIn()
	{
		var rLis = equipment?.GetEquippingObjRenderer();

		var totalRen = new List<Renderer>(rens);
		if (rLis != null) totalRen.AddRange(rLis);

		foreach (var p in totalRen)
		{
            if (p.material.shader.name == StaticMyClass.FadeShaderName)
            {
				Color c = p.material.color;
				c.a = 1;
				p.material.color = c;
            }
            else
            {
				p.enabled = true;
            }
		}
		//影
		transform.Find("Shadow1").gameObject.SetActive(true);
	}


}

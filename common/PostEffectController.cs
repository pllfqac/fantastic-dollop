using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PostEffectController : MonoBehaviour
{

    private PostProcessVolume volume;
    private Vignette vignette;
    private Coroutine coroutine;
    /// <summary>
    /// �ύX�����������̂݃G�t�F�N�g��OnOff�����邽�߂̏�Ԃ̕ۑ��p.True�ŃG�t�F�N�g��.
    /// </summary>
    private bool now;

    private void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out vignette);
        vignette.intensity.value = 0;
    }


    /// <summary>
    /// Vignette�G�t�F�N�g�̃I���I�t.
    /// </summary>
    /// <param name="isOn">True��On.False��Off.</param>
    public void ChangeVignetteEffect(bool isOn)
    {
        if (now == isOn) return;        //���łɓ�����ԂȂ牽�����Ȃ�.
        now = isOn;

        if (coroutine != null) StopCoroutine(coroutine);
        Debug.Log("Vignette Effect =>" + isOn);
        if (isOn)
        {           
            coroutine = StartCoroutine(VignetteOn());
        }
        else vignette.intensity.value = 0;
    }

    private IEnumerator VignetteOn()
    {
        for (float i = 0; i < 0.6f;)
        {
            vignette.intensity.value = i;
            i += 0.02f;
            yield return null;
        }
        coroutine = null;
    }
    
}

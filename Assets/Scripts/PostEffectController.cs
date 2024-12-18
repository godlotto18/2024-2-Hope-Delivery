using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostEffectController : MonoBehaviour
{
    public static PostEffectController Instance;
    private Volume volume;
    private Vignette vignette;
    private DepthOfField depthOfField;
    public float Value_Speed = 10f;
    private bool Is_On_coroutine = false;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this; // 현재 인스턴스를 설정
        }
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out depthOfField);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && Is_On_coroutine == false)
        {
            StartCoroutine(Hit_Effect());
        }
    }

    public IEnumerator Hit_Effect()
    {
        vignette.active = true;
        Is_On_coroutine = true;
        depthOfField.active = true;

        vignette.intensity.value = 0f;
        depthOfField.focalLength.value = 0f;

        for(float i = 0f; vignette.intensity.value <= 0.5; i++)
        {
            vignette.intensity.value += Value_Speed * Time.smoothDeltaTime;
            depthOfField.focalLength.value += 100 * Value_Speed * Time.smoothDeltaTime;
            yield return new WaitForSeconds(0.001f);
        }

        yield return new WaitForSeconds(0.2f);

        for (float i = 0f; vignette.intensity.value >= 0.5; i++)
        {
            vignette.intensity.value -= Value_Speed * Time.smoothDeltaTime;
            depthOfField.focalLength.value -= 100 * Value_Speed * Time.smoothDeltaTime;
            yield return new WaitForSeconds(0.02f);
        }

        vignette.active = false;
        Is_On_coroutine = false;
        depthOfField.active = false;
        yield return null;
    }
}

//ColorAdjust.saturation.value = -100;
//Vignette.center.value = new Vector2(2.0f, 2.0f);
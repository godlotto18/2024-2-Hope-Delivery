using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using TMPro.Examples;

public class HpBar : MonoBehaviour
{
    public Transform player;
    public Image healthbar;
    public float currentHealth;
    public float fullHealth = 100f;
    public bool isDead = false;

    public static HpBar Instance;
    public PostProcessVolume postProcessVolume;
    private Vignette vignette;
    private DepthOfField depthOfField;

    public float valueSpeed = 2.0f;
    private bool isEffectActive = false;

    public RectTransform healthbarContainer; // 체력바 전체 컨테이너
    public float maxHealthBarWidth = 200f; // 최대 체력바 너비 (픽셀)

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this; // 현재 인스턴스를 설정
        }
        currentHealth = fullHealth;
        UpdateHealthBar();

        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out vignette);
            postProcessVolume.profile.TryGetSettings(out depthOfField);

            if (vignette != null) vignette.intensity.value = 0f;
            if (depthOfField != null) depthOfField.active = false;
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void HPup()
    {
        currentHealth += 100f;
        if (currentHealth > fullHealth)
        {
            currentHealth = fullHealth; // 체력 초과 방지
        }
        Debug.Log(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHealthBar();

        if (!isEffectActive && !isDead)
        {
            StartCoroutine(ApplyHitEffect());
        }

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        healthbar.fillAmount = currentHealth / fullHealth;
    }

    public void HealthUpgrade(float hp)
    {
        fullHealth += hp;
        currentHealth = Mathf.Min(currentHealth + hp, fullHealth); // 증가한 체력만큼 현재 체력도 회복
        UpdateHealthBar();
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        isDead = true;
        StartCoroutine(ApplyDeathEffect());
        GameManager.Instance.GameOver();
    }

    private IEnumerator ApplyHitEffect()
    {
        isEffectActive = true;
        if (vignette != null) vignette.active = true;
        if (depthOfField != null) depthOfField.active = true;

        while (vignette != null && vignette.intensity.value < 0.3f)
        {
            vignette.intensity.value += valueSpeed * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        while (vignette != null && vignette.intensity.value > 0f)
        {
            vignette.intensity.value -= valueSpeed * Time.deltaTime;
            yield return null;
        }

        if (vignette != null) vignette.active = false;
        if (depthOfField != null) depthOfField.active = false;

        isEffectActive = false;
    }

    public IEnumerator ApplyDeathEffect()
    {
        if (vignette != null)
        {
            vignette.active = true;
            while (vignette.intensity.value < 1f)
            {
                vignette.intensity.value += valueSpeed * Time.deltaTime;
                yield return null;
            }
            vignette.intensity.value = 1f;
        }
    }
}

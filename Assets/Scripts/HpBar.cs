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

    public RectTransform healthbarContainer; // ü�¹� ��ü �����̳�
    public float maxHealthBarWidth = 200f; // �ִ� ü�¹� �ʺ� (�ȼ�)

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this; // ���� �ν��Ͻ��� ����
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
            currentHealth = fullHealth; // ü�� �ʰ� ����
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
        currentHealth = Mathf.Min(currentHealth + hp, fullHealth); // ������ ü�¸�ŭ ���� ü�µ� ȸ��
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

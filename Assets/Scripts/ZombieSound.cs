using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSound : MonoBehaviour
{
    public static ZombieSound Instance;
    public AudioClip[] basicSounds; // 3���� �⺻ �Ҹ�
    public AudioClip[] AttackSounds; // �´� �Ҹ�
    public AudioClip[] hitSounds; // �´� �Ҹ�
    public AudioClip[] deathSounds; // �״� �Ҹ�
    private AudioSource audioSource;

    public float minSoundInterval = 5f; // �⺻ �Ҹ� �ּ� ��� ����
    public float maxSoundInterval = 10f; // �⺻ �Ҹ� �ִ� ��� ����
    private float nextSoundInterval; // ���� �Ҹ� ��� ����
    private float timeSinceLastSound = 0f; // ������ �Ҹ� ��� �ð�

    public Transform player; // �÷��̾��� ��ġ
    public float maxDistance = 20f; // �ִ� �Ÿ� (�� ���������� ���� ����)
    public float minVolume = 0.1f; // �ּ� ����

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        // AudioSource ������Ʈ ��������
        audioSource = GetComponent<AudioSource>();
        // �⺻ �Ҹ� ��� ������ �������� ����
        SetRandomSoundInterval();
        // �⺻ �Ҹ� ��� ����
        PlayRandomBasicSound();
        
    }

    private void Update()
    {
        // 5�ʸ��� ���� �⺻ �Ҹ� ���
        timeSinceLastSound += Time.deltaTime;
        if (timeSinceLastSound >= nextSoundInterval)
        {
            PlayRandomBasicSound();
            timeSinceLastSound = 0f;
            SetRandomSoundInterval();
        }
        AdjustVolumeBasedOnDistance();
    }

    // �⺻ �Ҹ� �� �������� �ϳ� �����ؼ� ���
    private void PlayRandomBasicSound()
    {
        if (basicSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, basicSounds.Length);
            audioSource.clip = basicSounds[randomIndex];
            audioSource.Play();
        }
    }

    // ���� �¾��� ���� �Ҹ� ���
    public void PlayHitSound()
    {
        if (hitSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSounds.Length);
            audioSource.clip = hitSounds[randomIndex];
            audioSource.Play();
        }
        else
            Destroy(audioSource);
    }
    // ���� ������ ���� �Ҹ� ���
    public void PlayAttackSound()
    {
        if (AttackSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSounds.Length);
            audioSource.clip = hitSounds[randomIndex];
            audioSource.Play();
        }
        else
            Destroy(audioSource);
    }

    // ���� �׾��� ���� �Ҹ� ���
    public void PlayDeathSound()
    {
        if (deathSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, deathSounds.Length);
            audioSource.clip = deathSounds[randomIndex];
            audioSource.Play();
        }
        else
            Destroy(audioSource);
    }    
    // �÷��̾���� �Ÿ��� ����ؼ� ���� ����
    private void AdjustVolumeBasedOnDistance()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            // �Ÿ� ��ʷ� ���� ��� (�ִ� �Ÿ����� �ּ� ����, �ּ� �Ÿ����� �ִ� ����)
            float volume = Mathf.Clamp01(0.4f - distance / maxDistance);
            volume = Mathf.Max(volume, minVolume); // �ּ� ���� ����
            audioSource.volume = volume;
        }
        else
            Destroy(audioSource);
    }
    // ���� �Ҹ� ��� ������ �������� ����
    private void SetRandomSoundInterval()
    {
        nextSoundInterval = Random.Range(minSoundInterval, maxSoundInterval);
    }
}

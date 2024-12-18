using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSound : MonoBehaviour
{
    public static ZombieSound Instance;
    public AudioClip[] basicSounds; // 3개의 기본 소리
    public AudioClip[] AttackSounds; // 맞는 소리
    public AudioClip[] hitSounds; // 맞는 소리
    public AudioClip[] deathSounds; // 죽는 소리
    private AudioSource audioSource;

    public float minSoundInterval = 5f; // 기본 소리 최소 재생 간격
    public float maxSoundInterval = 10f; // 기본 소리 최대 재생 간격
    private float nextSoundInterval; // 다음 소리 재생 간격
    private float timeSinceLastSound = 0f; // 마지막 소리 재생 시간

    public Transform player; // 플레이어의 위치
    public float maxDistance = 20f; // 최대 거리 (이 범위에서만 음량 조절)
    public float minVolume = 0.1f; // 최소 음량

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
        // AudioSource 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>();
        // 기본 소리 재생 간격을 랜덤으로 설정
        SetRandomSoundInterval();
        // 기본 소리 재생 시작
        PlayRandomBasicSound();
        
    }

    private void Update()
    {
        // 5초마다 랜덤 기본 소리 재생
        timeSinceLastSound += Time.deltaTime;
        if (timeSinceLastSound >= nextSoundInterval)
        {
            PlayRandomBasicSound();
            timeSinceLastSound = 0f;
            SetRandomSoundInterval();
        }
        AdjustVolumeBasedOnDistance();
    }

    // 기본 소리 중 랜덤으로 하나 선택해서 재생
    private void PlayRandomBasicSound()
    {
        if (basicSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, basicSounds.Length);
            audioSource.clip = basicSounds[randomIndex];
            audioSource.Play();
        }
    }

    // 좀비가 맞았을 때의 소리 재생
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
    // 좀비가 때렸을 때의 소리 재생
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

    // 좀비가 죽었을 때의 소리 재생
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
    // 플레이어와의 거리에 비례해서 음량 조절
    private void AdjustVolumeBasedOnDistance()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            // 거리 비례로 음량 계산 (최대 거리에서 최소 음량, 최소 거리에서 최대 음량)
            float volume = Mathf.Clamp01(0.4f - distance / maxDistance);
            volume = Mathf.Max(volume, minVolume); // 최소 음량 보장
            audioSource.volume = volume;
        }
        else
            Destroy(audioSource);
    }
    // 다음 소리 재생 간격을 랜덤으로 설정
    private void SetRandomSoundInterval()
    {
        nextSoundInterval = Random.Range(minSoundInterval, maxSoundInterval);
    }
}

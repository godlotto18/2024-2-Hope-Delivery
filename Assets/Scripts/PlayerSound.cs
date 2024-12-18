using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public static PlayerSound Instance;
    public AudioClip WalkSound;
    public AudioClip SprintSound;
    public AudioClip JumpSound;
    public AudioClip HitMarkSound;
    private AudioSource audioSource;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        // AudioSource 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>();

    }

    // 공통 사운드 재생 메서드
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("오디오 클립 또는 AudioSource가 설정되지 않았습니다.");
        }
    }

    public void PlayWalkSound()
    {
        PlaySound(WalkSound);
    }
    public void PlaySprintSound()
    {
        PlaySound(SprintSound);
    }
    public void PlayJumpSound()
    {
        PlaySound(JumpSound);
    }
    public void PlayHitMarkSound()
    {
        PlaySound(HitMarkSound);
    }
}

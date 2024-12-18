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
        // AudioSource ������Ʈ ��������
        audioSource = GetComponent<AudioSource>();

    }

    // ���� ���� ��� �޼���
    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("����� Ŭ�� �Ǵ� AudioSource�� �������� �ʾҽ��ϴ�.");
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

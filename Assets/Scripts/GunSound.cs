using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSound : MonoBehaviour
{
    public static GunSound Instance;
    public AudioClip ShootSound;
    public AudioClip ReloadSound;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        // AudioSource 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>();

    }
    // 공통 사운드 재생 메서드
    public void GunSounds(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void PlayShootSound()
    {
        GunSounds(ShootSound);
    }
    public void PlayReloadSound()
    {
        GunSounds(ReloadSound);
    }
}

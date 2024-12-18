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
        // AudioSource ������Ʈ ��������
        audioSource = GetComponent<AudioSource>();

    }
    // ���� ���� ��� �޼���
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

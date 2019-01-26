using System;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    private AudioSource audio;
    
    private void Awake()
    {
        if (Instance != null)
            throw new Exception("More than 1 SoundPlayer!!!");

        Instance = this;

        audio = GetComponent<AudioSource>();
        audio.loop = false;
    }

    public void PlaySound(AudioClip clip)
    {
        audio.Stop();
        audio.clip = clip;
        audio.Play();
    }
}

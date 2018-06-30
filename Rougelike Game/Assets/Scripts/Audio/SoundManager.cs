using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Handles the sounds for the gameobject it is attached to
/// </summary>
public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    public static SoundManager Instance;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            throw new Exception("There are multiple Sound Managers!");
        }
    }

    public void PlayRandomizedPitch(AudioClip[] audioClip)
    {
        audioSource.clip = audioClip[Random.Range(0, audioClip.Length)];
        audioSource.pitch = Random.Range(0.7f, 1.3f);
        audioSource.Play(0);
    }
}

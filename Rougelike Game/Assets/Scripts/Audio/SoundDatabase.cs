using UnityEngine;
using System;

/// <summary>
/// Stores all of the sounds for easy access
/// </summary>
public class SoundDatabase : MonoBehaviour
{
    public AudioClip[] PlayerAttack;
    public AudioClip[] PlayerHighAttack;
    public AudioClip[] PlayerDamaged;
    public static SoundDatabase Instance;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            throw new Exception("There are multiple Sound Databases!");
        }
    }
}

using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

    public AudioSource buttonClick;
    public AudioClip clip;

    void start()
    {
        PlayClick();
    }

    public void PlayClick()
    {
        buttonClick.clip = clip;
        buttonClick.Play();
    }

}

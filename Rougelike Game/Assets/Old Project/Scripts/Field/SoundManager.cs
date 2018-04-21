using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource music;
    public static SoundManager instance;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;


	// Use this for initialization
	void Start () {
	
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

	}

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] sfx)
    {
        int randomIndex = Random.Range(0, sfx.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = sfx[randomIndex];
        efxSource.Play();


    }
}

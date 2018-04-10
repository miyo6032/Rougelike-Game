using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class LoadData : MonoBehaviour {
    public static LoadData instance = null;
    public bool dataLoaded = false;
    public bool launchTutorial = false;

    void Awake()
    {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        DontDestroyOnLoad(this);

    }

	public void Load()
    {

        if (File.Exists(Application.persistentDataPath + "/player.dat") && File.Exists(Application.persistentDataPath + "/inventory.dat") && File.Exists(Application.persistentDataPath + "/inventory.dat"))
        {

            dataLoaded = true;

            SceneManager.LoadScene(1);

        }

    }

    public void LoadTutorial()
    {
        launchTutorial = true;
        SceneManager.LoadScene(1);
    }

}
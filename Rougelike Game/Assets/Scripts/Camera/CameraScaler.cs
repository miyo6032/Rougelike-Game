using UnityEngine;
using System.Collections;

//Handles the camera movement and also sets the camera size
public class CameraScaler : MonoBehaviour
{
    //The target size of the camera in relation to the pixels
    public float referenceOrthographicSize;
    public float referencePixelsPerUnit;

    //The last size of the screen height
    private int lastSize;

    void Awake()
    {
        UpdateScreen();
    }

    void UpdateScreen()
    {
        lastSize = Screen.height;

        float refOrthoSize = (referenceOrthographicSize / referencePixelsPerUnit) * 0.5f;

        //The current orthographic size
        float orthoSize = (lastSize / referencePixelsPerUnit) * 0.5f;

        //Scale so that the orthographic size is a round number. If the orthographis size is larger than the reference, it will max out at the reference
        float multiplier = Mathf.Max(1, Mathf.Round(orthoSize / refOrthoSize));

        orthoSize /= multiplier;

        GetComponent<Camera>().orthographicSize = orthoSize;

    }

    void Update()
    {
        if (Screen.height != lastSize)
        {
            UpdateScreen();
        }

    }
}


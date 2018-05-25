using UnityEngine;
using System.Collections;

//Handles the camera movement and also sets the camera size
public class CameraMovement : MonoBehaviour
{

    private Transform player;
    PixelMovement pixelMovement;

    private int lastSize;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pixelMovement = GetComponent<PixelMovement>();
    }

    void Update()
    {

        if (player)
        {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
            //Basically round the position to keep pixels from looking weird.
            pixelMovement.UpdatePosition(transform.position);
        }

    }
}
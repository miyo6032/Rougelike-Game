﻿using UnityEngine;

/// <summary>
/// Handles the camera movement and also sets the camera size
/// </summary>
public class CameraMovement : MonoBehaviour
{
    private Transform player;
    private PixelMovement pixelMovement;
    private int lastSize;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pixelMovement = GetComponent<PixelMovement>();
    }

    private void Update()
    {
        if (player)
        {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
            // Basically round the position to keep pixels from looking weird.
            pixelMovement.UpdatePosition(transform.position);
        }
    }
}
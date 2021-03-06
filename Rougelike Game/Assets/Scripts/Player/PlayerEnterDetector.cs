﻿using UnityEngine;

/// <summary>
/// Detects when the player enters over the game object. Used for: loot bags, and entrances/exits
/// </summary>
public class PlayerEnterDetector : MonoBehaviour
{
    //When the player walks on top of the bag
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerEnter(other);
        }
    }

    //When the player walks off the bag
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerExit(other);
        }
    }

    /// <summary>
    /// Called when the player enters this game object's collider
    /// </summary>
    public virtual void PlayerEnter(Collider2D player) { }

    /// <summary>
    /// Called when the player exits this game object's collider
    /// </summary>
    public virtual void PlayerExit(Collider2D player) { }
}

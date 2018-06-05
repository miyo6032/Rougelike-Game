﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects when the player enters and exits and activates the loot inventory
/// </summary>
public class LootBag : MonoBehaviour
{
    public List<ItemSave> items = new List<ItemSave>();

    /// <summary>
    /// Adds the items to the items list
    /// </summary>
    /// <param name="itemDrops"></param>
    public void AddItems(List<ItemSave> itemDrops)
    {
        foreach (ItemSave item in itemDrops)
        {
            if (items.Count < 9)
            {
                items.Add(item);
            }
        }
    }

    //When the player walks on top of the bag
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            StaticCanvasList.instance.lootInventory.LoadInventory(this);
            StaticCanvasList.instance.gameUI.ToggleLootPanel();
        }
    }

    //When the player walks off the bag
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            StaticCanvasList.instance.lootInventory.UnloadInventory();
            StaticCanvasList.instance.gameUI.ToggleLootPanel();
            if (items.Count == 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

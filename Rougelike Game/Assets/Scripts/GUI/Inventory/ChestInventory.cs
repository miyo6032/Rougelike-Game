﻿using System.Collections.Generic;
using UnityEngine;

public class ChestInventory : MonoBehaviour {

    public List<ItemSlot> chestSlots;

    public InventoryManager inventoryManager;

    private Chest loadedChest;

    public void OpenChest(Chest newchest)
    {
        if (loadedChest == null)
        {
            loadedChest = newchest;

            foreach (ItemSave itemSave in newchest.chestItems)
            {
                inventoryManager.AddItemToSlot(itemSave.item, chestSlots[itemSave.slotPosition]);
            }
        }

        gameObject.SetActive(true);

        if (!inventoryManager.gameObject.activeSelf)
        {
            inventoryManager.Toggle();
        }
    }

    public void CloseChest()
    {
        if (loadedChest != null)
        {
            loadedChest.chestItems = ExtractItemsFromInventory();
            loadedChest = null;
            StaticCanvasList.instance.inventoryTooltip.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    //Removes everything from inventor and stores it in an array to pass to the loot bag
    List<ItemSave> ExtractItemsFromInventory()
    {
        List<ItemSave> items = new List<ItemSave>();
        for (int i = 0; i < chestSlots.Count; i++)
        {
            if (chestSlots[i].item != null)
            {
                items.Add(new ItemSave(chestSlots[i].item.item, i));
                Destroy(chestSlots[i].item.gameObject);
                chestSlots[i].item = null;
            }
        }
        return items;
    }

}
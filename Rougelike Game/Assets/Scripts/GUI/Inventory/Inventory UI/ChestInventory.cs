﻿using System.Collections.Generic;
using UnityEngine;

public class ChestInventory : MonoBehaviour {

    public List<ItemSlot> chestSlots;

    public InventoryManager inventoryManager;

    private Chest loadedChest;

    public void OpenChest(Chest newchest)
    {
        if (loadedChest != newchest)
        {
            CloseChest();

            loadedChest = newchest;

            newchest.SetOpenSprite();

            foreach (ItemSave itemSave in newchest.chestItems)
            {
                inventoryManager.AddItemToSlot(itemSave.item, chestSlots[itemSave.slotPosition]);
            }
        }

        gameObject.SetActive(true);
        StaticCanvasList.instance.panelManagement.SetRightPanel(gameObject);

        if (!inventoryManager.gameObject.activeSelf)
        {
            inventoryManager.Toggle();
        }
    }

    public void CloseChest()
    {
        if (loadedChest != null)
        {
            loadedChest.SetClosedSprite();
            loadedChest.chestItems = ExtractItemsFromInventory();
            loadedChest = null;
            StaticCanvasList.instance.inventoryTooltip.gameObject.SetActive(false);
            gameObject.SetActive(false);
            StaticCanvasList.instance.panelManagement.SetRightPanel(null);
        }
    }

    //Removes everything from inventor and stores it in an array to pass to the loot bag
    List<ItemSave> ExtractItemsFromInventory()
    {
        List<ItemSave> items = new List<ItemSave>();
        for (int i = 0; i < chestSlots.Count; i++)
        {
            if (chestSlots[i].GetItem() != null)
            {
                items.Add(new ItemSave(chestSlots[i].GetItem().item, i));
                Destroy(chestSlots[i].GetItem().gameObject);
                chestSlots[i].SetItem(null);
            }
        }
        return items;
    }

}

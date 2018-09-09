using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles showing items from a chest, and toggling that inventory
/// </summary>
public class ChestInventory : MonoBehaviour
{
    public static ChestInventory instance;
    public List<ItemSlot> chestSlots;
    private Chest loadedChest;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Duplicate " + this.GetType().Name);
            Destroy(gameObject);
        }
        gameObject.SetActive(false);
    }

    public void OpenChest(Chest newchest)
    {
        if (loadedChest != newchest)
        {
            CloseChest();
            loadedChest = newchest;
            newchest.SetOpenSprite();
            foreach (ItemSave itemSave in newchest.chestItems)
            {
                InventoryManager.instance.AddItemToSlot(itemSave.itemStack, chestSlots[itemSave.slotPosition]);
            }
        }

        gameObject.SetActive(true);
        PanelManagement.instance.SetRightPanel(gameObject);
        if (!InventoryManager.instance.gameObject.activeSelf)
        {
            InventoryManager.instance.Toggle();
        }
    }

    public void CloseChest()
    {
        if (loadedChest != null)
        {
            loadedChest.SetClosedSprite();
            loadedChest.chestItems = ExtractItemsFromInventory();
            loadedChest = null;
            Tooltip.instance.gameObject.SetActive(false);
            gameObject.SetActive(false);
            PanelManagement.instance.SetRightPanel(null);
        }
    }

    /// <summary>
    /// Removes everything from inventory and stores it in an array to pass to the chest
    /// </summary>
    /// <returns></returns>
    private List<ItemSave> ExtractItemsFromInventory()
    {
        List<ItemSave> items = new List<ItemSave>();
        for (int i = 0; i < chestSlots.Count; i++)
        {
            if (chestSlots[i].itemStack != null)
            {
                items.Add(new ItemSave(chestSlots[i].itemStack, i));
                chestSlots[i].RemoveItem();
            }
        }

        return items;
    }
}
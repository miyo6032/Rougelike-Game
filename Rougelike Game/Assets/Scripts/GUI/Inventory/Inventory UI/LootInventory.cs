using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the loot bag inventory
/// </summary>
public class LootInventory : MonoBehaviour {

	public List<ItemSlot> lootSlots;

    private LootBag loadedBag;

    public static LootInventory instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Duplicate " + this.GetType().Name);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Get data from the loot bag and load the items into the loot inventory
    /// </summary>
    /// <param name="bag"></param>
    public void LoadInventory(LootBag bag)
    {
        loadedBag = bag;

        foreach(ItemSave itemSave in bag.items)
        {
            InventoryManager.instance.AddItemToSlot(itemSave.item, lootSlots[itemSave.slotPosition]);
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Unload the inventory items - called when the player walks away from the bag
    /// </summary>
    public void UnloadInventory()
    {
        loadedBag.items = ExtractItemsFromInventory();
        loadedBag = null;
        Tooltip.instance.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Removes everything from inventory and stores it in an array to pass to the loot bag
    /// </summary>
    /// <returns></returns>
    private List<ItemSave> ExtractItemsFromInventory()
    {
        List<ItemSave> items = new List<ItemSave>();
        for (int i = 0; i < lootSlots.Count; i++)
        {
            if(lootSlots[i].itemStack != null)
            {
                items.Add(new ItemSave(lootSlots[i].itemStack, lootSlots[i].itemStack.amount));
                lootSlots[i].RemoveItem();
            }
        }
        return items;
    }

    // Actually open the ui for the loot - if the inventory is closed, open that as well
    // Called from a button
    public void OpenLoot()
    {
        if (!InventoryManager.instance.gameObject.activeSelf)
        {
            InventoryManager.instance.Toggle();
        }
        gameObject.SetActive(true);
    }

}

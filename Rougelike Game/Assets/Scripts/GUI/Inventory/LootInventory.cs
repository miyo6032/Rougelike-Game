using System.Collections.Generic;
using UnityEngine;

//Handles the loot portion of the inventory
public class LootInventory : MonoBehaviour {

	public List<ItemSlot> lootSlots;

    public InventoryManager inventoryManager;

    private LootBag loadedBag;

    //Get data from the loot bag and load the items into the loot inventory
    public void LoadInventory(LootBag loadedBag)
    {
        this.loadedBag = loadedBag;

        foreach(ItemSave itemSave in loadedBag.items)
        {
            inventoryManager.AddItemToSlot(itemSave.item, lootSlots[itemSave.slotPosition]);
        }

        gameObject.SetActive(true);
    }

    //Unload the inventory items - called when the player walks away from the bag
    public void UnloadInventory()
    {
        loadedBag.items = ExtractItemsFromInventory();
        loadedBag = null;
        StaticCanvasList.instance.inventoryTooltip.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    //Find all items in the loot inventory and record them as a list of items
    List<Item> ListItemsInInventory()
    {
        List<Item> items = new List<Item>();
        foreach(ItemSlot slot in lootSlots)
        {
            if(slot.item != null)
            {
                items.Add(slot.item.item);
            }
        }
        return items;
    }

    //Removes everything from inventor and stores it in an array to pass to the loot bag
    List<ItemSave> ExtractItemsFromInventory()
    {
        List<ItemSave> items = new List<ItemSave>();
        for(int i = 0; i < lootSlots.Count; i++)
        {
            if(lootSlots[i].item != null)
            {
                items.Add(new ItemSave(lootSlots[i].item.item, i));
                Destroy(lootSlots[i].item.gameObject);
                lootSlots[i].item = null;
            }
        }
        return items;
    }

    //Actually open the ui for the loot - if the inventory is closed, open that as well
    public void OpenLoot()
    {
        if (!inventoryManager.gameObject.activeSelf)
        {
            inventoryManager.Toggle();
        }
        gameObject.SetActive(true);
    }

}

using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class InventoryManager : MonoBehaviour {
    public List<ItemSlot> slots = new List<ItemSlot>();
    public List<ItemSlot> equipSlots = new List<ItemSlot>();

    public ItemSlot slotPrefab;
    public ItemInstance itemPrefab;
    public Transform inventoryPanel;

    //The item attached to the mouse pointer, if any
    public ItemInstance attachedItem = null;

    ItemDatabase database;

    void Start () {

        database = GetComponent<ItemDatabase>();
        database.PopulateItemModuleDatabase();

        AddItem(database.GenerateItem(4, 0));
        AddItem(database.GenerateItem(5, 0));
        AddItem(database.GenerateItem(3, 0));

    }

    public void AddItem(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {//If there is no item in the slot, Adds new item to that slot
            if (slots[i].item == null)
            {
                ItemInstance itemObj = Instantiate(itemPrefab);
                itemObj.Initialize(item, slots[i]);
                slots[i].item = itemObj;
                return;
            }
        }
    }

    //Return true if a certain item is in the inventory
    //Used for seeing if an item should stack into something
    public bool ItemInInventory(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item != null && slots[i].item.item == item)
            {
                return true;
            }
        }
        return false;
    }

}

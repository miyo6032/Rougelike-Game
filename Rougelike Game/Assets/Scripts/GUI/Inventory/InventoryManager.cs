using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class InventoryManager : MonoBehaviour {
    [HideInInspector]
    public List<ItemSlot> slots = new List<ItemSlot>();

    public List<ItemSlot> equipSlots = new List<ItemSlot>();

    public ItemSlot slotPrefab;
    public ItemInstance itemPrefab;
    public Transform inventoryPanel;

    //The item attached to the mouse pointer, if any
    public ItemInstance attachedItem;

    ItemDatabase database;

    void Start () {

        database = GetComponent<ItemDatabase>();
        database.ConstructItemDatabase();

        //Add(instantiate) the inventory slots to the inventory panel
        for (int i = 0; i < 18; i++)
        {//Inventory slots for the inventory
            ItemSlot instance = Instantiate(slotPrefab);
            instance.transform.SetParent(inventoryPanel);
            instance.item = null;
            instance.id = i;
            slots.Add(instance);
            instance.transform.localScale = new Vector3(1, 1, 1);
        }

        AddItem(0);
        AddItem(1);

    }

    //Adds an item to the inventory into an empty slot - or stack it
    public void AddItem(int id)
    {
        Item itemToAdd = database.GetItemByID(id);//Searches the database for the item

        if (itemToAdd.Stackable && ItemInInventory(itemToAdd))
        {//Adds and existing item to the inventory
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].item != null && slots[i].item.item.Id == id)
                {
                    ItemInstance data = slots[i].item;
                    data.amount++;
                    data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < slots.Count; i++)
            {//If there is no item in the slot, Adds new item to that slot
                if (slots[i].item == null)
                {
                    ItemInstance itemObj = Instantiate(itemPrefab);
                    itemObj.Initialize(itemToAdd, slots[i]);
                    slots[i].item = itemObj;
                    return;
                }
            }
        }
    }

    //Return true if a certain item is in the inventory
    //Used for seeing if an item should stack into something
    public bool ItemInInventory(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item != null && slots[i].item.item.Id == item.Id)
            {
                return true;
            }
        }
        return false;
    }

}

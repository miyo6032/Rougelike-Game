using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//Holds the inventory slot information, and implements some necessary item operations
public class InventoryManager : MonoBehaviour {
    public List<ItemSlot> slots = new List<ItemSlot>();
    public List<ItemSlot> equipSlots = new List<ItemSlot>();

    public ItemInstance itemPrefab;
    public Transform inventoryPanel;

    //The item attached to the mouse pointer, if any
    public ItemInstance attachedItem = null;

    ItemDatabase database;

    void Start () {
        database = GetComponent<ItemDatabase>();
        database.PopulateItemModuleDatabase();

        AddItemToSlot(database.GenerateItem(4, 0), FindNextOpenSlot(slots));
        AddItemToSlot(database.GenerateItem(5, 0), FindNextOpenSlot(slots));
        AddItemToSlot(database.GenerateItem(3, 0), FindNextOpenSlot(slots));

        gameObject.SetActive(false);
    }

    public void AddItemToSlot(Item item, ItemSlot slot)
    {
        ItemInstance itemObj = Instantiate(itemPrefab);
        itemObj.Initialize(item, slot);
        slot.item = itemObj;
    }

    //
    public ItemSlot FindNextOpenSlot(List<ItemSlot> slotList)
    {
        for (int i = 0; i < slotList.Count; i++)
        {//If there is no item in the slot, Adds new item to that slot
            if (slotList[i].item == null)
            {
                return slotList[i].GetComponent<ItemSlot>();
            }
        }
        return null;
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

    //Toggles the inventory to show or hide
    public void Toggle()
    {
        if (gameObject.activeSelf)
        {
            if(attachedItem != null)
            {
                AddItemToSlot(attachedItem.item, FindNextOpenSlot(slots));
                Destroy(attachedItem.gameObject);
            }
            StaticCanvasList.instance.inventoryTooltip.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

}

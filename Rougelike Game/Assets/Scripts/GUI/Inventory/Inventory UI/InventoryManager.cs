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

    void Start () {
        StaticCanvasList.instance.itemModuleDatabase.PopulateItemModuleDatabase();

        TextureDatabase textures = StaticCanvasList.instance.textureDatabase;
        ItemDatabase itemDatabase = StaticCanvasList.instance.itemDatabase;
        ItemGenerator itemGenerator = StaticCanvasList.instance.itemGenerator;

        itemDatabase.ConstructItemDatabase();
        textures.LoadAllTextures();

        AddItem(itemGenerator.GenerateItem(1, 0));
        AddItem(itemGenerator.GenerateItem(1, 0));
        AddItem(itemGenerator.GenerateItem(1, 1));
        AddItem(itemGenerator.GenerateItem(1, 2));

        
        AddItem(itemDatabase.GetItemByName("Minor Health Potion"), 3);

        gameObject.SetActive(false);
    }

    public void AddItem(Item item)
    {
        ItemSlot slot = FindSlotWithItem(item.Title, slots);
        if (slot)
        {
            slot.GetItem().ChangeAmount(1);
        }
        else
        {
            AddItemToSlot(item, FindNextOpenSlot(slots));
        }
    }

    public void AddItem(Item item, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            AddItem(item);
        }
    }

    public void AddItemToSlot(Item item, ItemSlot slot)
    {
        ItemInstance itemObj = Instantiate(itemPrefab);
        itemObj.Initialize(item, slot);
        slot.SetItem(itemObj);
    }

    // Find the next slot to place a new item in
    public ItemSlot FindNextOpenSlot(List<ItemSlot> slotList)
    {
        for (int i = 0; i < slotList.Count; i++)
        {//If there is no item in the slot, Adds new item to that slot
            if (slotList[i].GetItem() == null)
            {
                return slotList[i].GetComponent<ItemSlot>();
            }
        }
        return null;
    }

    public ItemSlot FindSlotWithItem(string itemId, List<ItemSlot> slotList)
    {
        for (int i = 0; i < slotList.Count; i++)
        {//If there is no item in the slot, Adds new item to that slot
            if (slotList[i].GetItem() != null && slotList[i].GetItem().item.Title == itemId)
            {
                return slotList[i].GetComponent<ItemSlot>();
            }
        }
        //No existing items
        return null;
    }

    //Return true if a certain item is in the inventory
    //Used for seeing if an item should stack into something
    public bool ItemInInventory(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].GetItem() != null && slots[i].GetItem().item == item)
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

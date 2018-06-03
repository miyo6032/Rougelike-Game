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

    ItemGenerator itemGenerator;

    void Start () {
        StaticCanvasList.instance.itemModuleDatabase.PopulateItemModuleDatabase();

        TextureDatabase textures = StaticCanvasList.instance.textureDatabase;
        ItemDatabase itemDatabase = StaticCanvasList.instance.itemDatabase;
        itemGenerator = StaticCanvasList.instance.itemGenerator;

        itemDatabase.ConstructItemDatabase();
        textures.LoadAllTextures();

        // Automatically equip the four starting items
        AddItemToSlot(itemGenerator.GenerateItem(1, 0), equipSlots[0]);
        AddItemToSlot(itemGenerator.GenerateItem(1, 0), equipSlots[1]);
        AddItemToSlot(itemGenerator.GenerateItem(1, 1), equipSlots[2]);
        AddItemToSlot(itemGenerator.GenerateItem(1, 2), equipSlots[3]);

        AddItem(itemDatabase.GetItemByName("Minor Health Potion"), 3);

        gameObject.SetActive(false);
    }

    public void AddGeneratedItem(int level, int equipment)
    {
        AddItem(itemGenerator.GenerateItem(level, equipment));
    }

    public void AddItem(Item item)
    {
        ItemSlot slot = FindSlotWithItem(item.Title, slots);
        if (slot && item.Stackable)
        {
            ItemInstance instance = slot.GetItem();
            instance.ChangeAmount(1);
            slot.SetItem(instance);
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
        itemObj.Initialize(item);
        slot.ItemDropIntoEmpty(itemObj);
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
                FindNextOpenSlot(slots).ItemDropIntoEmpty(attachedItem);
                attachedItem = null;
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

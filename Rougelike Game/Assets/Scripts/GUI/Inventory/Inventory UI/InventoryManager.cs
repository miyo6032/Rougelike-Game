using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds the inventory slot information, and implements some necessary item operations
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public List<ItemSlot> slots = new List<ItemSlot>();
    public List<ItemSlot> equipSlots = new List<ItemSlot>();
    public ItemInstance itemPrefab;

    public Item TEST;

    // The item attached to the mouse pointer, if any
    public ItemInstance attachedItem = null;
    ItemGenerator itemGenerator;

    void Start()
    {
        //Setup all of the databases
        StaticCanvasList.instance.itemModuleDatabase.PopulateItemModuleDatabase();
        TextureDatabase textures = StaticCanvasList.instance.textureDatabase;
        ItemDatabase itemDatabase = StaticCanvasList.instance.itemDatabase;
        itemGenerator = StaticCanvasList.instance.itemGenerator;
        itemDatabase.ConstructItemDatabase();
        textures.LoadAllTextures();

        // Automatically equip the four starting items
        AddItemToSlot(itemGenerator.GenerateItem(1, 0), equipSlots[0], 1);
        AddItemToSlot(itemGenerator.GenerateItem(1, 0), equipSlots[1], 1);
        AddItemToSlot(itemGenerator.GenerateItem(1, 1), equipSlots[2], 1);
        AddItemToSlot(itemGenerator.GenerateItem(1, 2), equipSlots[3], 1);

        AddItem(TEST, 1);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Adds a randomly generated equipment item based on level and equipment type
    /// </summary>
    /// <param name="level"></param>
    /// <param name="equipment"></param>
    public void AddGeneratedItem(int level, int equipment)
    {
        AddItem(itemGenerator.GenerateItem(level, equipment));
    }

    /// <summary>
    /// Creates a new item instance and stacks it to an existing stack, or to the next available slot
    /// </summary>
    /// <param name="item"></param>
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
            AddItemToSlot(item, FindNextOpenSlot(slots), 1);
        }
    }

    /// <summary>
    /// Creates multiple identical items
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    public void AddItem(Item item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            AddItem(item);
        }
    }

    /// <summary>
    /// Adds an item to a specified slot
    /// </summary>
    /// <param name="item"></param>
    /// <param name="slot"></param>
    public void AddItemToSlot(Item item, ItemSlot slot, int amount)
    {
        if (slot.GetItem() == null)
        {
            ItemInstance itemObj = Instantiate(itemPrefab);
            itemObj.Initialize(item);
            itemObj.ChangeAmount(amount - 1);
            slot.ItemDropIntoEmpty(itemObj);
        }
    }

    /// <summary>
    /// Find the next slot to place a new item in
    /// </summary>
    /// <param name="slotList"></param>
    /// <returns></returns>
    public ItemSlot FindNextOpenSlot(List<ItemSlot> slotList)
    {
        foreach (ItemSlot slot in slotList)
        {
            // If there is no item in the slot, Adds new item to that slot
            if (slot.GetItem() == null)
            {
                return slot.GetComponent<ItemSlot>();
            }
        }

        return null;
    }

    /// <summary>
    /// Finds a slot (if it exists) that already has the same item
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="slotList"></param>
    /// <returns></returns>
    public ItemSlot FindSlotWithItem(string itemId, List<ItemSlot> slotList)
    {
        foreach (ItemSlot slot in slotList)
        {
            // If there is no item in the slot, Adds new item to that slot
            if (slot.GetItem() != null && slot.GetItem().item.Title == itemId)
            {
                return slot.GetComponent<ItemSlot>();
            }
        }

        //No existing items
        return null;
    }

    /// <summary>
    /// Toggles the inventory to show or hide
    /// </summary>
    public void Toggle()
    {
        if (gameObject.activeSelf)
        {
            if (attachedItem != null)
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

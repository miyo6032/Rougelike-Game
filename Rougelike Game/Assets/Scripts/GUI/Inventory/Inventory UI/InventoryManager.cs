using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds the inventory slot information, and implements some necessary item operations
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public List<ItemSlot> slots = new List<ItemSlot>();
    public List<ItemSlot> equipSlots = new List<ItemSlot>();

    public ItemScriptableObject meat;
    public ItemScriptableObject starterSword;
    public ItemScriptableObject starterHelmet;
    public ItemScriptableObject starterArmor;

    private void Start()
    {
        //Setup all of the databases
        TextureDatabase textures = StaticCanvasList.instance.textureDatabase;
        textures.LoadAllTextures();

        AddScriptableItem(meat, 1);
        EquipScriptableItem(starterSword, equipSlots[0]);
        EquipScriptableItem(starterSword, equipSlots[1]);
        EquipScriptableItem(starterHelmet, equipSlots[3]);
        EquipScriptableItem(starterArmor, equipSlots[2]);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Adds a scriptable item, allowing sprites to be dragged in
    /// </summary>
    /// <param name="item"></param>
    public void AddScriptableItem(ItemScriptableObject item, int amount)
    {
        item.Start();
        AddItem(new ItemStack(item.item, amount));
    }

    public void EquipScriptableItem(ItemScriptableObject item, ItemSlot slot)
    {
        item.Start();
        AddItemToSlot(new ItemStack(item.item, 1), slot);
    }

    /// <summary>
    /// Creates a new item instance and stacks it to an existing stack, or to the next available slot
    /// </summary>
    /// <param name="stack"></param>
    public void AddItem(ItemStack stack)
    {
        ItemSlot slot = FindSlotWithItem(stack.item, slots);
        if (slot && stack.item.Stackable)
        {
            slot.ItemDropIntoFull(stack);
        }
        else
        {
            AddItemToSlot(stack, FindNextOpenSlot(slots));
        }
    }

    /// <summary>
    /// Adds an item to a specified slot
    /// </summary>
    public void AddItemToSlot(ItemStack item, ItemSlot slot)
    {
        slot.ItemDropIntoEmpty(item);
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
            if (slot.itemStack == null)
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
    public ItemSlot FindSlotWithItem(Item item, List<ItemSlot> slotList)
    {
        foreach (ItemSlot slot in slotList)
        {
            // If there is no item in the slot, Adds new item to that slot
            if (slot.itemStack != null && slot.itemStack.item == item)
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
            if (StaticCanvasList.instance.itemDragger.itemStack != null)
            {
                FindNextOpenSlot(slots).ItemDropIntoEmpty(StaticCanvasList.instance.itemDragger.itemStack);
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

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds the inventory slot information, and implements some necessary item operations
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public ItemScriptableObject invisible;

    public List<ItemSlot> slots = new List<ItemSlot>();
    public List<ItemSlot> equipSlots = new List<ItemSlot>();

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

    /// <summary>
    /// Creates a new item instance and stacks it to an existing stack, or to the next available slot.
    /// Returns false if unable to add item to the inventory
    /// </summary>
    public bool AddItem(ItemStack stack)
    {
        ItemSlot slot = FindSlotWithItem(stack.item, slots);
        if (slot && stack.item.stackable)
        {
            slot.ItemDropIntoFull(stack);
            return true;
        }
        else
        {
            slot = FindNextOpenSlot(slots);
            if (slot)
            {
                AddItemToSlot(stack, slot);
                return true;
            }
        }
        return false;
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
    public ItemSlot FindSlotWithItem(ItemScriptableObject item, List<ItemSlot> slotList)
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
            if (ItemDragger.instance.itemStack != null)
            {
                FindNextOpenSlot(slots).ItemDropIntoEmpty(ItemDragger.instance.itemStack);
            }

            Tooltip.instance.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
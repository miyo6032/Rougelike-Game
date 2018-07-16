using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds the inventory slot information, and implements some necessary item operations
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

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

        //Setup all of the databases
        TextureDatabase.instance.LoadAllTextures();

        gameObject.SetActive(false);
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

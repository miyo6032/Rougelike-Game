using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// The slot that represents an equipped item when dropped into
/// </summary>
public class EquipSlot : ItemSlot, IPointerClickHandler
{
    public int equipmentSlot;
    public Sprite emptySprite;
    public Sprite fullSprite;
    public Image slotImage;
    public PlayerStats playerStat;

    // When the player drops an item from clicking
    public override void OnPointerClick(PointerEventData eventData)
    {
        ItemInstance droppedItem = StaticCanvasList.instance.inventoryManager.attachedItem;
        // If there is an item attached to the mouse pointer
        if (droppedItem && ItemCanBeEquipped(droppedItem))
        {
            // Do the item drop first to check to see if the item can be equipped
            ItemDropIntoEmpty(droppedItem);
            StaticCanvasList.instance.inventoryManager.attachedItem = null;
        }
    }

    /// <summary>
    /// When the items is dropped, equip it and attach it to the slot
    /// </summary>
    /// <param name="droppedItem"></param>
    public override void ItemDropIntoEmpty(ItemInstance droppedItem)
    {
        // If there is an item attached to the mouse pointer
        if (droppedItem && item == null && ItemCanBeEquipped(droppedItem))
        {
            LinkItem(droppedItem);
            playerStat.EquipItem(droppedItem, this);
            droppedItem.attached = false;
            slotImage.sprite = fullSprite;
        }
    }

    /// <summary>
    /// When the item is dropped, unequip the previous item and equip this one
    /// </summary>
    /// <param name="droppedItem"></param>
    public override void ItemDropIntoFull(ItemInstance droppedItem)
    {
        if (droppedItem && item != null && ItemCanBeEquipped(droppedItem))
        {
            playerStat.UnequipItem(item);
            item.PickItemUp();
            LinkItem(droppedItem);
            playerStat.EquipItem(droppedItem, this);
            droppedItem.attached = false;
            slotImage.sprite = fullSprite;
        }
    }

    /// <summary>
    /// When dropped, the remove the slot's background empty image
    /// </summary>
    public void SlotImageToEmpty()
    {
        slotImage.sprite = emptySprite;
    }

    /// <summary>
    /// If the player is qualified to equip the item
    /// </summary>
    /// <param name="droppedItem"></param>
    /// <returns></returns>
    bool ItemCanBeEquipped(ItemInstance droppedItem)
    {
        return droppedItem.item.EquippedSlot == equipmentSlot && playerStat.GetLevel() >= droppedItem.item.ItemLevel;
    }
}

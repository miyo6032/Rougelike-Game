using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Rougelike_Game.Assets.Scripts.GUI.Inventory.Items_and_Slots;

/// <summary>
/// The slot that represents an equipped item when dropped into
/// </summary>
public class EquipSlot : ItemSlot
{
    public EquipmentType equipmentSlot;
    public Sprite emptySprite;
    public Sprite fullSprite;
    public Image slotImage;
    public PlayerStats playerStat;

    /// <summary>
    /// When the items is dropped, equip it and attach it to the slot
    /// </summary>
    /// <param name="droppedItem"></param>
    public override void ItemDropIntoEmpty(ItemStack droppedItem)
    {
        // If there is an item attached to the mouse pointer
        if (ItemCanBeEquipped(droppedItem.item))
        {
            SetItem(droppedItem);
            playerStat.EquipItem(droppedItem.item);
            ItemDragger.instance.RemoveItem();
            slotImage.sprite = fullSprite;
        }
    }

    /// <summary>
    /// When the item is dropped, unequip the previous item and equip this one
    /// </summary>
    /// <param name="droppedItem"></param>
    public override void ItemDropIntoFull(ItemStack droppedItem)
    {
        if (ItemCanBeEquipped(droppedItem.item))
        {
            playerStat.UnequipItem(itemStack.item);
            playerStat.EquipItem(droppedItem.item);
            PickItemUp();
            SetItem(droppedItem);
            slotImage.sprite = fullSprite;
        }
    }

    public override void PickItemUp()
    {
        base.PickItemUp();
        slotImage.sprite = emptySprite;
    }

    public override void RemoveItem()
    {
        playerStat.UnequipItem(itemStack.item);
        base.RemoveItem();
    }

    /// <summary>
    /// If the player is qualified to equip the item
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    private bool ItemCanBeEquipped(ItemScriptableObject item)
    {
        return item.equippedSlot == equipmentSlot && playerStat.GetLevel() >= item.level;
    }
}
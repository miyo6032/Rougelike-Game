using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class EquipSlot : ItemSlot, IPointerClickHandler
{
    public int equipmentSlot;
    public SkillDatabase database;

    public Sprite emptySprite;
    public Sprite fullSprite;

    public Image slotImage;
    public PlayerStats playerStat;

    //When the player drops an item from clicking
    public override void OnPointerClick(PointerEventData eventData)
    {
        ItemInstance droppedItem = StaticCanvasList.instance.inventoryManager.attachedItem;
        //If there is an item attached to the mouse pointer
        if (droppedItem)
        {
            //Do the item drop first to check to see if the item can be equipped
            ItemDropIntoEmpty(droppedItem);
            StaticCanvasList.instance.inventoryManager.attachedItem = null;
        }
    }

    //Handles when an item is dropped upon the slot
    //Does item exchanging, equipping and stuff like that
    public override void ItemDropIntoEmpty(ItemInstance droppedItem)
    {
        //If there is an item attached to the mouse pointer
        if (droppedItem && item == null && ItemCanBeEquipped(droppedItem))
        {
            LinkItemAndSlot(droppedItem, this);
            playerStat.EquipItem(droppedItem, this);
            droppedItem.attached = false;
            slotImage.sprite = fullSprite;
        }
    }

    //Overrides to address a few differences with equipping
    public override void ItemDropIntoFull(ItemInstance droppedItem)
    {
        if (droppedItem && item != null && ItemCanBeEquipped(droppedItem))
        {
            playerStat.UnequipItem(item);
            item.PickItemUp();
            LinkItemAndSlot(droppedItem, this);
            playerStat.EquipItem(droppedItem, this);
            droppedItem.attached = false;
            slotImage.sprite = fullSprite;
        }
    }

    public void SlotImageToEmpty()
    {
        slotImage.sprite = emptySprite;
    }

    bool ItemCanBeEquipped(ItemInstance droppedItem)
    {
        return droppedItem.item.EquippedSlot == equipmentSlot 
            && playerStat.GetLevel() >= droppedItem.item.ItemLevel;
    }

}

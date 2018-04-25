using UnityEngine.EventSystems;
using UnityEngine;

public class EquipSlot : ItemSlot, IPointerClickHandler
{
    public int equipmentSlot;

    //When the player drops an item from clicking
    public override void OnPointerClick(PointerEventData eventData)
    {
        ItemInstance droppedItem = StaticCanvasList.instance.inventoryManager.attachedItem;
        //If there is an item attached to the mouse pointer

        if (droppedItem)
        {
            //Do the item drop first to check to see if the item can be equipped
            ItemDrop(droppedItem);
        }
    }

    public override void ItemDrop(ItemInstance droppedItem)
    {
        //If the item can be equipped in that slot
        if (droppedItem.item.EquippedSlot == equipmentSlot)
        {
            //Attempt to equip the item. EquipItem() will return true if the item is equippable
            if (playerStat.level >= droppedItem.item.ItemLevel)
            {
                //If the previous slot was empty, just add a new item to ti
                if (item == null)
                {
                    LinkItemAndSlot(droppedItem, this);
                    StaticCanvasList.instance.inventoryManager.attachedItem = null;
                }
                else
                {
                    playerStat.UnequipItem(item);
                    item.SetItemAttached();
                    LinkItemAndSlot(droppedItem, this);
                }
                playerStat.EquipItem(droppedItem, this);
                droppedItem.attached = false;
            }
        }
    }
}

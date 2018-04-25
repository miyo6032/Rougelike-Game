﻿using UnityEngine.EventSystems;
using UnityEngine;

//The item slot that items will go into
//Responsbile for handling when the item is dropped on the item slot
public class ItemSlot : Slot<Item>, IPointerClickHandler
{
    //When the player drops an item from clicking
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        ItemInstance droppedItem = StaticCanvasList.instance.inventoryManager.attachedItem;
        //If there is an item attached to the mouse pointer
        if (droppedItem)
        {
            //Disconnnect the item from the mouse
            droppedItem.attached = false;
            StaticCanvasList.instance.inventoryManager.attachedItem = null;
            ItemDrop(droppedItem);
        }
    }

    //Handles when an item is dropped upon the slot
    //Does item exchanging, equipping and stuff like that
    public override void ItemDrop(ItemInstance droppedItem)
    {
        if (droppedItem == null)
        {
            return;
        }
        else if(item != null)
        {
            item.SetItemAttached();
        }

        LinkItemAndSlot(droppedItem, this);
    }

    //Link the item to a slot
    public override void LinkItemAndSlot(ItemInstance item, Slot<Item> slot)
    {
        item.slot = slot;
        item.transform.SetParent(slot.transform);
        slot.item = item;
        item.ItemToParentSlot();
    }

}

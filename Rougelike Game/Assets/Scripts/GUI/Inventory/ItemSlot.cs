using UnityEngine.EventSystems;
using UnityEngine;

//The item slot that items will go into
//Responsbile for handling when the item is dropped on the item slot
public class ItemSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public int equipmentSlot;
    public int id;
    [HideInInspector]
    public ItemInstance item;

    PlayerStats playerStats;

    void Start()
    {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

    //When the player drops and item from dragging
    public void OnDrop(PointerEventData eventData)
    {
        ItemInstance droppedItem = eventData.pointerDrag.GetComponent<ItemInstance>();
        ItemDrop(droppedItem);
    }

    //When the player drops an item from clicking
    public void OnPointerClick(PointerEventData eventData)
    {
        ItemInstance droppedItem = StaticCanvasList.instance.inventoryManager.attachedItem;
        //If there is an item attached to the mouse pointer
        if (droppedItem)
        {
            //Disconnnect the item from the mouse
            droppedItem.attached = false;
            StaticCanvasList.instance.inventoryManager.attachedItem = null;
            //if there is an item in the slot, swap the items being dragged
            if(item != null)
            {
                //Attach the item from this slot
                item.SetItemAttached();
                //Crucial to make the ItemDrop() work to exchange item correctly
                item = null;
            }
            ItemDrop(droppedItem);
        }
    }

    //Handles when an item is dropped upon the slot
    //Does item exchanging, equipping and stuff like that
    //Basically, its just a lot of edge cases
    public void ItemDrop(ItemInstance droppedItem)
    {
        if (droppedItem == null)
        {
            return;
        }
        //If this slot is an equipment slot, then we have to handle equipping
        else if (equipmentSlot >= 0)
        {
            //If the item can be equipped in that slot
            if (droppedItem.item.EquippedSlot == id)
            {
                //Attempt to equip the item. EquipItem() will return true if the item is equippable
                if (playerStats.EquipItem(droppedItem))
                {
                    //If the previous slot was empty, just add a new item to ti
                    if (item == null)
                    {
                        //Update which slot contains the item
                        droppedItem.slot.item = null;
                        LinkItemAndSlot(droppedItem, this);
                    }
                    else
                    {
                        ExchangeItems(droppedItem);
                    }
                }
            }
        }
        //Otherwise, it this an empty slot?
        else if (item == null)
        {
            //If the item was from an equipped slot, unequip the item
            if (droppedItem.equipped)
            {
                playerStats.UnequipItem(droppedItem);
            }
            //Keeps an item from setting its previous slot to null
            //If the item has been exchanged with something else
            if (droppedItem.slot.item == droppedItem)
            {
                droppedItem.slot.item = null;
            }
            LinkItemAndSlot(droppedItem, this);
        }
        //Handles the exchange between two items
        else if(droppedItem.attached)
        {
            //If the dragged item is from an equipment slot
            //If the item dropped upon can be equipped, equip it.
            if (droppedItem.equipped && playerStats.EquipItem(item))
            {
                ExchangeItems(droppedItem);
            }
            //If the dropped item is just from a regular slot, exchange items
            else if (!droppedItem.equipped)
            {
                ExchangeItems(droppedItem);
            }
        }

        droppedItem.ItemToParentSlot();
    }

    //Switches the dropped item to this slot and this slot's item to the dropped item's slot
    public void ExchangeItems(ItemInstance droppedItem)
    {
        LinkItemAndSlot(item, droppedItem.slot);
        LinkItemAndSlot(droppedItem, this);
    }

    //Link the item to a slot
    public void LinkItemAndSlot(ItemInstance item, ItemSlot slot)
    {
        item.slot = slot;
        item.transform.SetParent(slot.transform);
        slot.item = item;
        item.ItemToParentSlot();
    }

}

using UnityEngine.EventSystems;
using UnityEngine;

//The item slot that items will go into
//Responsbile for handling when the item is dropped on the item slot
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemInstance item = null;

    //When the player drops an item from clicking
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        ItemInstance droppedItem = StaticCanvasList.instance.inventoryManager.attachedItem;

        ItemDropIntoEmpty(droppedItem);
    }

    //Handles when an item is dropped upon the slot
    //Does item exchanging, equipping and stuff like that
    public virtual void ItemDropIntoEmpty(ItemInstance droppedItem)
    {
        //If there is an item attached to the mouse pointer
        if (droppedItem && item == null)
        {
            //Disconnnect the item from the mouse
            droppedItem.attached = false;
            LinkItemAndSlot(droppedItem, this);
            StaticCanvasList.instance.inventoryManager.attachedItem = null;
        }
    }

    //When the item is dropped into a slot that is already full
    public virtual void ItemDropIntoFull(ItemInstance droppedItem)
    {
        //If there is an item attached to the mouse pointer
        if (droppedItem && item != null)
        {
            //Disconnnect the item from the mouse
            droppedItem.attached = false;
            item.PickItemUp();
            LinkItemAndSlot(droppedItem, this);
        }
    }

    //Link the item to a slot
    public void LinkItemAndSlot(ItemInstance item, ItemSlot slot)
    {
        item.slot = slot;
        item.transform.SetParent(slot.transform);
        slot.item = item;
        item.ItemToParentSlot();
        item.transform.localScale = new Vector3(1, 1, 1);
        item.transform.localPosition = Vector2.zero;
    }

    public ItemInstance GetItem()
    {
        return item;
    }

    public virtual void SetItem(ItemInstance item)
    {
        this.item = item;
    }

}

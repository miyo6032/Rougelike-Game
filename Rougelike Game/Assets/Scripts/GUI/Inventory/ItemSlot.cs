using UnityEngine.EventSystems;
using UnityEngine;

//The item slot that items will go into
//Responsbile for handling when the item is dropped on the item slot
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public ItemInstance item;

    protected PlayerStats playerStat;

    void Start()
    {
        playerStat = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

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
    }

}

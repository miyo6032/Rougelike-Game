using UnityEngine.EventSystems;
using UnityEngine;

/// <summary>
/// The item slot in the inventory that items will go into
/// </summary>
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemInstance item;

    /// <summary>
    /// When the player drops an item from clicking
    /// </summary>
    /// <param name="eventData"></param>
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        ItemInstance droppedItem = StaticCanvasList.instance.inventoryManager.attachedItem;

        if (item == null)
        {
            ItemDropIntoEmpty(droppedItem);
            StaticCanvasList.instance.inventoryManager.attachedItem = null;
        }
    }

    /// <summary>
    /// Handles when an item is dropped upon the slot
    /// </summary>
    /// <param name="droppedItem"></param>
    public virtual void ItemDropIntoEmpty(ItemInstance droppedItem)
    {
        // If there is an item attached to the mouse pointer
        if (droppedItem && item == null)
        {
            // Disconnnect the item from the mouse
            droppedItem.attached = false;
            LinkItem(droppedItem);
        }
    }

    /// <summary>
    /// When the item is dropped into a slot that is already full
    /// </summary>
    /// <param name="droppedItem"></param>
    public virtual void ItemDropIntoFull(ItemInstance droppedItem)
    {
        // If there is an item attached to the mouse pointer
        if (droppedItem && item != null)
        {
            if(droppedItem.item == item.item)
            {
                item.ChangeAmount(droppedItem.GetAmount());
                SetItem(item);
                Destroy(droppedItem.gameObject);
            }
            else
            {
                item.PickItemUp();
                LinkItem(droppedItem);
            }
            // Disconnnect the item from the mouse
            droppedItem.attached = false;
        }
    }

    /// <summary>
    /// Link and item to a slot
    /// </summary>
    /// <param name="itemInstance"></param>
    public void LinkItem(ItemInstance itemInstance)
    {
        itemInstance.slot = this;
        itemInstance.transform.SetParent(transform);
        SetItem(itemInstance);
        itemInstance.ItemToParentSlot();
        itemInstance.transform.localScale = new Vector3(1, 1, 1);
        itemInstance.transform.localPosition = Vector2.zero;
    }

    public ItemInstance GetItem()
    {
        return item;
    }

    public virtual void SetItem(ItemInstance itemInstance)
    {
        item = itemInstance;
    }

}

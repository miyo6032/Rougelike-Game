using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The item slot in the inventory that items will go into
/// </summary>
public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public ItemStack itemStack {get; private set;}
    public Text stackText;
    public Image itemSprite;

    /// <summary>
    /// Handles item manipulation by the mouse pointer
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        ItemStack attachedItem = ItemDragger.instance.itemStack;

        if (itemStack == null && attachedItem != null)
        {
            ItemDropIntoEmpty(attachedItem);
        }
        else if (itemStack != null && attachedItem == null)
        {
            PickItemUp();
        }
        else if (itemStack != null && attachedItem != null)
        {
            ItemDropIntoFull(attachedItem);
        }

        if (itemStack != null)
        {
            Tooltip.instance.ShowItemTooltip(itemStack.item);
        }
    }

    /// <summary>
    /// Pick the current item up
    /// </summary>
    public virtual void PickItemUp()
    {
        ItemDragger.instance.SetItem(itemStack);
        RemoveItem();
    }

    /// <summary>
    /// Set the previously empty slot to a new item
    /// </summary>
    /// <param name="droppedItem"></param>
    public virtual void ItemDropIntoEmpty(ItemStack droppedItem)
    {
        SetItem(droppedItem);
        ItemDragger.instance.RemoveItem();
    }

    /// <summary>
    /// Exchange items with the item dragger, or if the items are the same, increase the stack amount
    /// </summary>
    public virtual void ItemDropIntoFull(ItemStack droppedItem)
    {
        if (droppedItem.item == itemStack.item && droppedItem.item.GetIsStackable())
        {
            ChangeAmount(droppedItem.amount);
            SetItem(itemStack);
            ItemDragger.instance.RemoveItem();
        }
        else
        {
            PickItemUp();
            SetItem(droppedItem);
        }
    }

    /// <summary>
    /// When the slot recieves a new item
    /// </summary>
    public virtual void SetItem(ItemStack stack)
    {
        itemStack = stack;
        stackText.text = stack.amount == 1 ? "" : stack.amount.ToString();
        itemSprite.sprite = stack.item.GetSprite();
    }

    /// <summary>
    /// When the slot removes the item
    /// </summary>
    public virtual void RemoveItem()
    {
        itemStack = null;
        stackText.text = "";
        itemSprite.sprite = InventoryManager.instance.invisible.GetSprite();
    }

    /// <summary>
    /// Show the tooltip
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemStack != null)
        {
            Tooltip.instance.ShowItemTooltip(itemStack.item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.instance.gameObject.SetActive(false);
    }

    /// <summary>
    /// Change the amount of an itemstack
    /// </summary>
    public virtual void ChangeAmount(int i)
    {
        itemStack.amount += i;
        if (itemStack.amount > 1)
        {
            stackText.text = itemStack.amount.ToString();
        }
        else if (itemStack.amount == 1)
        {
            stackText.text = "";
        }
        else
        {
            RemoveItem();
        }
    }
}
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The item slot in the inventory that items will go into
/// </summary>
public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public ItemStack itemStack;
    public Text stackText;
    public Image itemSprite;

    /// <summary>
    /// When the player drops an item from clicking
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        ItemStack attachedItem = StaticCanvasList.instance.itemDragger.itemStack;

        if (itemStack == null && attachedItem != null)
        {
            ItemDropIntoEmpty(attachedItem);
        }
        else if(itemStack != null && attachedItem == null)
        {
            PickItemUp();
        }
        else if(itemStack != null && attachedItem != null)
        {
            ItemDropIntoFull(attachedItem);
        }
    }

    public virtual void PickItemUp()
    {
        StaticCanvasList.instance.itemDragger.SetItem(itemStack);
        RemoveItem();
    }

    /// <summary>
    /// Handles when an item is dropped upon the slot
    /// </summary>
    /// <param name="droppedItem"></param>
    public virtual void ItemDropIntoEmpty(ItemStack droppedItem)
    {
        SetItem(droppedItem);
        StaticCanvasList.instance.itemDragger.RemoveItem();
    }

    /// <summary>
    /// When the item is dropped into a slot that is already full
    /// </summary>
    /// <param name="droppedItem"></param>
    public virtual void ItemDropIntoFull(ItemStack droppedItem)
    {
        if(droppedItem.item == itemStack.item && droppedItem.item.Stackable)
        {
            ChangeAmount(droppedItem.amount);
            SetItem(itemStack);
            StaticCanvasList.instance.itemDragger.RemoveItem();
        }
        else
        {
            PickItemUp();
            SetItem(droppedItem);
        }
    }

    public virtual void SetItem(ItemStack stack)
    {
        itemStack = stack;
        stackText.text = stack.amount == 1 ? "" : stack.amount.ToString();
        itemSprite.sprite = StaticCanvasList.instance.textureDatabase.LoadTexture(stack.item.Sprite);
    }

    public virtual void RemoveItem()
    {
        itemStack = null;
        stackText.text = "";
        itemSprite.sprite = StaticCanvasList.instance.textureDatabase.LoadTexture("Invisible");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemStack != null)
        {
            StaticCanvasList.instance.inventoryTooltip.ShowItemTooltip(itemStack.item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StaticCanvasList.instance.inventoryTooltip.gameObject.SetActive(false);
    }

    /// <summary>
    /// Change the amount of an item
    /// </summary>
    /// <param name="i"></param>
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

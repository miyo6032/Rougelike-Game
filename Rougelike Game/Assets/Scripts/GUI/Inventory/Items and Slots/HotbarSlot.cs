using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
///Used for the hotbar slot in the hotbar - the hotbar detector in the inventory is in InventoryHotbarSlot 
/// </summary>
public class HotbarSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemSlot itemSlot;
    public Image itemSprite;
    public Text stackAmount;
    public ItemUse itemUse;

    /// <summary>
    /// Re render the items - amount and sprites
    /// </summary>
    /// <param name="item"></param>
    public void UpdateItem()
    {
        if (itemSlot.itemStack != null)
        {
            itemSprite.sprite = StaticCanvasList.instance.textureDatabase.LoadTexture(itemSlot.itemStack.item.Sprite);
            //Only show number amount if item amount is not 1
            stackAmount.text = (itemSlot.itemStack.amount == 1) ? "" : itemSlot.itemStack.amount.ToString();
        }
        else
        {
            stackAmount.text = "";
            itemSprite.sprite = StaticCanvasList.instance.textureDatabase.LoadTexture("Invisible");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UseItem();
    }

    /// <summary>
    /// Apply the item's value if it exists
    /// </summary>
    public void UseItem()
    {
        if (itemSlot.itemStack != null)
        {
            itemUse.ApplyItemEffect(itemSlot);
            UpdateItem();
        }
    }
}

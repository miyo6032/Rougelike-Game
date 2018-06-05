using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
///Used for the hotbar slot in the hotbar - the hotbar detector in the inventory is in InventoryHotbarSlot 
/// </summary>
public class HotbarSlot : MonoBehaviour, IPointerClickHandler
{
    private ItemInstance itemInstance;
    public Image[] itemSprites;
    public Text stackAmount;
    public ItemUse itemUse;

    /// <summary>
    /// Re render the items - amount and sprites
    /// </summary>
    /// <param name="item"></param>
    public void UpdateItem(ItemInstance item)
    {
        itemInstance = item;
        int i = 0;
        if (item != null)
        {
            for (; i < itemInstance.item.Sprites.Length; i++)
            {
                itemSprites[i].sprite =
                    StaticCanvasList.instance.textureDatabase.LoadTexture(itemInstance.item.Sprites[i]);
            }

            //Only show number amount if item amount is not 1
            stackAmount.text = (item.GetAmount() == 1) ? "" : item.GetAmount().ToString();
        }

        //Set the rest of the item images to invisible
        for (; i < 3; i++)
        {
            itemSprites[i].sprite = StaticCanvasList.instance.textureDatabase.LoadTexture("Invisible");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UseItem();
    }

    /// <summary>
    /// Apply the item's effect if it exists
    /// </summary>
    public void UseItem()
    {
        if (itemInstance != null)
        {
            itemUse.ApplyItemEffect(itemInstance);
            UpdateItem(itemInstance);
        }
    }
}

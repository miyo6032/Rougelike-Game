using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for item dragging whenever an item is picked up
/// </summary>
public class ItemDragger : MonoBehaviour {
    [HideInInspector]
    public ItemStack itemStack;
    public Text stackText;
    public Image itemSprite;

    /// <summary>
    /// Set the item dragger to be dragging this item
    /// </summary>
    public void SetItem(ItemStack item)
    {
        itemStack = item;
        stackText.text = itemStack.amount == 1 ? "" : itemStack.amount.ToString();
        itemSprite.sprite = StaticCanvasList.instance.textureDatabase.LoadTexture(itemStack.item.Sprite);
    }

    /// <summary>
    /// Clear the item dragger from any item
    /// </summary>
    public void RemoveItem()
    {
        itemStack = null;
        stackText.text = "";
        itemSprite.sprite = StaticCanvasList.instance.textureDatabase.LoadTexture("Invisible");
    }

    private void Update()
    {
        // Always follow the mouse
        transform.position = Input.mousePosition;
    }
}

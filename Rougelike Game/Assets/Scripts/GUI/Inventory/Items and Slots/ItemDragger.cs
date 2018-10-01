using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for item dragging whenever an item is picked up
/// </summary>
public class ItemDragger : MonoBehaviour
{
    public static ItemDragger instance;
    
    public ItemStack itemStack;

    public Text stackText;
    public Image itemSprite;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Duplicate " + this.GetType().Name);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set the item dragger to be dragging this item
    /// </summary>
    public void SetItem(ItemStack itemStack)
    {
        this.itemStack = itemStack;
        stackText.text = itemStack.amount == 1 ? "" : itemStack.amount.ToString();
        itemSprite.sprite = itemStack.item.GetSprite();
    }

    /// <summary>
    /// Clear the item dragger from any item
    /// </summary>
    public void RemoveItem()
    {
        itemStack = null;
        stackText.text = "";
        itemSprite.sprite = InventoryManager.instance.invisible.GetSprite();
    }

    private void Update()
    {
        // Always follow the mouse
        transform.position = Input.mousePosition;
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents the item game object in the inventory
/// </summary>
public class ItemInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // The item's data and stats are stored in this
    [HideInInspector] public Item item;

    // Points to the item's slot
    [HideInInspector] public ItemSlot slot;

    // Whether or not this item is equipped
    [HideInInspector] public bool equipped;
    public Text stackText;

    // The stack (amount of items)
    private int amount;
    private PlayerStats playerStat;
    private InventoryManager inventory;

    // Whether the item is currently attached to the mouse pointer
    [HideInInspector] public bool attached;
    public Image[] itemSprites;

    void Start()
    {
        playerStat = GameObject.Find("Player").GetComponent<PlayerStats>();
        inventory = StaticCanvasList.instance.inventoryManager;
    }

    /// <summary>
    /// When a new item is created, fill its stats and load textures
    /// </summary>
    /// <param name="Item"></param>
    public void Initialize(Item Item)
    {
        name = Item.Title;
        item = Item;
        amount = 1;

        // Load sprites
        for (int i = 0; i < item.Sprites.Length; i++)
        {
            itemSprites[i].sprite = StaticCanvasList.instance.textureDatabase.LoadTexture(item.Sprites[i]);
        }
    }

    /// <summary>
    /// Pick up a new item, or exchange it with the current one
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ItemInstance droppedItem = inventory.attachedItem;
            if (droppedItem)
            {
                slot.ItemDropIntoFull(droppedItem);
            }
            else
            {
                PickItemUp();
            }
        }
    }

    public void PickItemUp()
    {
        SetItemAttached();
        if (equipped)
        {
            playerStat.UnequipItem(this);
        }

        slot = null;
    }

    public void DestroyItem()
    {
        if (slot != null)
        {
            slot.SetItem(null);
        }

        Destroy(gameObject);
    }

    // Called by slot when items are exchanged in dragging
    /// <summary>
    /// Set the item to be dragged automatically by the mouse
    /// </summary>
    void SetItemAttached()
    {
        attached = true;
        inventory.attachedItem = this;
        transform.SetParent(inventory.transform.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        slot.SetItem(null);
    }

    /// <summary>
    /// Set the item back from begin dragged to its parent
    /// </summary>
    public void ItemToParentSlot()
    {
        transform.SetParent(slot.transform);
        transform.position = slot.transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        slot.SetItem(this);
    }

    /// <summary>
    /// Let the item be "attached" to the mouse - follow its position
    /// </summary>
    void Update()
    {
        if (attached)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StaticCanvasList.instance.inventoryTooltip.ShowItemTooltip(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StaticCanvasList.instance.inventoryTooltip.gameObject.SetActive(false);
    }

    /// <summary>
    /// Change the amount of an item
    /// </summary>
    /// <param name="i"></param>
    public void ChangeAmount(int i)
    {
        amount += i;
        if (amount > 1)
        {
            stackText.text = amount.ToString();
        }
        else if (amount == 1)
        {
            stackText.text = "";
        }
        else
        {
            DestroyItem();
        }
    }

    public int GetAmount()
    {
        return amount;
    }
}

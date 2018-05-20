using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Represent the item game object in the inventory
//Works closely with the item slot class to provide item dragging functionality
public class ItemInstance : MonoBehaviour,
IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [HideInInspector]
    public Item item; //The item's data and stats are stored in this
    [HideInInspector]
    public ItemSlot slot; //Points to the item's slot
    [HideInInspector]
    public int amount; //The stack (amount of items)
    [HideInInspector]
    public bool equipped; //Whether or not this item is equipped

    private PlayerStats playerStat;

    //Whether the item is currently attached to the mouse pointer
    [HideInInspector]
    public bool attached = false;

    public Image[] itemSprites;

    void Start()
    {
        playerStat = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

    //Initialize a new item - used when the inventory adds a new item
    public void Initialize(Item Item, ItemSlot Slot)
    {
        name = Item.Title;
        item = Item;
        amount = 1;
        slot = Slot;
        transform.SetParent(slot.transform);
        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = Vector2.zero;

        //Load sprites
        for(int i = 0; i < item.Sprites.Length; i++)
        {
            itemSprites[i].sprite = Resources.Load<Sprite>("ItemIcons/ItemParts/" + item.Sprites[i]);
        }
    }

    //Pick up a new item, or exchange it with the current one
    public void OnPointerDown(PointerEventData eventData)
    {
        ItemInstance droppedItem = StaticCanvasList.instance.inventoryManager.attachedItem;
        if (droppedItem)
        {
            slot.ItemDropIntoFull(droppedItem);
        }
        else
        {
            PickItemUp();
        }
    }

    public void PickItemUp()
    {
        SetItemAttached();
        if (equipped)
        {
            playerStat.UnequipItem(this);
        }
    }

    //Set the item to be dragged automatically by the mouse
    //Called by slot when items are exchanged in dragging
    void SetItemAttached()
    {
        attached = true;
        StaticCanvasList.instance.inventoryManager.attachedItem = this;
        transform.SetParent(StaticCanvasList.instance.inventoryManager.transform);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        slot.item = null;
    }

    //Set the item back from begin dragged to its rightful parent
    public void ItemToParentSlot()
    {
        transform.SetParent(slot.transform);
        transform.position = slot.transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        slot.item = this;
    }

    //Let the item be "attached" to the mouse - follow its position
    void Update()
    {
        if (attached)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StaticCanvasList.instance.inventoryTooltip.ShowTooltip(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StaticCanvasList.instance.inventoryTooltip.gameObject.SetActive(false);
    }
}

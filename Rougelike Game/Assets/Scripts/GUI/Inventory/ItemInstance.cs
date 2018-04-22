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

    private ShowTooltip tooltip;
    private PlayerStats playerStat;
    private Player player;

    //Whether the item is currently attached to the mouse pointer
    [HideInInspector]
    public bool attached = false;

    void Start()
    {
        playerStat = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

    //Initialize a new item - used when the inventory adds a new item
    public void Initialize(Item i, ItemSlot s)
    {
        item = i;
        amount = 1;
        slot = s;
        transform.SetParent(slot.transform);
        GetComponent<Image>().sprite = i.Sprite;
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ItemInstance droppedItem = StaticCanvasList.instance.inventoryManager.attachedItem;
        //If the player was already carrying an item with them
        if (droppedItem)
        {
            //If this item is equipped, we know it is in an equipped slot, so 
            //We have to call item drop first and see if we can equip the dropped item
            if (equipped)
            {
                slot.ItemDrop(droppedItem);
            }
            //Otherwise, just do the normal operation
            else
            {
                //Disconnect the item from the mouse
                droppedItem.attached = false;
                SetItemAttached();
                slot.ItemDrop(droppedItem);
            }
        }
        //If there is no item attached, just pick up the item with the mouse
        else
        {
            if (equipped)
            {
                playerStat.UnequipItem(this);
            }
            SetItemAttached();
        }
    }

    //Set the item to be dragged automatically by the mouse
    //Called by slot when items are exchanged in dragging
    public void SetItemAttached()
    {
        attached = true;
        StaticCanvasList.instance.inventoryManager.attachedItem = this;
        transform.SetParent(transform.parent.parent.parent);
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

    void Update()
    {
        if (attached)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //tooltip.Activate(item, this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //tooltip.Deactivate();
    }
}

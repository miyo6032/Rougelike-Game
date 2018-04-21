using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Represent the item game object in the inventory
//Works closely with the item slot class to provide item dragging functionality
public class ItemInstance : MonoBehaviour,
IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler,
IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [HideInInspector]
    public Item item;
    public ItemSlot slot;
    public int amount;
    public bool equipped;

    private ShowTooltip tooltip;
    private PlayerStats playerStat;
    private Player player;

    //Whether the item is currently attached to the mouse pointer
    [HideInInspector]
    public bool attached = false;

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

    //Clicking "picks up" the item to the player doesn't have to drag
    public void OnPointerClick(PointerEventData eventData)
    {
        //If there is an item attached to the mouse pointer
        ItemInstance droppedItem = StaticCanvasList.instance.inventoryManager.attachedItem;
        if (droppedItem)
        {
            //Disconnect the item from the mouse
            droppedItem.attached = false;
            //Attach this item to the mouse
            SetItemAttached();
            slot.ItemDrop(droppedItem);
        }
        //If there is no item attached, just pick up the item with the mouse
        else
        {
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

    //When the player begins to drag, snap it to the center of the pointer - makes it look nice
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(transform.parent.parent.parent);
        transform.position = eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        slot.item = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    //Makes the item move when the player first clicks on it
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    //Set the position back to the parent slot
    public void OnPointerUp(PointerEventData eventData)
    {
        ItemToParentSlot();
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

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Represent the item game object in the inventory
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
        SetItemAttached();
    }

    //Set the item to be dragged automatically by the mouse
    //Called by slot when items are exchanged in dragging
    public void SetItemAttached()
    {
        attached = true;
        StaticCanvasList.instance.inventoryManager.attachedItem = this;
        transform.SetParent(transform.parent.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    //Set the item back from begin dragged to its rightful parent
    public void ItemToParentSlot()
    {
        transform.SetParent(slot.transform);
        transform.position = slot.transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    //When the player begins to drag, snap it to the center of the pointer - makes it look nice
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(transform.parent.parent.parent);
        transform.position = eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

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

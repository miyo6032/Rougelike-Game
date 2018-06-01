﻿using UnityEngine;
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
    public bool equipped; //Whether or not this item is equipped

    public Text stackText;

    private int amount; //The stack (amount of items)

    private PlayerStats playerStat;

    private InventoryManager inventory;

    //Whether the item is currently attached to the mouse pointer
    [HideInInspector]
    public bool attached = false;

    public Image[] itemSprites;

    void Start()
    {
        playerStat = GameObject.Find("Player").GetComponent<PlayerStats>();
        inventory = StaticCanvasList.instance.inventoryManager;
    }

    public void Initialize(Item Item)
    {
        name = Item.Title;
        item = Item;
        amount = 1;

        //Load sprites
        for (int i = 0; i < item.Sprites.Length; i++)
        {
            itemSprites[i].sprite = StaticCanvasList.instance.textureDatabase.LoadTexture(item.Sprites[i]);
        }
    }

    //Pick up a new item, or exchange it with the current one
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
        if(slot != null)
        {
            slot.SetItem(null);
        }
        Destroy(gameObject);
    }

    //Set the item to be dragged automatically by the mouse
    //Called by slot when items are exchanged in dragging
    void SetItemAttached()
    {
        attached = true;
        inventory.attachedItem = this;
        transform.SetParent(inventory.transform.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        slot.SetItem(null);
    }

    //Set the item back from begin dragged to its rightful parent
    public void ItemToParentSlot()
    {
        transform.SetParent(slot.transform);
        transform.position = slot.transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        slot.SetItem(this);
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

    public void ChangeAmount(int i)
    {
        amount += i;
        if (amount > 1)
        {
            stackText.text = amount.ToString();
        }
        else if(amount == 1)
        {
            stackText.text = "";
        }
        else
        {
            //Destroy Item
        }
    }

    public int GetAmount()
    {
        return amount;
    }

    public void UseItem()
    {

        ChangeAmount(-1);

        if (amount == 0)
        {
            DestroyItem();
        }
    }
}

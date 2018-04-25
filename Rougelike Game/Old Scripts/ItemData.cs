using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemData : MonoBehaviour,
IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler,
IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

    public Item item;
    public int amount;
    public int slot;
    public bool equipped;
    public int equipNumber = -1;

    private Inventory inv;
    private ShowTooltip tooltip;
    private PlayerStat playerStat;
    private Player player;

    void Start()
    {
        inv = GameObject.Find("ItemDatabase").GetComponent<Inventory>();
        playerStat = GameObject.Find("Player").GetComponent<PlayerStat>();
        player = playerStat.gameObject.GetComponent<Player>();
        tooltip = inv.GetComponent<ShowTooltip>();
        this.transform.SetParent(inv.slots[slot].transform);
        this.transform.position = inv.slots[slot].transform.position;
        transform.localScale = new Vector3(1, 1, 1);

    }

    public bool EquipItem()
    {

        if (playerStat.level >= item.ItemLevel && item.Id != playerStat.equippedItems[item.EquippedSlot].Id)
        {//Equipps a new item
            equipped = true;
            Item prevItem = playerStat.equippedItems[item.EquippedSlot];
            if (prevItem.Id != -1)
            {
                inv.unequipItemType(prevItem.Id);
            }
            transform.GetComponentInChildren<Text>().text = "E";
            playerStat.equippedItems[item.EquippedSlot] = item;
            playerStat.UpdateGearStats();
            if (item.Skill != 0)
            {
                inv.AddItem(item.Skill);
            }
            return true;
        }
        else
        {
            return false;
        }

    }

    public void unequipItem()
    {
        if (item.Id > -1)
        {
            if (playerStat.equippedItems[item.EquippedSlot] == item)
            {//Unequips the item
                if (item.Skill != 0)
                {

                    ItemData itemd = inv.GetItemdata(item.Skill);
                    if (itemd != null)
                    {
                        inv.items[itemd.slot] = new Item();
                        GameObject.Destroy(itemd.gameObject);

                        if (player.currentEquippedSkill() == item.Skill)
                        {
                            player.unequipSkill();
                        }

                    }

                }
                equipped = false;
                transform.GetComponentInChildren<Text>().text = "";
                playerStat.equippedItems[item.EquippedSlot] = new Item();
                playerStat.UpdateGearStats();

            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inv.ChestDrawn())//One click chest exchange
        {
            if (item.Id > -1)
            {
                if (slot > 37 && slot <= 54)
                {
                    for (int i = 8; i < 24; i++)
                    {
                        if (inv.slots[i].transform.childCount == 0)
                        {
                            moveItemInSlot(i);
                            return;
                        }
                    }
                }
                else if (slot <= 23 && slot >= 8)
                {
                    for (int i = 38; i <= 54; i++)
                    {
                        if (inv.slots[i].transform.childCount == 0)
                        {
                            moveItemInSlot(i);
                            return;
                        }
                    }
                }
            }
        }
        else if (inv.LootDrawn())//One click loot bag exchange
        {
            if (item.Id > -1)
            {
                if (slot > 33 && slot <= 37)
                {
                    for (int i = 8; i < 24; i++)
                    {
                        if (inv.slots[i].transform.childCount == 0)
                        {
                            moveItemInSlot(i);
                            return;
                        }
                    }
                }
                else if (slot <= 23 && slot >= 8)
                {
                    for (int i = 34; i <= 37; i++)
                    {
                        if (inv.slots[i].transform.childCount == 0)
                        {
                            moveItemInSlot(i);
                            return;
                        }
                    }
                }
            }
        }
        else if (inv.buySellMode())//One click buysell exchange
        {
            if (item.Id > -1)
            {
                if (slot == 32)
                {
                    for (int i = 8; i < 24; i++)
                    {
                        if (inv.slots[i].transform.childCount == 0)
                        {

                            moveItemInSlot(i);
                            return;
                        }
                    }
                }
                else if (slot <= 23 && slot >= 8)
                {
                    if (inv.slots[32].transform.childCount == 0)
                    {
                        inv.slots[32].GetComponent<SellSlot>().item = inv.slots[slot].GetComponentInChildren<ItemData>();
                        moveItemInSlot(32);

                    }
                }
            }
        }
        else
        {
            if (item.Id > -1)
            {
                if (!inv.itemIsAlreadyEquipped(item.Id))
                {
                    transform.SetParent(transform.parent.parent.parent);
                    transform.position = eventData.position;
                    GetComponent<CanvasGroup>().blocksRaycasts = false;
                    inv.slots[item.EquippedSlot].GetComponent<Slot>().EquipAndMoveItems(this);
                    transform.SetParent(inv.slots[slot].transform);
                    transform.position = inv.slots[slot].transform.position;
                    GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
                else if (slot < 8)
                {
                    transform.SetParent(transform.parent.parent.parent);
                    transform.position = eventData.position;
                    GetComponent<CanvasGroup>().blocksRaycasts = false;
                    int newSlot = inv.findEmptySlot();
                    if (newSlot != -1)
                    {
                        inv.slots[newSlot].GetComponent<Slot>().EquipAndMoveItems(this);
                    }
                    transform.SetParent(inv.slots[slot].transform);
                    transform.position = inv.slots[slot].transform.position;
                    GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
            }
        }
    }

    private void moveItemInSlot(int i)
    {
        inv.slots[i].GetComponent<Slot>().EquipAndMoveItems(this);
        transform.SetParent(inv.slots[slot].transform);
        transform.position = inv.slots[slot].transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.SetParent(transform.parent.parent.parent);
            transform.position = eventData.position;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(inv.slots[slot].transform);
        transform.position = inv.slots[slot].transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.position = eventData.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (item != null)
        {
            transform.position = inv.slots[slot].transform.position;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(item, this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();
    }

}

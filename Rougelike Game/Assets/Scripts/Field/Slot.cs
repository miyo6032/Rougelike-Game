using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {

    public int id;
    public bool isSkillSlot = false;
    public bool isEquipSlot = false;
    private Inventory inv;

    void Start() {
        inv = GameObject.Find("ItemDatabase").GetComponent<Inventory>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        EquipAndMoveItems(droppedItem);
    }

    public void EquipAndMoveItems(ItemData droppedItem) { 
		if ((isSkillSlot && droppedItem.item.Id >= -1) ||
            (!isSkillSlot && droppedItem.item.Id < -1) || 
            (isEquipSlot && droppedItem.item.Id < -1) ||
            (transform.childCount > 0 && droppedItem.item.Id == inv.items[id].Id)
            ){
			return;
		}
        if(droppedItem.item.Id >= -1 && isEquipSlot)//If the item was dropped on an equipment slot
        {
            if (droppedItem.item.EquippedSlot == id)//If the item can be equipped in that slot
            {
                if (droppedItem.EquipItem())//If the item meets all equip needs
                {
                    if (inv.items[id].Id == -1)//Sets the previous slot to empty
                    {
                        inv.items[droppedItem.slot] = new Item();
                        inv.items[id] = droppedItem.item;
                        droppedItem.slot = id;
                    }
                    else
                    {
                        ExchangeItems(droppedItem);
                    }
                }
            }
        }
        //If this slot is empty
        else if (inv.items[id].Id == -1)
        {
            if (!inv.LootDrawn() && !inv.buySellMode())
            {
                droppedItem.unequipItem();
            }
            inv.items[droppedItem.slot] = new Item();
            inv.items[id] = droppedItem.item;
            droppedItem.slot = id;
        }
        else if(droppedItem.slot != id){
            ItemData item = transform.GetChild(0).GetComponent<ItemData>();
            if (droppedItem.slot < 8)
            {
                if (droppedItem.item.EquippedSlot == item.item.EquippedSlot)
                {
                    ExchangeItems(droppedItem);
                    if (!inv.LootDrawn() && !inv.buySellMode())
                    {
                        if (item.EquipItem())
                        {
                            droppedItem.unequipItem();
                        }
                        else
                        {
                            ExchangeItems(droppedItem);
                        }
                    }
                }
            }
            else
            {
                ExchangeItems(droppedItem);
            }
		}
	}

    private void ExchangeItems(ItemData droppedItem)
    {
        Transform item = this.transform.GetChild(0);
        item.GetComponent<ItemData>().slot = droppedItem.slot;
        item.transform.SetParent(inv.slots[droppedItem.slot].transform);
        item.transform.position = inv.slots[droppedItem.slot].transform.position;

        inv.items[droppedItem.slot] = item.GetComponent<ItemData>().item;
        inv.items[id] = droppedItem.item;

        droppedItem.slot = id;
        droppedItem.transform.SetParent(this.transform);
        droppedItem.transform.position = this.transform.position;
    }

}

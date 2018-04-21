using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellSlot : MonoBehaviour, IDropHandler
{
    private Inventory inv;
    private PlayerStat playerStat;

    public ItemData item;
    public Text goldText;

    void Start()
    {
        inv = GameObject.Find("ItemDatabase").GetComponent<Inventory>();
        playerStat = GameObject.Find("Player").GetComponent<PlayerStat>();
        item = gameObject.GetComponent<ItemData>();
    }

    public void Sell()
    {
        if (inv.items[32].Id == -1) return;

        if (item.amount > 0)
        {
            item.amount--;
            playerStat.UpdateGold(item.item.Value);
        }
        if(item.amount <= 0)
        {
            inv.items[item.slot] = new Item();
            Destroy(item.gameObject);
            item = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        item = droppedItem;
    }

    public void Update()
    {
        if (inv.items[32].Id != -1)
        {
           goldText.text = "Gold: " + item.item.Value;
        }
        else
        {
            goldText.text = "Gold: " + 0;
        }
    }

}

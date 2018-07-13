using System.Collections;
using UnityEngine;

public class ShopSlot : ItemSlot {

    public override void SetItem(ItemInstance itemInstance)
    {
        if(itemInstance == null)
        {
            Debug.Log("-gold");
        }
        base.SetItem(itemInstance);
    }

    public override void ItemDropIntoEmpty(ItemInstance droppedItem)
    {
        Debug.Log("+gold");
        base.ItemDropIntoEmpty(droppedItem);
    }

    public override void ItemDropIntoFull(ItemInstance droppedItem)
    {
        Debug.Log("-gold" + item.item.Title);
        Debug.Log("+gold" + droppedItem.item.Title);
        base.ItemDropIntoFull(droppedItem);
    }

}

using System.Collections;
using UnityEngine;

public class ShopSlot : ItemSlot {

    public override void SetItem(ItemStack itemInstance)
    {
        if(itemInstance == null)
        {
            Debug.Log("-gold");
        }
        base.SetItem(itemInstance);
    }

    public override void ItemDropIntoEmpty(ItemStack droppedItem)
    {
        Debug.Log("+gold");
        base.ItemDropIntoEmpty(droppedItem);
    }

    public override void ItemDropIntoFull(ItemStack droppedItem)
    {
        Debug.Log("-gold" + itemStack.item.Title);
        Debug.Log("+gold" + droppedItem.item.Title);
        base.ItemDropIntoFull(droppedItem);
    }

}

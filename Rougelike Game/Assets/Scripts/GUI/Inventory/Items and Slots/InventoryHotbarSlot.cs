//Detects when an item is placed in this slot, and updates the hotbar
public class InventoryHotbarSlot : ItemSlot {

    public HotbarSlot hotbarSlot;

    public override void SetItem(ItemInstance item)
    {
        base.SetItem(item);
        hotbarSlot.UpdateItem(item);
    }

}

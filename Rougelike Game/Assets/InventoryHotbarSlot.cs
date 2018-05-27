public class InventoryHotbarSlot : ItemSlot {

    public HotbarSlot hotbarSlot;

    public override void SetItem(ItemInstance item)
    {
        base.SetItem(item);
        hotbarSlot.UpdateItem(item);
    }

}

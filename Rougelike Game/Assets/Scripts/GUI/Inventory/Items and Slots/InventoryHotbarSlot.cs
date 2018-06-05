/// <summary>
/// Detects when an item is placed in this slot, and updates the hotbar
/// </summary>
public class InventoryHotbarSlot : ItemSlot
{
    public HotbarSlot hotbarSlot;

    /// <summary>
    /// Used to detect an item change
    /// </summary>
    /// <param name="itemInstance"></param>
    public override void SetItem(ItemInstance itemInstance)
    {
        base.SetItem(itemInstance);
        hotbarSlot.UpdateItem(itemInstance);
    }
}

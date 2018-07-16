/// <summary>
/// When item is placed, will destroy the item
/// </summary>
public class ItemDelete : ItemSlot
{
    /// <summary>
    /// When dropped, destroy that item
    /// </summary>
    /// <param name="droppedItem"></param>
    public override void ItemDropIntoEmpty(ItemStack droppedItem)
    {
        ItemDragger.instance.RemoveItem();
    }
}

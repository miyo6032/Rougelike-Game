/// <summary>
/// When item is placed, will destroy the item
/// </summary>
public class ItemDelete : ItemSlot
{
    /// <summary>
    /// When dropped, destroy that item
    /// </summary>
    /// <param name="droppedItem"></param>
    public override void ItemDropIntoEmpty(ItemInstance droppedItem)
    {
        //If there is an item attached to the mouse pointer
        if (droppedItem && item == null) droppedItem.DestroyItem();
    }
}

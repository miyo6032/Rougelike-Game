/// <summary>
/// Stores an item's constant data and the position it was in the slot
/// </summary>
public class ItemSave
{
    public Item item;
    public int slotPosition;

    public ItemSave(Item item, int slotPosition)
    {
        this.item = item;
        this.slotPosition = slotPosition;
    }
}

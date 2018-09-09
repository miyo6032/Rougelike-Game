/// <summary>
/// Stores an item's constant data and the position it was in the slot
/// </summary>
[System.Serializable]
public class ItemSave
{
    public ItemStack itemStack;
    public int slotPosition;

    public ItemSave(ItemStack item, int slotPosition)
    {
        this.itemStack = item;
        this.slotPosition = slotPosition;
    }
}
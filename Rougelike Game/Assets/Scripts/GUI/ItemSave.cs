/// <summary>
/// Stores an item's constant data and the position it was in the slot
/// </summary>
[System.Serializable]
public class ItemSave
{
    public ItemStack item;
    public int slotPosition;

    public ItemSave(ItemStack item, int slotPosition)
    {
        this.item = item;
        this.slotPosition = slotPosition;
    }
}

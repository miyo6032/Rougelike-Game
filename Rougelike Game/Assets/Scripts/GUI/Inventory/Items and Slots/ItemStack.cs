/// <summary>
/// An item instance in the world with the amount as well as the item information
/// </summary>
public class ItemStack
{
    public ItemScriptableObject item;
    public int amount;

    public ItemStack(ItemScriptableObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}
using UnityEngine;

/// <summary>
/// The item in the world that the player can move over to pick up into the inventory
/// </summary>
public class ItemPickup : PlayerEnterDetector
{
    private ItemStack itemStack;

    public void SetItem(ItemStack stack)
    {
        this.itemStack = stack;
        GetComponent<SpriteRenderer>().sprite = stack.item.GetSprite();
    }

    public override void PlayerEnter(Collider2D player)
    {
        if (InventoryManager.instance.AddItem(itemStack))
        {
            Destroy(gameObject);
        }
    }
}
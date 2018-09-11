using UnityEngine;

/// <summary>
/// The item in the world that the player can move over to pick up into the inventory
/// </summary>
public class ItemPickup : PlayerEnterDetector
{
    private ItemStack item;

    public void SetItem(ItemStack item)
    {
        this.item = item;
        GetComponent<SpriteRenderer>().sprite = TextureDatabase.instance.LoadTexture(item.item.Sprite);
    }

    public override void PlayerEnter(Collider2D player)
    {
        if (InventoryManager.instance.AddItem(item))
        {
            Destroy(gameObject);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class ItemDragger : MonoBehaviour {
    [HideInInspector]
    public ItemStack itemStack;
    public Text stackText;
    public Image itemSprite;

    public void SetItem(ItemStack item)
    {
        itemStack = item;
        stackText.text = itemStack.amount == 1 ? "" : itemStack.amount.ToString();
        itemSprite.sprite = StaticCanvasList.instance.textureDatabase.LoadTexture(itemStack.item.Sprite);
    }

    public void RemoveItem()
    {
        itemStack = null;
        stackText.text = "";
        itemSprite.sprite = StaticCanvasList.instance.textureDatabase.LoadTexture("Invisible");
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }
}

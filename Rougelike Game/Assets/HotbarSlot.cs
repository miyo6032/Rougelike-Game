using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HotbarSlot : MonoBehaviour, IPointerClickHandler{

    private ItemInstance itemInstance;

    public Image[] itemSprites;

    public void UpdateItem(ItemInstance item)
    {
        itemInstance = item;
        int i = 0;
        if (item != null)
        {
            for (; i < itemInstance.item.Sprites.Length; i++)
            {
                itemSprites[i].sprite = StaticCanvasList.instance.textureDatabase.LoadTexture(itemInstance.item.Sprites[i]);
            }
        }

        for(; i < 3; i++)
        {
            itemSprites[i].sprite = StaticCanvasList.instance.textureDatabase.LoadTexture("Invisible");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Use the item
        if (itemInstance != null)
        {
            itemInstance.ChangeAmount(-1);
        }
    }
}

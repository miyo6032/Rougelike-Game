using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows the tooltip for the shop items
/// </summary>
public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemScriptableObject item;
    public Image image;

    public void SetItemAndImage(ItemScriptableObject item, Sprite sprite)
    {
        this.item = item;
        image.sprite = sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            Tooltip.instance.ShowItemTooltip(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.instance.gameObject.SetActive(false);
    }
}
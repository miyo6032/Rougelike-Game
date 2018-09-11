using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows the tooltip for the shop items
/// </summary>
public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Item currentItem;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void SetItemAndImage(Item item, Sprite sprite)
    {
        currentItem = item;
        image.sprite = sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            Tooltip.instance.ShowItemTooltip(currentItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.instance.gameObject.SetActive(false);
    }
}
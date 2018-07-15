using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {

    public Text buyTitleText;
    public Text sellTitleText;
    public Image buyItemImage;
    public Image sellItemImage;
    public Slider amountSlider;
    public GameObject amountConfirm;
    public ItemSave itemToExchange;

    public int gold;
    public ItemSave[] buyItems;
    private int buyItemIndex;

    public void LoadNPCItems(ItemSave[] items)
    {
        if(buyItems != items)
        {
            buyItems = items;
            buyItemIndex = 0;
        }
    }

    public void SetSprites(Image spritesToSet, string spriteNames)
    {
        spritesToSet.sprite = StaticCanvasList.instance.textureDatabase.LoadTexture(spriteNames);
    }

    public void NextBuyItem()
    {
        buyItemIndex = buyItemIndex == buyItems.Length - 1 ? 0 : buyItemIndex + 1;
        ItemSave itemToShow = buyItems[buyItemIndex];
        SetSprites(buyItemImage, itemToShow.item.item.Sprite);
        buyTitleText.text = itemToShow.item.item.Title;
    }

    public void PrevBuyItem()
    {
        buyItemIndex = buyItemIndex == 0 ? buyItems.Length - 1 : buyItemIndex - 1;
        ItemSave itemToShow = buyItems[buyItemIndex];
        SetSprites(buyItemImage, itemToShow.item.item.Sprite);
        buyTitleText.text = itemToShow.item.item.Title;
    }

    public void NextSellItem()
    {

    }

    public void PrevSellItem()
    {

    }

    public void SellItem()
    {

    }

    public void BuyItem()
    {

    }

    public void AddGold(int amount)
    {
        gold += amount;
    }

    /// <summary>
    /// Will attempt to "buy" gold, and return a bool as to whether there is enough gold left
    /// </summary>
    public bool RemoveGold(int amount)
    {
        if(amount > gold)
        {
            return false;
        }
        gold -= amount;
        return true;
    }

}

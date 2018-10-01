using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Handles the shop ui and buy and selling items
/// </summary>
public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public Text NPCName;
    public Text NPCIntro;
    public Text buyTitleText;
    public Text sellTitleText;
    public Text sellStackAmount;
    public Text buyItemCost;
    public Text sellItemCost;
    public Text goldText;
    public Image traderPortrait;
    public ShopItem buyItem;
    public ShopItem sellItem;
    public GameObject noItemsToSell;

    public int gold;
    private NPCTrader currentTrader;
    private int buyItemIndex;
    private int sellItemIndex;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Duplicate " + this.GetType().Name);
            Destroy(gameObject);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Open the shop of a particular trader - showing the shop ui
    /// </summary>
    public void OpenShop(NPCTrader trader)
    {
        if (currentTrader != trader)
        {
            currentTrader = trader;
            NPCName.text = trader.introDialogue.name;
            NPCIntro.text = trader.introDialogue.sentence;
            traderPortrait.sprite = trader.introDialogue.portrait;
            goldText.text = gold.ToString();
            // Reset indices
            buyItemIndex = 1;
            PrevBuyItem();
            sellItemIndex = 1;
            PrevSellItem();
            gameObject.SetActive(true);
        }
    }

    public void CloseShop()
    {
        currentTrader = null;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Used by a button to go to the next buy item
    /// </summary>
    public void NextBuyItem()
    {
        UpdateBuyItem(IncreaseIndex);
    }

    /// <summary>
    /// Used by a button to go to the previous buy item
    /// </summary>
    public void PrevBuyItem()
    {
        UpdateBuyItem(DecreaseIndex);
    }

    /// <summary>
    /// Used by a button to go to the next sell item
    /// </summary>
    public void NextSellItem()
    {
        UpdateSellItem(IncreaseIndex);
    }

    /// <summary>
    /// Used by a button to go to the previous sell item
    /// </summary>
    public void PrevSellItem()
    {
        UpdateSellItem(DecreaseIndex);
    }

    /// <summary>
    /// Sell an item, reducing the amount from the inventory
    /// </summary>
    public void SellItem()
    {
        gold += InventoryManager.instance.slots[sellItemIndex].itemStack.item.GetValue();
        InventoryManager.instance.slots[sellItemIndex].ChangeAmount(-1);
        UpdateSellItems();
        goldText.text = gold.ToString();
    }

    /// <summary>
    /// Buy and item, or do nothing if there are not enough funds
    /// </summary>
    public void BuyItem()
    {
        ItemScriptableObject itemToBuy = currentTrader.itemsForSale[buyItemIndex];
        if (gold >= itemToBuy.GetValue())
        {
            InventoryManager.instance.AddItem(new ItemStack(itemToBuy, 1));
            UpdateSellItems();
            gold -= itemToBuy.GetValue();
            goldText.text = gold.ToString();
        }
    }

    /// <summary>
    /// Initiate the dialogue for the trader (even traders have something to say)
    /// </summary>
    public void OpenDialogue()
    {
        DialoguePanel.instance.StartDialogue(currentTrader.dialogue);
    }

    /// <summary>
    /// Change the item index and update the ui to show the current buy item.
    /// </summary>
    private void UpdateBuyItem(ChangeIndex changeIndex)
    {
        buyItemIndex = changeIndex(buyItemIndex, currentTrader.itemsForSale.Count);
        ItemScriptableObject itemToShow = currentTrader.itemsForSale[buyItemIndex];
        buyItem.SetItemAndImage(itemToShow, itemToShow.GetSprite());
        buyTitleText.text = itemToShow.GetTitle();
        buyItemCost.text = itemToShow.GetValue().ToString();
    }

    /// <summary>
    /// Change the item index and update the sell item to show the current sell item.
    /// </summary>
    private void UpdateSellItem(ChangeIndex changeIndex)
    {
        List<ItemSlot> slots = InventoryManager.instance.slots;
        // Search for the next slot that has an item, and show that item
        int counter = 0;
        do
        {
            sellItemIndex = changeIndex(sellItemIndex, slots.Count);
            counter++;
            if (counter > slots.Count)
            {
                noItemsToSell.SetActive(true);
                return;
            }
        } while (slots[sellItemIndex].itemStack == null);
        noItemsToSell.SetActive(false);
        ItemStack itemStack = slots[sellItemIndex].itemStack;
        sellItem.SetItemAndImage(itemStack.item, itemStack.item.GetSprite());
        sellTitleText.text = itemStack.item.GetTitle();
        sellStackAmount.text = itemStack.amount == 1 ? "" : itemStack.amount.ToString();
        sellItemCost.text = itemStack.item.GetValue().ToString();
    }

    private void UpdateSellItems()
    {
        sellItemIndex--;
        NextSellItem();
    }

    private delegate int ChangeIndex(int index, int count);

    private int IncreaseIndex(int index, int count)
    {
        return index >= count - 1 ? 0 : index + 1;
    }

    private int DecreaseIndex(int index, int count)
    {
        return index <= 0 ? count - 1 : index - 1;
    }
}
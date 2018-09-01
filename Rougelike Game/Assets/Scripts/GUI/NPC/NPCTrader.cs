using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the data for a trader NPC, activated on click
/// </summary>
public class NPCTrader : NPC
{
    public List<ItemScriptableObject> itemsForSale;
    public Dialogue introDialogue;

    public override void OnNPCClicked()
    {
        ShopManager.instance.OpenShop(this);
    }
}
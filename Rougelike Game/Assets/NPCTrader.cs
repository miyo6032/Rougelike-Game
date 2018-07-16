using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the data for a trader NPC, activated on click
/// </summary>
public class NPCTrader : NPC {

    public List<ItemScriptableObject> itemsForSale;
    [TextArea(2, 4)]
    public string introDialogue;

    public override void OnNPCClicked()
    {
        StaticCanvasList.instance.shopManager.OpenShop(this);
    }

}

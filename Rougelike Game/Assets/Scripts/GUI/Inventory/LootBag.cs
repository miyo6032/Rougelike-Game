using System.Collections.Generic;
using UnityEngine;

//Detects when the player enters and exits and activates the loot inventory
public class LootBag : MonoBehaviour
{

    public List<ItemSave> items = new List<ItemSave>();

    //When the player walks on top of the bag
    void OnTriggerEnter2D(Collider2D other)
    {
        StaticCanvasList.instance.lootInventory.LoadInventory(this);
        StaticCanvasList.instance.gameUI.ToggleLootPanel();
    }

    //When the player walks off the bag
    void OnTriggerExit2D(Collider2D other)
    {
        StaticCanvasList.instance.lootInventory.UnloadInventory();
        StaticCanvasList.instance.gameUI.ToggleLootPanel();
    }

}

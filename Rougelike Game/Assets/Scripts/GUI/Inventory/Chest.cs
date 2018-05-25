using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

    public List<ItemSave> chestItems = new List<ItemSave>();
    public int lootLevel;
    public int fullness;

    void Start()
    {
        List<Item> items = new List<Item>();
        for(int i = 0; i < fullness; i++)
        {
            items.Add(StaticCanvasList.instance.itemDatabase.GenerateItem(lootLevel, 0));
        }
    }

    public void AddItems(List<Item> itemDrops)
    {
        foreach (Item item in itemDrops)
        {
            if (chestItems.Count < 24)
            {
                chestItems.Add(new ItemSave(item, chestItems.Count));
            }
        }
    }

}

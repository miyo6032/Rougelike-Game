using System.Collections.Generic;
using UnityEngine;

//Holds the chest items, and the chest opening and closing sprites
public class Chest : MonoBehaviour {

    public List<ItemSave> chestItems = new List<ItemSave>();
    public int lootLevel;
    public int fullness;

    public Sprite chestOpen;
    public Sprite chestClosed;

    void Start()
    {
        GenerateItems();
    }

    public void SetOpenSprite()
    {
        GetComponent<SpriteRenderer>().sprite = chestOpen;
    }

    public void SetClosedSprite()
    {
        GetComponent<SpriteRenderer>().sprite = chestClosed;
    }

    void GenerateItems()
    {
        List<ItemSave> items = new List<ItemSave>();
        for (int i = 0; i < fullness; i++)
        {
            int itemType = Random.Range(0, 3);
            ItemSave item = new ItemSave(StaticCanvasList.instance.itemGenerator.GenerateItem(lootLevel, itemType), i);
            items.Add(item);
        }
        chestItems = items;
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

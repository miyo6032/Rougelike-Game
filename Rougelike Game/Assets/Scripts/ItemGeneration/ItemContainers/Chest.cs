using System.Collections.Generic;
using UnityEngine;

//Holds the chest items, and the chest opening and closing sprites
public class Chest : MonoBehaviour {

    public List<ItemSave> chestItems = new List<ItemSave>();
    public int lootLevel;
    public Vector2Int dropRange;

    public Sprite chestOpen;
    public Sprite chestClosed;

    void Start()
    {
        chestItems = StaticCanvasList.instance.itemDropGenerator.GenerateItemDrops(lootLevel, dropRange);
    }

    public void SetOpenSprite()
    {
        GetComponent<SpriteRenderer>().sprite = chestOpen;
    }

    public void SetClosedSprite()
    {
        GetComponent<SpriteRenderer>().sprite = chestClosed;
    }

    //Called when saving, because the player potentially changed the chest contents
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

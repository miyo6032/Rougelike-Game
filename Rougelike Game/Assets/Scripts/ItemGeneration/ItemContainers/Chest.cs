using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the chest items, and the chest opening and closing sprites
/// </summary>
public class Chest : MonoBehaviour
{
    public List<ItemSave> chestItems = new List<ItemSave>();
    public int lootLevel;
    public Vector2Int dropRange;

    public Sprite chestOpen;
    public Sprite chestClosed;

    private void Start()
    {
        chestItems = ItemDropGenerator.instance.GenerateItemDrops(lootLevel, dropRange);
    }

    public void SetOpenSprite()
    {
        GetComponent<SpriteRenderer>().sprite = chestOpen;
    }

    public void SetClosedSprite()
    {
        GetComponent<SpriteRenderer>().sprite = chestClosed;
    }
}
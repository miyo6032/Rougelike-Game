using UnityEngine;
using System.Collections.Generic;

public class ItemDropGenerator : MonoBehaviour {

    //Generate items drops - varies level and choose an amount based on a range
    public List<ItemSave> GenerateItemDrops(int level, Vector2Int dropRange)
    {
        List<ItemSave> items = new List<ItemSave>();

        int amount = Random.Range(dropRange.x, dropRange.y + 1);

        for (int i = 0; i < amount; i++)
        {
            int randomLevel = Mathf.Clamp(level + Random.Range(-1, 2), 1, 30);

            ItemSave item = new ItemSave(StaticCanvasList.instance.itemGenerator.GenerateItem(randomLevel, Random.Range(0, 3)), i);
            items.Add(item);
        }

        return items;
    }

}

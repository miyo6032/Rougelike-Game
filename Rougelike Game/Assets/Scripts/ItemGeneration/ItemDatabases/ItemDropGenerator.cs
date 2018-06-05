using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates items drops
/// </summary>
public class ItemDropGenerator : MonoBehaviour {

    /// <summary>
    /// Generate items drops - varies level and choose an amount based on a range
    /// </summary>
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

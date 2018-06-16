using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;

/// <summary>
/// Loads all of the item data from the json file and holds it for other scripts to use
/// </summary>
public class ItemDatabase : MonoBehaviour
{
    private readonly Dictionary<string, Item> items = new Dictionary<string, Item>();

    /// <summary>
    /// Finds the item if it exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Item GetItemByName(string id)
    {
        Item item;
        if (items.TryGetValue(id, out item))
        {
            return item;
        }

        return null;
    }

    /// <summary>
    /// Populates the database for use - called from the InventoryManager to populate before its start
    /// </summary>
    public void ConstructItemDatabase()
    {
        JsonData itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Consumables.json"));
        for (int i = 0; i < itemData.Count; i++)
        {
            string[] sprites = new string[1];
            sprites[0] = itemData[i]["sprite"].ToString();
            items.Add(itemData[i]["title"].ToString(),
                new Item(itemData[i]["title"].ToString(), (int) itemData[i]["value"],
                    itemData[i]["description"].ToString(), (bool) itemData[i]["stackable"],
                    (int) itemData[i]["itemLevel"], sprites, false));
        }
    }
}

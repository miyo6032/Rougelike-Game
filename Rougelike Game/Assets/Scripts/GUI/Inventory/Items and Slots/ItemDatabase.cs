using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;

//Loads all of the item data from the json file and holds it for other scripts to use
public class ItemDatabase : MonoBehaviour
{

    Dictionary<string, Item> items = new Dictionary<string, Item>();

    //Finds the item if it exists
    public Item GetItemByName(string id)
    {
        Item item;

        if (items.TryGetValue(id, out item))
        {
            return item;
        }

        return null;
    }

    //Populates the database for use - called from the InventoryManager to populate before its start
    public void ConstructItemDatabase()
    {
        JsonData itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
        for (int i = 0; i < itemData.Count; i++)
        {
            string[] sprites = new string[1];
            sprites[0] = itemData[i]["sprite"].ToString();

            items.Add(itemData[i]["title"].ToString(), new Item(
            itemData[i]["title"].ToString(),
            (int)itemData[i]["value"],
            (int)itemData[i]["stats"]["attack"],
            (int)itemData[i]["stats"]["maxattack"],
            (int)itemData[i]["stats"]["defence"],
             itemData[i]["description"].ToString(),
            (bool)itemData[i]["stackable"],
            (int)itemData[i]["equippedSlot"],
            (int)itemData[i]["itemLevel"],
            sprites,
            itemData[i]["itemColor"].ToString()
        ));
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;
using System.Linq;

/// <summary>
/// Loads all of the item data from the json file and holds it for other scripts to use
/// </summary>
public class ItemModuleDatabase : MonoBehaviour
{
    private readonly List<LeveledItemModule> blades = new List<LeveledItemModule>();
    private readonly List<ItemModule> hilts = new List<ItemModule>();
    private readonly List<ItemModule> handles = new List<ItemModule>();
    private readonly List<LeveledItemModule> baseArmor = new List<LeveledItemModule>();
    private readonly List<ItemModule> beltsAndCollars = new List<ItemModule>();
    private readonly List<LeveledItemModule> helmets = new List<LeveledItemModule>();
    private readonly List<ItemModule> helmetAccessories = new List<ItemModule>();

    public void PopulateItemModuleDatabase()
    {
        DataIntoLeveledItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Blades.json")), blades);
        DataIntoItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Hilts.json")), hilts);
        DataIntoItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Handles.json")), handles);
        DataIntoLeveledItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/BaseArmor.json")), baseArmor);
        DataIntoItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Belts&Collars.json")), beltsAndCollars);
        DataIntoLeveledItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Helmets.json")), helmets);
        DataIntoItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/HelmetAccessories.json")), helmetAccessories);
    }

    void DataIntoItemModules(JsonData itemData, List<ItemModule> parts)
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            if (itemData[i].Keys.Contains("stats"))
            {
                parts.Add(new ItemModule(
                    itemData[i]["title"].ToString(), 
                    itemData[i]["sprite"].ToString(),
                    itemData[i]["color"].ToString(),
                    (int)itemData[i]["stats"]["minAttack"],
                    (int)itemData[i]["stats"]["maxAttack"],
                    (int)itemData[i]["stats"]["defense"]
                ));
            }
            else
            {
                parts.Add(new ItemModule(
                    itemData[i]["title"].ToString(),
                    itemData[i]["sprite"].ToString(),
                    itemData[i]["color"].ToString()
                ));
            }
        }
    }

    void DataIntoLeveledItemModules(JsonData itemData, List<LeveledItemModule> parts)
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            parts.Add(new LeveledItemModule(
                itemData[i]["title"].ToString(), 
                itemData[i]["sprite"].ToString(),
                itemData[i]["color"].ToString(), 
                (int) itemData[i]["level"]));
        }
    }

    /// <summary>
    /// Based on a level, will find an item piece of a similar level
    /// </summary>
    /// <param name="level"></param>
    /// <param name="modules"></param>
    /// <returns></returns>
    LeveledItemModule FindLeveledItem(int level, List<LeveledItemModule> modules)
    {
        List<LeveledItemModule> possibleModules = new List<LeveledItemModule>();
        foreach (LeveledItemModule module in modules)
        {
            if (module.level == level)
            {
                possibleModules.Add(module);
            }
        }

        if (possibleModules.Count > 0)
        {
            return possibleModules[Random.Range(0, possibleModules.Count)];
        }

        return null;
    }

    /// <summary>
    /// Find a chestplated based on the level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public LeveledItemModule FindArmor(int level)
    {
        return FindLeveledItem(level, baseArmor);
    }

    /// <summary>
    /// Find a helmet based on the level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public LeveledItemModule FindHelmet(int level)
    {
        return FindLeveledItem(level, helmets);
    }

    /// <summary>
    /// Get a random accessory for a certain type of armor (helmet or chestplate)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public ItemModule GetRandomAccessory(int type)
    {
        if (type == 1)
        {
            return beltsAndCollars[Random.Range(0, beltsAndCollars.Count)];
        }
        return helmetAccessories[Random.Range(0, helmetAccessories.Count)];
    }

    /// <summary>
    /// Get a randomly generate sword - only the blade depends on the level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public ItemModule[] GetSword(int level)
    {
        ItemModule[] items = new ItemModule[3];
        items[0] = FindLeveledItem(level, blades);
        items[1] = hilts[Random.Range(0, hilts.Count)];
        items[2] = handles[Random.Range(0, handles.Count)];
        return items;
    }
}

using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;

//Loads all of the item data from the json file and holds it for other scripts to use
public class ItemModuleDatabase : MonoBehaviour {

    List<LeveledItemModule> blades = new List<LeveledItemModule>();
    List<ItemModule> hilts = new List<ItemModule>();
    List<ItemModule> handles = new List<ItemModule>();
    List<LeveledItemModule> baseArmor = new List<LeveledItemModule>();
    List<ItemModule> beltsAndCollars = new List<ItemModule>();
    List<LeveledItemModule> helmets = new List<LeveledItemModule>();
    List<ItemModule> helmetAccessories = new List<ItemModule>();

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
            parts.Add(new ItemModule(
                itemData[i]["title"].ToString(),
                itemData[i]["sprite"].ToString(),
                itemData[i]["color"].ToString()
            ));
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
                (int)itemData[i]["level"]
            ));
        }
    }

    LeveledItemModule FindLeveledItem(int level, List<LeveledItemModule> modules){
        List<LeveledItemModule> possibleModules = new List<LeveledItemModule>();
        foreach(LeveledItemModule module in modules)
        {
            if(module.level >= level - 1 && module.level <= level + 1)
            {
                possibleModules.Add(module);
            }
        }
        if(possibleModules.Count > 0)
        {
            return possibleModules[Random.Range(0, possibleModules.Count)];
        }
        return null;
    }

    public LeveledItemModule FindArmor(int level)
    {
        return FindLeveledItem(level, baseArmor);
    }

    public LeveledItemModule FindHelmet(int level)
    {
        return FindLeveledItem(level, helmets);
    }

    public ItemModule GetRandomAccessory(int type)
    {
        if (type == 1)
        {
            return beltsAndCollars[Random.Range(0, beltsAndCollars.Count)];
        }
        else
        {
            return helmetAccessories[Random.Range(0, helmetAccessories.Count)];
        }
    }

    public ItemModule[] GetSword(int level)
    {
        ItemModule[] items = new ItemModule[3];
        items[0] = FindLeveledItem(level, blades);
        items[1] = hilts[Random.Range(0, hilts.Count)];
        items[2] = handles[Random.Range(0, handles.Count)];
        return items;
    }

}

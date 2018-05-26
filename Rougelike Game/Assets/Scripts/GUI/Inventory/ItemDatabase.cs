using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;

//Loads all of the item data from the json file and holds it for other scripts to use
public class ItemDatabase : MonoBehaviour {

    List<ItemModule> blades = new List<ItemModule>();
    List<ItemModule> hilts = new List<ItemModule>();
    List<ItemModule> handles = new List<ItemModule>();
    List<LeveledItemModule> baseArmor = new List<LeveledItemModule>();
    List<ItemModule> beltsAndCollars = new List<ItemModule>();
    List<LeveledItemModule> helmets = new List<LeveledItemModule>();
    List<ItemModule> helmetAccessories = new List<ItemModule>();

    Dictionary<string, Sprite> textures = new Dictionary<string, Sprite>();

    string[] itemCategories = { "armor", "swords", "helmets" };

    public void PopulateItemModuleDatabase()
    {
        DataIntoItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Blades.json")), blades);
        DataIntoItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Hilts.json")), hilts);
        DataIntoItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Handles.json")), handles);
        DataIntoLeveledItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/BaseArmor.json")), baseArmor);
        DataIntoItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Belts&Collars.json")), beltsAndCollars);
        DataIntoLeveledItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Helmets.json")), helmets);
        DataIntoItemModules(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/HelmetAccessories.json")), helmetAccessories);
        LoadAllTextures();
    }

    void LoadAllTextures()
    {
        foreach (string category in itemCategories)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("ItemIcons/" + category);
            foreach (Sprite sprite in sprites)
            {
                string path = sprite.ToString();
                path = category + "/" + path.Substring(0, path.IndexOf(" "));
                textures.Add(path, sprite);
            }
        }
    }

    public Sprite LoadTexture(string texture)
    {
        Sprite sprite;
        textures.TryGetValue(texture, out sprite);
        return sprite;
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

    public Item GenerateItem(int level, int equipmentType)
    {
        //The sword
        if(equipmentType == 0)
        {
            return GenerateSword(level, equipmentType);
        }
        return GenerateArmor(level, equipmentType);
    }

    Item GenerateSword(int level, int equipmentType)
    {
        ItemModule blade = blades[Random.Range(0, blades.Count)];
        ItemModule hilt = hilts[Random.Range(0, hilts.Count)];
        ItemModule handle = handles[Random.Range(0, handles.Count)];

        string title = hilt.Title + " " + blade.Title + " " + handle.Title;
        int attack = Mathf.CeilToInt(level * Random.Range(0.5f, 1));
        int maxAttack = Mathf.CeilToInt(attack + level * Random.Range(0.5f, 1));
        int defence = Mathf.CeilToInt(level * Random.Range(0f, 0.5f));
        int value = attack + maxAttack + defence;
        bool stackable = false;
        string description = "";
        string[] sprites = { blade.Sprite, handle.Sprite, hilt.Sprite };

        Item sword = new Item(
            title,
            value,
            attack,
            maxAttack,
            defence,
            description,
            stackable,
            equipmentType,
            level,
            -1,
            sprites,
            blade.Color
        );

        return sword;
    }

    Item GenerateArmor(int level, int equipmentType)
    {
        LeveledItemModule armor;
        ItemModule accessory;
        string title;
        if (equipmentType == 1)
        {
            armor = findLeveledItem(level, baseArmor);
            accessory = beltsAndCollars[Random.Range(0, beltsAndCollars.Count)];
            title = armor.itemModule.Title + " " + accessory.Title;
        }
        else
        {
            armor = findLeveledItem(level, helmets);
            accessory = helmetAccessories[Random.Range(0, helmetAccessories.Count)];
            title = accessory.Title + " " + armor.itemModule.Title;
        }

        level = armor.level + Mathf.Clamp(Random.Range(-1, 2), 1, 30); //Change levels a little bit

        int attack = 0;
        int maxAttack = 0;
        int defence = Mathf.CeilToInt(level * Random.Range(1f, 2f) * 0.8f);
        int value = attack + maxAttack + defence;
        bool stackable = false;
        string description = "";
        string[] sprites = { armor.itemModule.Sprite, accessory.Sprite };

        Item item = new Item(
            title,
            value,
            attack,
            maxAttack,
            defence,
            description,
            stackable,
            equipmentType,
            level,
            -1,
            sprites,
            armor.itemModule.Color
        );



        return item;
    }

    LeveledItemModule findLeveledItem(int level, List<LeveledItemModule> modules){
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

}

public class ItemModule
{
    public string Title;
    public string Sprite;
    public string Color;

    public ItemModule(string title, string sprite, string color)
    {
        Title = title;
        Sprite = sprite;
        Color = color;
    }

}

public class LeveledItemModule
{
    public ItemModule itemModule;
    public int level;

    public LeveledItemModule(string title, string sprite, string color, int level)
    {
        itemModule = new ItemModule(title, sprite, color);
        this.level = level;
    }
}

[System.Serializable]
public class Item{
	public string Title { get; set;}
	public int Value { get; set;}
	public int Attack { get; set;}
    public int MaxAttack { get; set; }
    public int Defence { get; set;}
	public string Description { get; set;}
	public bool Stackable { get; set;}
	public string[] Sprites { get; set;}
	public int EquippedSlot { get; set;}
	public int ItemLevel { get; set;}
    public string ItemColor { get; set; }

    public Item(string title, int value, int attack, int maxAttack, int defence, string description, bool stackable, int equippedSlot, int itemLevel, int skill, string[] sprites, string itemColor){
		this.Title = title;
		this.Value = value;
		this.Attack = attack;
        this.MaxAttack = maxAttack;
		this.Defence = defence;
		this.Description = description;
		this.Stackable = stackable;
		this.EquippedSlot = equippedSlot;
		this.Sprites = sprites;
		this.ItemLevel = itemLevel;
        this.ItemColor = itemColor;
	}

	public Item(){}
}

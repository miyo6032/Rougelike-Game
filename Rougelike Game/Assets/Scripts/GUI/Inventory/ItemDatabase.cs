using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;

//Loads all of the item data from the json file and holds it for other scripts to use
public class ItemDatabase : MonoBehaviour {

    List<ItemModule> blades = new List<ItemModule>();
    List<ItemModule> hilts = new List<ItemModule>();
    List<ItemModule> handles = new List<ItemModule>();
    List<ItemModule> baseArmor = new List<ItemModule>();
    List<ItemModule> beltsAndCollars = new List<ItemModule>();
    List<ItemModule> gloves = new List<ItemModule>();

    Dictionary<string, Sprite> textures = new Dictionary<string, Sprite>();

    string[] itemCategories = { "armor", "swords" };

    public void PopulateItemModuleDatabase()
    {
        DataIntoList(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Blades.json")), blades);
        DataIntoList(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Hilts.json")), hilts);
        DataIntoList(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Handles.json")), handles);
        DataIntoList(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/BaseArmor.json")), baseArmor);
        DataIntoList(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Belts&Collars.json")), beltsAndCollars);
        DataIntoList(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/ItemPieces/Gloves.json")), gloves);
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

    void DataIntoList(JsonData itemData, List<ItemModule> parts)
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            parts.Add(new ItemModule(
                itemData[i]["title"].ToString(),
                itemData[i]["sprite"].ToString()
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
        else if(equipmentType == 1)
        {
            return GenerateArmor(level, equipmentType);
        }

        return null;

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
            sprites
        );

        return sword;
    }

    Item GenerateArmor(int level, int equipmentType)
    {
        ItemModule armor = baseArmor[Random.Range(0, baseArmor.Count)];
        ItemModule accessory = beltsAndCollars[Random.Range(0, beltsAndCollars.Count)];
        ItemModule glove = gloves[Random.Range(0, gloves.Count)];

        string title = glove.Title + " " + armor.Title + " " + accessory.Title;
        int attack = 0;
        int maxAttack = 0;
        int defence = Mathf.CeilToInt(level * Random.Range(1f, 2f));
        int value = attack + maxAttack + defence;
        bool stackable = false;
        string description = "";
        string[] sprites = { armor.Sprite, accessory.Sprite, glove.Sprite };

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
            sprites
        );

        return sword;
    }

}

public class ItemModule
{
    public string Title;
    public string Sprite;

    public ItemModule(string title, string sprite)
    {
        Title = title;
        Sprite = sprite;
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

    public Item(string title, int value, int attack, int maxAttack, int defence, string description, bool stackable, int equippedSlot, int itemLevel, int skill, string[] sprites){
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
	}

	public Item(){}
}

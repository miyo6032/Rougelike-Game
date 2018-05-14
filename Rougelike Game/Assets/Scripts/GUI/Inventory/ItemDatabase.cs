using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;

//Loads all of the item data from the json file and holds it for other scripts to use
public class ItemDatabase : MonoBehaviour {

    List<ItemModule> blades = new List<ItemModule>();
    List<ItemModule> hilts = new List<ItemModule>();
    List<ItemModule> handles = new List<ItemModule>();

    public void PopulateItemModuleDatabase()
    {
        DataIntoList(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Blades.json")), blades);
        DataIntoList(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Hilts.json")), hilts);
        DataIntoList(JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Handles.json")), handles);
    }

    void DataIntoList(JsonData itemData, List<ItemModule> parts)
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            parts.Add(new ItemModule(
                itemData[i]["title"].ToString(),
                itemData[i]["description"].ToString(),
                itemData[i]["sprite"].ToString()
            ));
        }
    }

    public Item GenerateItem(int level, int equipmentType)
    {
        //The sword
        if(equipmentType == 0)
        {
            ItemModule blade = blades[Random.Range(0, blades.Count)];
            ItemModule hilt = hilts[Random.Range(0, hilts.Count)];
            ItemModule handle = handles[Random.Range(0, handles.Count)];

            string title = hilt.Title + blade.Title + handle.Title;
            int attack = (int)(level * Random.Range(0f, 1));
            int maxAttack = (int)(attack + level * Random.Range(0f, 1));
            int defence = (int)(level * Random.Range(0f, 1));
            int value = attack + maxAttack + defence;
            bool stackable = false;
            string description = "";
            string[] sprites = { blade.Sprite, hilt.Sprite, handle.Sprite };
            
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

        return null;

    }

}

public class ItemModule
{
    public string Title;
    public string Description;
    public string Sprite;

    public ItemModule(string title, string description, string sprite)
    {
        Title = title;
        Description = description;
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

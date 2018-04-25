using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;

//Loads all of the item data from the json file and holds it for other scripts to use
public class ItemDatabase : MonoBehaviour {

	public Dictionary<int, Item> items = new Dictionary<int, Item>();

    //Finds the 
	public Item GetItemByID(int id){

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
                items.Add((int)itemData[i]["id"], new Item(
                (int)itemData[i]["id"],
                itemData[i]["title"].ToString(),
                (int)itemData[i]["value"],
                (int)itemData[i]["stats"]["attack"],
                (int)itemData[i]["stats"]["maxattack"],
                (int)itemData[i]["stats"]["defence"],
                 itemData[i]["description"].ToString(),
                (bool)itemData[i]["stackable"],
                (string)itemData[i]["slug"],
                (int)itemData[i]["equippedSlot"],
                (int)itemData[i]["itemLevel"],
                (int)itemData[i]["skill"]
            ));
        }
    }
}

public class Item{
	public int Id{ get; set;}
	public string Title { get; set;}
	public int Value { get; set;}
	public int Attack { get; set;}
    public int MaxAttack { get; set; }
    public int Defence { get; set;}
	public string Description { get; set;}
	public bool Stackable { get; set;}
	public string Slug { get; set;}
	public Sprite Sprite { get; set;}
	public int EquippedSlot { get; set;}
	public int ItemLevel { get; set;}
    public int Skill { get; set; }

    public Item(int id, string title, int value, int attack, int maxAttack, int defence, string description, bool stackable, string slug, int equippedSlot, int itemLevel, int skill){
		this.Id = id;
		this.Title = title;
		this.Value = value;
		this.Attack = attack;
        this.MaxAttack = maxAttack;
		this.Defence = defence;
		this.Description = description;
		this.Stackable = stackable;
		this.Slug = slug;
		this.EquippedSlot = equippedSlot;
		this.Sprite = Resources.Load<Sprite> ("ItemIcons/" + slug);
		this.ItemLevel = itemLevel;
        this.Skill = skill;
	}

	public Item(){
		this.Id = -1;
	}
}

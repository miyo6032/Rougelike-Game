using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.IO;

public class ItemDatabase : MonoBehaviour {

	public List<Item> items = new List<Item>();

	private List<Item> database = new List<Item>();
	private JsonData itemData;

	void Start(){

		itemData = JsonMapper.ToObject (File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
		ConstructItemDatabase ();

	}

	public Item GetItemByID(int id){
		for (int i = 0; i < database.Count; i++) {
			if(id == database[i].Id)
				return database[i];
		}
		return null;
	}

	void ConstructItemDatabase(){
		for(int i = 0; i < itemData.Count; i++){
            for (int r = 0; r < 4; r++)//Adds duplicate rarer items;
            {
                database.Add(new Item(
                (int)itemData[i]["id"] + r*1000,
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
                r,
                (int)itemData[i]["skill"]
            ));
            }
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
    public int Rarity { get; set; }
    public int Skill { get; set; }

    public Item(int id, string title, int value, int attack, int maxAttack, int defence, string description, bool stackable, string slug, int equippedSlot, int itemLevel, int rarity, int skill){
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
        this.Rarity = rarity;
        this.Skill = skill;
	}

	public Item(){
		this.Id = -1;
	}
}

using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;

//Loads all of the item data from the json file and holds it for other scripts to use
public class ItemDatabase : MonoBehaviour {

    List<ItemModule> blades = new List<ItemModule>();
    List<ItemModule> hilts = new List<ItemModule>();
    List<ItemModule> handles = new List<ItemModule>();

    public Item GenerateItem(int level, int equipmentType)
    {
        //The sword
        if(equipmentType == 0)
        {
            ItemModule blade = blades[Random.Range(0, blades.Count)];
            ItemModule hilt = hilts[Random.Range(0, hilts.Count)];
            ItemModule handle = handles[Random.Range(0, handles.Count)];

            string title = hilt.Title + blade.Title + handle.Title;
            //other stats

            Item sword = new Item();
        }

        return null;

    }

}

public class ItemModule
{
    public string Title;
    public string Type;
    public string Sprite;

    public ItemModule(string title, string type, string sprite)
    {
        Title = title;
        Type = type;
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
	public string Slug { get; set;}
	public string Sprite { get; set;}
	public int EquippedSlot { get; set;}
	public int ItemLevel { get; set;}

    public Item(string title, int value, int attack, int maxAttack, int defence, string description, bool stackable, string slug, int equippedSlot, int itemLevel, int skill){
		this.Title = title;
		this.Value = value;
		this.Attack = attack;
        this.MaxAttack = maxAttack;
		this.Defence = defence;
		this.Description = description;
		this.Stackable = stackable;
		this.Slug = slug;
		this.EquippedSlot = equippedSlot;
		this.Sprite = "ItemIcons/" + slug;
		this.ItemLevel = itemLevel;
	}

	public Item(){}
}

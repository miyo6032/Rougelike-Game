using UnityEngine;/// <summary>
/// Represents an item's data
/// </summary>
[System.Serializable]
public class Item : ScriptableObject
{
    public string Title { get; set; }
    public int Value { get; set; }
    public int Attack { get; set; }
    public int MaxAttack { get; set; }
    public int Defence { get; set; }
    public string Description { get; set; }
    public bool Stackable { get; set; }
    public string[] Sprites { get; set; }
    public int EquippedSlot { get; set; }
    public int ItemLevel { get; set; }
    public string ItemColor { get; set; }

    public Item(string title, int value, int attack, int maxAttack, int defence, string description, bool stackable, int equippedSlot, int itemLevel, string[] sprites, string itemColor)
    {
        Title = title;
        Value = value;
        Attack = attack;
        MaxAttack = maxAttack;
        Defence = defence;
        Description = description;
        Stackable = stackable;
        EquippedSlot = equippedSlot;
        Sprites = sprites;
        ItemLevel = itemLevel;
        ItemColor = itemColor;
    }

    public Item(string title, int value, string description, bool stackable, int itemLevel, string[] sprites)
    {
        Title = title;
        Value = value;
        Attack = 0;
        MaxAttack = 0;
        Defence = 0;
        Description = description;
        Stackable = stackable;
        EquippedSlot = -1;
        Sprites = sprites;
        ItemLevel = itemLevel;
        ItemColor = "";
    }

    public Item() { }
}
using UnityEngine;/// <summary>
/// Represents an item's data
/// </summary>
[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Item")]
public class Item : ScriptableObject
{
    public string Title;
    public int Value;
    public int Attack;
    public int MaxAttack;
    public int Defence;
    public string Description;
    public bool Stackable;
    public bool Consumable;
    public string[] Sprites;
    public int EquippedSlot = -1;
    public int ItemLevel = 1;
    public string ItemColor;
    public Modifier[] ModifiersAffected;
    public int focusConsumption;

    public Item(string title, int value, int attack, int maxAttack, int defence, string description, bool stackable, int equippedSlot, int itemLevel, string[] sprites, string itemColor, bool consumable)
        : this (title, value, description, stackable, itemLevel, sprites, consumable)
    {
        Attack = attack;
        MaxAttack = maxAttack;
        Defence = defence;
        Description = description;
        EquippedSlot = equippedSlot;
        ItemColor = itemColor;
        ModifiersAffected = new Modifier[0];
        focusConsumption = 0;
    }

    public Item(string title, int value, string description, bool stackable, int itemLevel, string[] sprites, bool consumable)
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
        ModifiersAffected = new Modifier[0];
        Consumable = consumable;
        focusConsumption = 0;
    }

    public Item() { }
}
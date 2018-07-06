using UnityEngine;

[System.Serializable]
public enum EquipmentType
{
    None,
    Sword,
    Armor,
    Helmet
}

/// <summary>
/// Represents an item's data
/// </summary>
[System.Serializable]
public class Item
{
    public string Title;
    [Range(0, 10000)]
    public int Value;
    public string Description;
    public bool Stackable;
    [HideInInspector]
    public string[] Sprites;
    [Header("Equipment")]
    public EquipmentType EquippedSlot = EquipmentType.None;
    [Range(0, 100)]
    public int Attack;
    [Range(0, 100)]
    public int MaxAttack;
    [Range(0, 100)]
    public int Defence;
    [Range(1, 100)]
    public int ItemLevel = 1;
    [HideInInspector]
    public string ItemColor;
    [Header("Consumable")]
    public bool Consumable;
    public Modifier[] ModifiersAffected;
    [Range(0, 100)]
    public int focusConsumption;

    public Item(string title, int value, int attack, int maxAttack, int defence, string description, bool stackable, EquipmentType equippedSlot, int itemLevel, string[] sprites, string itemColor, bool consumable)
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
        EquippedSlot = EquipmentType.None;
        Sprites = sprites;
        ItemLevel = itemLevel;
        ItemColor = "";
        ModifiersAffected = new Modifier[0];
        Consumable = consumable;
        focusConsumption = 0;
    }

    public Item() { }
}
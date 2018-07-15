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
    public string Sprite;
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
    public Effect[] ConsumptionEffects;
    [Range(0, 100)]
    public int focusConsumption;
}
using UnityEngine;
using System;
using Rougelike_Game.Assets.Scripts.GUI.Inventory.Items_and_Slots;

[CreateAssetMenu(menuName = "Custom/Item")]
public class ItemScriptableObject : ScriptableObject
{
    public string title;
    public string description;
    public bool stackable;
    [Range(0, 10000)]
    public int value;
    public Sprite sprite;

    [Header("Equipment")]
    public EquipmentType equippedSlot = EquipmentType.None;

    public Modifier[] equipmentModifiers;

    [Range(1, 100)]
    public int level = 1;

    [Header("Consumable")]
    public bool consumable;

    public Effect[] consumptionEffects;

    [Range(0, 100)]
    public int focusConsumption;

}

using UnityEngine;
using System;
using Rougelike_Game.Assets.Scripts.GUI.Inventory.Items_and_Slots;

[CreateAssetMenu(menuName = "Custom/Item")]
public class ItemScriptableObject : ScriptableObject
{
    [SerializeField]
    private string title;
    [SerializeField]
    private string description;
    [Range(0, 10000), SerializeField]
    private int value;
    [SerializeField]
    private Sprite sprite;

    [Header("Equipment"), SerializeField]
    private EquipmentType equippedSlot = EquipmentType.None;
    [SerializeField]
    private Modifier[] equipmentModifiers;
    [Range(1, 100), SerializeField]
    private int level = 1;

    [Header("Effect")]
    [SerializeField]
    private bool consumable;
    [SerializeField]
    private Effect[] consumptionEffects;

    [Range(0, 100), SerializeField]
    private int focusConsumption;

    [Header("Metadata")]
    [SerializeField]
    private bool stackable;
    [SerializeField]
    private int id;

    public string GetTitle() { return title; }
    public string GetDescription() {return description;}
    public int GetValue() {return value;}
    public Sprite GetSprite(){return sprite;}
    public Modifier[] GetEquipmentModifiers() {
        Modifier[] modifiers = new Modifier[equipmentModifiers.Length];
        Array.Copy(equipmentModifiers, modifiers, equipmentModifiers.Length);
        return modifiers;
    }
    public Effect[] GetConsumptionEffects(){
        Effect[] effect = new Effect[consumptionEffects.Length];
        Array.Copy(consumptionEffects, effect, consumptionEffects.Length);
        return effect;
    }
    public EquipmentType GetEquipmentSlot(){return equippedSlot;}
    public int GetLevel(){return level;}
    public bool GetIsConsumable(){return consumable;}
    public int GetFocusConsumption(){return focusConsumption;}
    public bool GetIsStackable(){return stackable;}
    public int GetId(){return id;}
}

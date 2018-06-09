using UnityEngine;

/// <summary>
/// Produces generated gear from pieces of items
/// </summary>
public class ItemGenerator : MonoBehaviour
{

    public int damageStart = 5;

    public Item GenerateItem(int level, int equipmentType)
    {
        //The sword
        if (equipmentType == 0)
        {
            return GenerateSword(level, equipmentType);
        }

        //The other armors
        return GenerateArmor(level, equipmentType);
    }

    Item GenerateSword(int level, int equipmentType)
    {
        ItemModule[] pieces = StaticCanvasList.instance.itemModuleDatabase.GetSword(level);
        ItemModule blade = pieces[0];
        ItemModule hilt = pieces[1];
        ItemModule handle = pieces[2];
        string title = hilt.Title + " " + blade.Title + " " + handle.Title;
        int bonusMinAttack = hilt.MinAttack + blade.MinAttack + handle.MinAttack;
        int bonusMaxAttack = hilt.MaxAttack + blade.MaxAttack + handle.MaxAttack;
        int attack = Mathf.RoundToInt(level + damageStart) + bonusMinAttack;
        int maxAttack = Mathf.RoundToInt(attack + level/2 + damageStart) + bonusMaxAttack;
        int defence = hilt.Defense + blade.Defense + handle.Defense;
        int value = attack + maxAttack + defence * 2;
        string description = "";
        string[] sprites = {blade.Sprite, handle.Sprite, hilt.Sprite};
        Item sword = new Item(title, value, attack, maxAttack, defence, description, false, equipmentType, level,
            sprites, blade.Color);
        return sword;
    }

    //Generates Helmets and Armor
    Item GenerateArmor(int level, int equipmentType)
    {
        LeveledItemModule armor;
        if (equipmentType == 1)
        {
            armor = StaticCanvasList.instance.itemModuleDatabase.FindArmor(level);
        }
        else
        {
            armor = StaticCanvasList.instance.itemModuleDatabase.FindHelmet(level);
        }

        string title;
        string[] sprites;
        ItemModule accessory = new ItemModule("", "", "");

        //Handle case when item has accessory or not
        if (Random.Range(0, 2) == 0)
        {
            accessory = StaticCanvasList.instance.itemModuleDatabase.GetRandomAccessory(equipmentType);
            title = armor.Title + " " + accessory.Title;
            sprites = new string[2];
            sprites[0] = armor.Sprite;
            sprites[1] = accessory.Sprite;
        }
        else
        {
            title = armor.Title;
            sprites = new string[1];
            sprites[0] = armor.Sprite;
        }

        int bonusMinAttack = armor.MinAttack + accessory.MinAttack;
        int bonusMaxAttack = armor.MaxAttack + accessory.MaxAttack;
        int attack = bonusMinAttack;
        int maxAttack = bonusMaxAttack;
        int bonusDefense = armor.Defense + accessory.Defense;
        int defence = Mathf.RoundToInt(level + damageStart) + bonusDefense;
        int value = attack + maxAttack + Mathf.RoundToInt(defence * 2f);
        string description = "";
        Item item = new Item(title, value, attack, maxAttack, defence, description, false, equipmentType, level,
            sprites, armor.Color);
        return item;
    }
}

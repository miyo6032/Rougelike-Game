using UnityEngine;

//Produces new randomly generated items for the inventory and chest and loot
public class ItemGenerator : MonoBehaviour {

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
        int attack = Mathf.RoundToInt(level * Random.Range(0.5f, 1f));
        int maxAttack = Mathf.RoundToInt(attack + level * Random.Range(0.5f, 1f));
        int defence = 0;
        int value = attack + maxAttack + defence;
        bool stackable = false;
        string description = "";
        string[] sprites = { blade.Sprite, handle.Sprite, hilt.Sprite };

        Item sword = new Item(
            title,
            value,
            attack,
            maxAttack,
            defence,
            description,
            stackable,
            equipmentType,
            level,
            sprites,
            blade.Color
        );

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

        //Handle case when item has accessory or not
        if (Random.Range(0, 2) == 0)
        {
            ItemModule accessory = StaticCanvasList.instance.itemModuleDatabase.GetRandomAccessory(equipmentType);
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

        int attack = 0;
        int maxAttack = 0;
        int defence = Mathf.RoundToInt(level * Random.Range(0.5f, 1.5f));
        int value = attack + maxAttack + Mathf.RoundToInt(defence * 2f);
        bool stackable = false;
        string description = "";

        Item item = new Item(
            title,
            value,
            attack,
            maxAttack,
            defence,
            description,
            stackable,
            equipmentType,
            level,
            sprites,
            armor.Color
        );

        return item;
    }

}

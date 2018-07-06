using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates items drops
/// </summary>
public class ItemDropGenerator : MonoBehaviour {

    public enum ItemDropTypes
    {
        CommonArtifact,
        RareArtifact,
        RareArmor,
        RareWeapon,
        RareHelmet
    }

    public int CommonArtifactDropWeight;
    public int RareArtifactDropWeight;
    public int RareArmorDropWeight;
    public int RareWeaponDropWeight;
    public int RareHelmetDropWeight;

    private List<ItemDropTypes> ItemDropQueue = new List<ItemDropTypes>();

    private void Start()
    {
        FillItemDropQueue();
    }

    public void FillItemDropQueue()
    {
        FillWithType(CommonArtifactDropWeight, ItemDropTypes.CommonArtifact);
        FillWithType(RareArtifactDropWeight, ItemDropTypes.RareArtifact);
        FillWithType(RareArmorDropWeight, ItemDropTypes.RareArmor);
        FillWithType(RareWeaponDropWeight, ItemDropTypes.RareWeapon);
        FillWithType(RareHelmetDropWeight, ItemDropTypes.RareHelmet);
    }

    private void FillWithType(int weight, ItemDropTypes type)
    {
        for (int i = 0; i < weight; i++)
        {
            ItemDropQueue.Add(type);
        }
    }

    /// <summary>
    /// Generate items drops - varies level and choose an amount based on a range
    /// </summary>
    public List<ItemSave> GenerateItemDrops(int level, Vector2Int dropRange)
    {
        List<ItemSave> items = new List<ItemSave>();

        int amount = Random.Range(dropRange.x, dropRange.y + 1);

        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, ItemDropQueue.Count);
            ItemDropTypes itemType = ItemDropQueue[index];
            ItemDropQueue.Remove(itemType);
            int randomLevel = Mathf.Clamp(level + Random.Range(-1, 2), 1, 30);
            ItemSave item;
            switch (itemType)
            {
                case ItemDropTypes.RareArmor:
                    item = new ItemSave(StaticCanvasList.instance.moduledItemGenerator.GenerateItem(randomLevel, EquipmentType.Armor), i, 1);
                    break;
                case ItemDropTypes.RareWeapon:
                    item = new ItemSave(StaticCanvasList.instance.moduledItemGenerator.GenerateItem(randomLevel, EquipmentType.Sword), i, 1);
                    break;
                case ItemDropTypes.RareHelmet:
                    item = new ItemSave(StaticCanvasList.instance.moduledItemGenerator.GenerateItem(randomLevel, EquipmentType.Helmet), i, 1);
                    break;
                case ItemDropTypes.RareArtifact:
                    item = new ItemSave(StaticCanvasList.instance.presetItemDatabase.GetArtifact(randomLevel + 3), i, 1);
                    break;
                default:
                    item = new ItemSave(StaticCanvasList.instance.presetItemDatabase.GetArtifact(randomLevel), i, 1);
                    break;
            }
            ItemSave duplicateItem = GetDuplicateItem(items, item);
            if (duplicateItem != null)
            {
                duplicateItem.amount += 1;
            }
            else
            {
                items.Add(item);
            }
            if(ItemDropQueue.Count == 0)
            {
                FillItemDropQueue();
            }
        }

        return items;
    }

    private ItemSave GetDuplicateItem(List<ItemSave> items, ItemSave item)
    {
        foreach (ItemSave prevItem in items)
        {
            if (prevItem.item == item.item)
            {
                return prevItem;
            }
        }
        return null;
    }

}

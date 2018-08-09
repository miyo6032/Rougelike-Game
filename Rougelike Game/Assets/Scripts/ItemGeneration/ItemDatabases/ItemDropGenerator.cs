using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Generates items drops
/// </summary>
public class ItemDropGenerator : MonoBehaviour
{
    public static ItemDropGenerator instance;

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
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Duplicate " + this.GetType().Name);
            Destroy(gameObject);
        }
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
                case ItemDropTypes.RareArtifact:
                    item = new ItemSave(new ItemStack(ItemDatabase.instance.GetArtifact(randomLevel + 3), 1), 0);
                    break;

                default:
                    item = new ItemSave(new ItemStack(ItemDatabase.instance.GetArtifact(randomLevel), 1), 0);
                    break;
            }
            ItemSave duplicateItem = GetDuplicateItem(items, item);
            if (duplicateItem != null)
            {
                duplicateItem.item.amount += 1;
            }
            else
            {
                items.Add(item);
            }
            if (ItemDropQueue.Count == 0)
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
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
        RareWeapon,
        RareShield,
        RareChestplate,
        RareHelmet,
        RareLeggings,
        RareBoots,
        RareNecklace,
        RareRing
    }

    public List<ItemScriptableObject> artifacts;
    public List<ItemScriptableObject> swords;
    public List<ItemScriptableObject> chestplates;
    public List<ItemScriptableObject> helmets;
    public List<ItemScriptableObject> leggings;
    public List<ItemScriptableObject> boots;
    public List<ItemScriptableObject> shields;
    public List<ItemScriptableObject> rings;
    public List<ItemScriptableObject> necklaces;

    public int CommonArtifactDropWeight;
    public int RareArtifactDropWeight;
    public int RareEquipmentDropWeight;

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
        FillWithType(RareEquipmentDropWeight, ItemDropTypes.RareBoots);
        FillWithType(RareEquipmentDropWeight, ItemDropTypes.RareChestplate);
        FillWithType(RareEquipmentDropWeight, ItemDropTypes.RareLeggings);
        FillWithType(RareEquipmentDropWeight, ItemDropTypes.RareNecklace);
        FillWithType(RareEquipmentDropWeight, ItemDropTypes.RareRing);
        FillWithType(RareEquipmentDropWeight, ItemDropTypes.RareShield);
        FillWithType(RareEquipmentDropWeight, ItemDropTypes.RareWeapon);
        FillWithType(RareEquipmentDropWeight, ItemDropTypes.RareHelmet);
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

        List<int> chestSlots = new List<int>();
        for (int i = 0; i < ChestInventory.instance.chestSlots.Count; i++)
        {
            chestSlots.Add(i);
        }

        int amount = Random.Range(dropRange.x, dropRange.y + 1);

        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, ItemDropQueue.Count);
            ItemDropTypes itemType = ItemDropQueue[index];
            ItemDropQueue.Remove(itemType);
            int randomLevel = Mathf.Clamp(level + Random.Range(-1, 2), 1, 30);
            int randomSlot = chestSlots[Random.Range(0, chestSlots.Count)];
            chestSlots.Remove(randomSlot);
            ItemSave item;
            switch (itemType)
            {
                case ItemDropTypes.RareArtifact:
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel + 3, artifacts), 1), randomSlot);
                    break;

                case ItemDropTypes.RareChestplate:
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel + 3, chestplates), 1), randomSlot);
                    break;

                case ItemDropTypes.RareBoots:
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel + 3, boots), 1), randomSlot);
                    break;

                case ItemDropTypes.RareHelmet:
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel + 3, helmets), 1), randomSlot);
                    break;

                case ItemDropTypes.RareLeggings:
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel + 3, leggings), 1), randomSlot);
                    break;

                case ItemDropTypes.RareNecklace:
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel + 3, necklaces), 1), randomSlot);
                    break;

                case ItemDropTypes.RareRing:
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel + 3, rings), 1), randomSlot);
                    break;

                case ItemDropTypes.RareShield:
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel + 3, shields), 1), randomSlot);
                    break;

                case ItemDropTypes.RareWeapon:
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel + 3, swords), 1), randomSlot);
                    break;

                default:
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel, artifacts), 1), randomSlot);
                    break;
            }

            items.Add(item);

            if (ItemDropQueue.Count == 0)
            {
                FillItemDropQueue();
            }
        }

        return items;
    }

    /// <summary>
    /// Get an artifact based on a level given (and thus more valuable)
    /// </summary>
    private Item GetRandomItemByLevel(int level, List<ItemScriptableObject> items)
    {
        List<ItemScriptableObject> candidateItems = items.FindAll(itemtype => itemtype.item.ItemLevel == level);

        if (candidateItems.Count > 0)
        {
            return candidateItems[Random.Range(0, candidateItems.Count)].item;
        }
        return GetRandomItemByLevel(level - 1, items);
    }
}
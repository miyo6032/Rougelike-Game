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
        Equipment
    }

    public List<ItemScriptableObject> artifacts;
    public List<ItemScriptableObject> equipment;

    public int CommonArtifactDropWeight;
    public int RareArtifactDropWeight;
    public int EquipmentDropWeight;

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
        FillWithType(EquipmentDropWeight, ItemDropTypes.Equipment);
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
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel + 5, artifacts), 1), randomSlot);
                    break;

                case ItemDropTypes.Equipment:
                    item = new ItemSave(new ItemStack(GetRandomItemByLevel(randomLevel, equipment), 1), randomSlot);
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
    private ItemScriptableObject GetRandomItemByLevel(int level, List<ItemScriptableObject> items)
    {
        List<ItemScriptableObject> candidateItems = items.FindAll(itemtype => itemtype.level == level);

        if (candidateItems.Count > 0)
        {
            return candidateItems[Random.Range(0, candidateItems.Count)];
        }
        return GetRandomItemByLevel(level - 1, items);
    }
}
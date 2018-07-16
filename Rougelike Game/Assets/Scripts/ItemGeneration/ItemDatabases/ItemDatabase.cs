using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gets preset items that are generated from item scritable objects
/// </summary>
public class ItemDatabase : MonoBehaviour {

    public List<ItemScriptableObject> artifacts;
    public List<ItemScriptableObject> swords;
    public List<ItemScriptableObject> armor;
    public List<ItemScriptableObject> helmets;

    public static ItemDatabase instance;

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
    }

    /// <summary>
    /// Get an artifact based on a level given (and thus more valuable)
    /// </summary>
    public Item GetArtifact(int level)
    {
        List<ItemScriptableObject> candidateItems = artifacts.FindAll(itemtype => itemtype.item.ItemLevel == level);

        if (candidateItems.Count > 0)
        {
            return candidateItems[Random.Range(0, candidateItems.Count)].item;
        }
        return GetArtifact(level - 1);
    }
}

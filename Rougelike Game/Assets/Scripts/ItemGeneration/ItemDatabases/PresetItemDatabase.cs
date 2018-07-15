using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gets preset items that are generated from item scritable objects
/// </summary>
public class PresetItemDatabase : MonoBehaviour {

    public List<ItemScriptableObject> Artifacts;

    /// <summary>
    /// Get an artifact based on a level given (and thus more valuable)
    /// </summary>
    public Item GetArtifact(int level)
    {
        List<ItemScriptableObject> candidateItems = Artifacts.FindAll(itemtype => itemtype.item.ItemLevel == level);

        if (candidateItems.Count > 0)
        {
            ItemScriptableObject item = candidateItems[Random.Range(0, candidateItems.Count)];
            item.Start();
            return item.item;
        }
        return GetArtifact(level - 1);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PresetItemDatabase : MonoBehaviour {

    public List<ItemScriptableObject> Artifacts;

    public Item GetArtifact(int level)
    {
        List<ItemScriptableObject> candidateItems = Artifacts.FindAll(itemtype => itemtype.Item.ItemLevel == level);

        if (candidateItems.Count > 0)
        {
            ItemScriptableObject item = candidateItems[Random.Range(0, candidateItems.Count)];
            item.Start();
            return item.Item;
        }
        return GetArtifact(level - 1);
    }
}

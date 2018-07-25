using UnityEngine;

/// <summary>
/// Describes the stats and attributes of the Destructible
/// </summary>
[CreateAssetMenu(menuName = "Custom/Destructible")]
public class DestructibleScriptableObject : ScriptableObject {

    [Range(0, 100)]
    public int maxHealth;
    [Range(0, 100)]
    public int defence;
    public ItemScriptableObject[] potentialItemDrops;
    public Sprite[] sprites;
    public Color particleDamagedColor;
}

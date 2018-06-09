using UnityEngine;

/// <summary>
/// Holds info for an upgrade in the upgrades tree
/// </summary>
[CreateAssetMenu(menuName = "Custom/Upgrade", fileName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public string name;
    public int maxHealthMultiplier = 1;
    public int attackMultiplier = 1;
    public int defenseMultiplier = 1;
    public int maxFocusMultiplier = 1;
    public int hitSpeedMultiplier = 1;
    public Sprite upgradeSprite;
}

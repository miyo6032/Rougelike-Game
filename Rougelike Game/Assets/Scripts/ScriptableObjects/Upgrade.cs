using UnityEngine;

/// <summary>
/// Holds info for an upgrade in the upgrades tree
/// </summary>
[CreateAssetMenu(menuName = "Custom/Upgrade", fileName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public new string name;
    public int maxHealthMultiplier = 0;
    public int attackMultiplier = 0;
    public int defenseMultiplier = 0;
    public int maxFocusMultiplier = 0;
    public int hitSpeedMultiplier = 0;
    public Sprite upgradeSprite;
    public Sprite rimSprite;
    public Sprite unlockedRimSprite;

    public Upgrade(string name, Sprite upgradeSprite, Sprite rimSprite, Sprite unlockedRimSprite)
    {
        this.name = name;
        this.upgradeSprite = upgradeSprite;
        this.rimSprite = rimSprite;
        this.unlockedRimSprite = unlockedRimSprite; 
    }

    public Upgrade(){}

}

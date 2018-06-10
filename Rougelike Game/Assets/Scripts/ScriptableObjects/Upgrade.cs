using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Holds info for an upgrade in the upgrades tree
/// </summary>
[CreateAssetMenu(menuName = "Custom/Upgrade", fileName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public new string name;
    public Stat[] statsAffected;
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

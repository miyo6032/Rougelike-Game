using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum Stats
{
    maxHealth,
    baseAttack,
    maxFocus,
    hitSpeed,
    baseDefense,
    damage,
    healing
}

[System.Serializable]
public class Stat
{
    public Stats stat;
    public float effect;
}

/// <summary>
/// Keeps track of the player's game status, and holds status functions for updating stats.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    // The player's maximum health that is fixed at the start of every game
    public int maxHealth;

    // The player's attack power - is the base damage to an enemy before other caluclations are added in
    public int minAttack;
    public int maxAttack;
    private int baseAttack;

    // The maximum amount of focus a player can store at one time
    public int maxFocus;

    // The speed that a player can it - influenced by strength and weapon weight
    public float hitSpeed;

    // The player's in-game health that is updated regularly
    private int health;

    // The player's defense - used in the damage calculation when the player is hit
    private int totalDefense;
    private int baseDefense;

    // Just standard rpg experience, when the player has enough they will level up
    private int experience;
    private int maxExperience = 50;

    // Leveling up will allow the player to choose upgrades from the skill tree and also improve the base stats by a little bit
    private int level = 1;

    // The upgrade points for a player after leveling up
    public int upgradePoints = 5;

    // The player's focus bar - used for special skills
    private int focus;

    Animator damageCounter;
    Animator animator;
    PlayerAnimation playerAnimation;
    Text damageText;

    void Start()
    {
        animator = GetComponent<Animator>();
        damageCounter = HelperScripts.GetComponentFromChildrenExc<Animator>(transform);
        damageText = HelperScripts.GetComponentFromChildrenExc<Text>(transform);
        health = maxHealth;
        UpdateStats();
        StaticCanvasList.instance.statUI.UpdateStatUI(level, experience, maxExperience, health,maxHealth, focus, maxFocus, totalDefense, minAttack, maxAttack);
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    /// <summary>
    /// Add xp and handle leveling up
    /// </summary>
    /// <param name="xp"></param>
    public void AddXP(int xp)
    {
        experience += xp;
        if (experience >= maxExperience)
        {
            // Upgrades points granted at level 30: 5
            upgradePoints += Mathf.CeilToInt(level/6f);
            level++;
            experience = experience - maxExperience;
            maxExperience += Mathf.CeilToInt(maxExperience * 0.2f);
        }
        UpdateStats();
    }

    public int GetLevel()
    {
        return level;
    }

    public void UseUpgradePoint()
    {
        upgradePoints--;
        UpdateStats();
    }

    public int GetUpgradePoints()
    {
        return upgradePoints;
    }

    /// <summary>
    /// Damage the player, generate the damage counter, and update the health ui
    /// </summary>
    /// <param name="damage"></param>
    public void DamagePlayer(int damage)
    {
        damage = Mathf.Clamp(damage - totalDefense, 0, damage);
        DamagePlayerDirectly(damage);
    }

    /// <summary>
    /// Bypasses armor and damages player
    /// </summary>
    public void DamagePlayerDirectly(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, health);
        damageCounter.SetTrigger("damage");
        animator.SetTrigger("damage");
        damageText.text = "" + damage;
        StaticCanvasList.instance.gameUI.UpdateHealth(health / (float)maxHealth * 100);
        UpdateStats();
    }

    /// <summary>
    /// Heal the player by some amount
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        StaticCanvasList.instance.gameUI.UpdateHealth(health / (float) maxHealth * 100);
        UpdateStats();
    }

    /// <summary>
    /// Called by slots when equipping an item
    /// </summary>
    /// <param name="inst"></param>
    /// <param name="slot"></param>
    /// <returns></returns>
    public bool EquipItem(ItemInstance inst, EquipSlot slot)
    {
        // If the item has the correct stats to equip
        if (level >= inst.item.ItemLevel)
        {
            inst.equipped = true;
            UpdateStats();
            playerAnimation.ColorAnimator(slot.gameObject.name, inst.item.ItemColor);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Unequip an item when it is removed from the equipment slot
    /// </summary>
    /// <param name="inst"></param>
    public void UnequipItem(ItemInstance inst)
    {
        inst.equipped = false;
        inst.slot.GetComponent<EquipSlot>().SlotImageToEmpty();
        UpdateStats();
    }

    /// <summary>
    /// Update the player's stats by going over all the equipped items and summing their stats
    /// </summary>
    public void UpdateStats()
    {
        totalDefense = baseDefense;
        minAttack = baseAttack;
        maxAttack = baseAttack;

        // Sum all of the equipment stats
        foreach (EquipSlot slot in StaticCanvasList.instance.inventoryManager.equipSlots)
        {
            if (slot.GetItem() != null)
            {
                Item equippedItem = slot.GetItem().item;
                totalDefense += equippedItem.Defence;
                minAttack += equippedItem.Attack;
                maxAttack += equippedItem.MaxAttack;
            }
        }

        StaticCanvasList.instance.statUI.UpdateStatUI(level, experience, maxExperience, health, maxHealth, focus, maxFocus, totalDefense, minAttack, maxAttack);
        StaticCanvasList.instance.skillTree.upgradePointsText.text = "Availible Upgrade Points: " + upgradePoints;
    }

    /// <summary>
    /// Adds a new upgrade to the player
    /// </summary>
    /// <param name="upgrade"></param>
    public void AddUpgrade(Upgrade upgrade)
    {
        ApplyStats(upgrade.statsAffected);
        UpdateStats();
    }

    /// <summary>
    /// Apply a specific stat to the playerStats
    /// </summary>
    /// <param name="stat"></param>
    private void ApplyStat(Stat stat)
    {
        switch (stat.stat)
        {
            case Stats.maxHealth:
                maxHealth += Mathf.RoundToInt(stat.effect);
                break;
            case Stats.baseAttack:
                baseAttack += Mathf.RoundToInt(stat.effect);
                break;
            case Stats.baseDefense:
                baseDefense += Mathf.RoundToInt(stat.effect);
                break;
            case Stats.hitSpeed:
                hitSpeed += stat.effect;
                break;
            case Stats.maxFocus:
                maxFocus += Mathf.RoundToInt(stat.effect);
                break;
            case Stats.damage:
                DamagePlayerDirectly(Mathf.RoundToInt(stat.effect));
                break;
            case Stats.healing:
                Heal(Mathf.RoundToInt(stat.effect));
                break;
        }
    }

    /// <summary>
    /// Apply a bunch of stats to the playerstat
    /// </summary>
    /// <param name="stats"></param>
    public void ApplyStats(Stat[] stats)
    {
        foreach (var stat in stats)
        {
            ApplyStat(stat);
        }
        UpdateStats();
    }

    /// <summary>
    /// Undo effects of a stat (basically apply -1 * stat)
    /// </summary>
    /// <param name="stats"></param>
    public void ReverseStats(Stat[] stats)
    {
        foreach (var stat in stats)
        {
            stat.effect = -stat.effect;
            ApplyStat(stat);
            stat.effect = -stat.effect;
        }
        UpdateStats();
    }
}

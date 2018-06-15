using UnityEngine;
using UnityEngine.UI;

public enum ModifierType
{
    maxHealth,
    attack,
    maxFocus,
    hitSpeed,
    defense,
    damage,
    healing
}

[System.Serializable]
public class Modifier
{
    public ModifierType ModifierType;
    public int value;
}

/// <summary>
/// Keeps track of the player's game status, and holds status functions for updating stats.
/// </summary>
public class PlayerStats : Stats
{
    public Stat hitDelay;

    // Just standard rpg experience, when the player has enough they will level up
    private int experience;
    private int maxExperience = 50;

    // Leveling up will allow the player to choose upgrades from the skill tree and also improve the base stats by a little bit
    private int level = 1;

    // The upgrade points for a player after leveling up
    public int upgradePoints = 5;

    // The player's focus bar - used for special skills
    private int focus;
    public Stat maxFocus;

    Animator damageCounter;
    Animator animator;
    PlayerAnimation playerAnimation;
    Text damageText;

    void Start()
    {
        animator = GetComponent<Animator>();
        damageCounter = HelperScripts.GetComponentFromChildrenExc<Animator>(transform);
        damageText = HelperScripts.GetComponentFromChildrenExc<Text>(transform);
        health = maxHealth.GetIntValue();
        UpdateStats();
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
    public override void TakeDamage(int damage)
    {
        damage = Mathf.Clamp(damage - defense.GetIntValue(), 0, damage);
        DamagePlayerDirectly(damage);
    }

    /// <summary>
    /// Bypasses armor and damages player
    /// </summary>
    public void DamagePlayerDirectly(int damage)
    {
        base.TakeDamage(damage);
        damageCounter.SetTrigger("damage");
        animator.SetTrigger("damage");
        damageText.text = "" + damage;
        UpdateStats();
    }

    public override void Heal(int amount)
    {
        base.Heal(amount);
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
            maxAttack.AddModifier(inst.item.MaxAttack, inst);
            minAttack.AddModifier(inst.item.Attack, inst);
            defense.AddModifier(inst.item.Defence, inst);
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
        maxAttack.RemoveSource(inst);
        minAttack.RemoveSource(inst);
        defense.RemoveSource(inst);
        inst.equipped = false;
        inst.slot.GetComponent<EquipSlot>().SlotImageToEmpty();
        UpdateStats();
    }

    /// <summary>
    /// Update the player's stats by going over all the equipped items and summing their stats
    /// </summary>
    public void UpdateStats()
    {
        StaticCanvasList.instance.gameUI.UpdateHealth(health / (float)maxHealth.GetValue());
        StaticCanvasList.instance.statUI.UpdateStatUI(level, experience, maxExperience, health, maxHealth.GetIntValue(), focus, maxFocus.GetIntValue(), defense.GetIntValue(), minAttack.GetIntValue(), maxAttack.GetIntValue());
        StaticCanvasList.instance.skillTree.upgradePointsText.text = "Availible Upgrade Points: " + upgradePoints;
    }

    /// <summary>
    /// Adds a new upgrade to the player
    /// </summary>
    /// <param name="upgrade"></param>
    public void AddUpgrade(Upgrade upgrade)
    {
        ApplyStats(upgrade.ModifiersAffected);
        UpdateStats();
    }

    /// <summary>
    /// Apply a specific ModifierType to the playerStats
    /// </summary>
    /// <param name="modifier"></param>
    private void ApplyStat(Modifier modifier)
    {
        switch (modifier.ModifierType)
        {
            case ModifierType.maxHealth:
                maxHealth.AddModifier(Mathf.RoundToInt(modifier.value));
                Heal(Mathf.RoundToInt(modifier.value));
                break;
            case ModifierType.attack:
                minAttack.AddModifier(Mathf.RoundToInt(modifier.value));
                maxAttack.AddModifier(Mathf.RoundToInt(modifier.value));
                break;
            case ModifierType.defense:
                defense.AddModifier(Mathf.RoundToInt(modifier.value));
                break;
            case ModifierType.hitSpeed:
                hitDelay.AddModifier(modifier.value);
                break;
            case ModifierType.maxFocus:
                maxFocus.AddModifier(Mathf.RoundToInt(modifier.value));
                break;
            case ModifierType.damage:
                DamagePlayerDirectly(Mathf.RoundToInt(modifier.value));
                break;
            case ModifierType.healing:
                Heal(Mathf.RoundToInt(modifier.value));
                break;
        }
    }

    /// <summary>
    /// Apply a bunch of stats to the playerstat
    /// </summary>
    /// <param name="modifiers"></param>
    public void ApplyStats(Modifier[] modifiers)
    {
        foreach (var stat in modifiers)
        {
            ApplyStat(stat);
        }
        UpdateStats();
    }

    /// <summary>
    /// Undo effects of a ModifierType (basically apply -1 * ModifierType)
    /// </summary>
    /// <param name="modifiers"></param>
    public void ReverseStats(Modifier[] modifiers)
    {
        foreach (var stat in modifiers)
        {
            stat.value = -stat.value;
            ApplyStat(stat);
            stat.value = -stat.value;
        }
        UpdateStats();
    }
}

using EZCameraShake;
using UnityEngine;

public enum ModifierType
{
    maxHealth,
    attack,
    maxFocus,
    hitSpeed,
    defense,
    damage,
    healing,
    movementDelay
}

[System.Serializable]
public class Modifier
{
    public ModifierType ModifierType;
    public float value;
}

/// <summary>
/// Keeps track of the player's game status, and holds status functions for updating stats.
/// </summary>
public class PlayerStats : Stats
{
    public static PlayerStats instance;
    public Stat hitDelay;

    // Just standard rpg experience, when the player has enough they will level up
    private int experience;
    private int maxExperience = 50;

    // Leveling up will allow the player to choose upgrades from the skill tree and also improve the base stats by a little bit
    private int level = 1;

    // The upgrade points for a player after leveling up
    public int upgradePoints = 5;

    // The player's focus bar - used for special skills
    public float focus;
    public Stat maxFocus;

    public DamageCounter damageCounterPrefab;

    public ItemScriptableObject meat;
    public ItemScriptableObject starterSword;
    public ItemScriptableObject starterHelmet;
    public ItemScriptableObject starterArmor;

    private Animator animator;
    private PlayerAnimation playerAnimation;
    private SoundManager soundManager;

    void Start()
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
        animator = GetComponent<Animator>();
        health = maxHealth.GetIntValue();
        focus = maxFocus.GetIntValue();
        UpdateStats();
        playerAnimation = GetComponent<PlayerAnimation>();
        soundManager = GetComponent<SoundManager>();

        InventoryManager.instance.AddItem(new ItemStack(meat.item, 1));
        InventoryManager.instance.AddItemToSlot(new ItemStack(starterSword.item, 1), InventoryManager.instance.equipSlots[0]);
        InventoryManager.instance.AddItemToSlot(new ItemStack(starterSword.item, 1), InventoryManager.instance.equipSlots[1]);
        InventoryManager.instance.AddItemToSlot(new ItemStack(starterHelmet.item, 1), InventoryManager.instance.equipSlots[3]);
        InventoryManager.instance.AddItemToSlot(new ItemStack(starterArmor.item, 1), InventoryManager.instance.equipSlots[2]);

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

    public void ChangeFocus(int amount)
    {
        focus = Mathf.Clamp(focus + amount, 0, maxFocus.GetValue());
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
        ApplyDamageEffects(damage);
    }

    /// <summary>
    /// Applies damage effect like camera shake and sound
    /// </summary>
    /// <param name="damage"></param>
    void ApplyDamageEffects(int damage)
    {
        if (damage >= maxHealth.GetValue() / 6f)
        {
            CameraShaker.Instance.ShakeOnce(1f, 1f, 0.0f, 0.3f);
            soundManager.PlayRandomizedPitch(SoundDatabase.Instance.PlayerHighAttack);
        }
        else
        {
            soundManager.PlayRandomizedPitch(SoundDatabase.Instance.PlayerAttack);
        }
    }

    /// <summary>
    /// Bypasses armor and damages player
    /// </summary>
    public void DamagePlayerDirectly(int damage)
    {
        base.TakeDamage(damage);
        DamageCounter instance = Instantiate(damageCounterPrefab, transform);
        instance.SetText(damage.ToString());
        animator.SetTrigger("damage");
        UpdateStats();
    }

    public override void Heal(float amount)
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
    public bool EquipItem(ItemStack inst, EquipSlot slot)
    {
        // If the item has the correct stats to equip
        if (level >= inst.item.ItemLevel)
        {
            maxAttack.AddModifier(inst.item.MaxAttack, inst);
            minAttack.AddModifier(inst.item.Attack, inst);
            defense.AddModifier(inst.item.Defence, inst);
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
    public void UnequipItem(ItemStack inst)
    {
        maxAttack.RemoveSource(inst);
        minAttack.RemoveSource(inst);
        defense.RemoveSource(inst);
        UpdateStats();
    }

    /// <summary>
    /// Update the player's stats by going over all the equipped items and summing their stats
    /// </summary>
    public void UpdateStats()
    {
        InGameUI.instance.UpdateHealth(health / maxHealth.GetValue());
        InGameUI.instance.UpdateFocus(focus / maxFocus.GetValue());
        PlayerStatUI.instance.UpdateStatUI(level, experience, maxExperience, (int)health, maxHealth.GetIntValue(), Mathf.RoundToInt(focus), maxFocus.GetIntValue(), defense.GetIntValue(), minAttack.GetIntValue(), maxAttack.GetIntValue());
        SkillTree.instance.upgradePointsText.text = "Availible Upgrade Points: " + upgradePoints;
    }

    /// <summary>
    /// Adds a new upgrade to the player
    /// </summary>
    /// <param name="upgrade"></param>
    public void AddUpgrade(Upgrade upgrade)
    {
        ApplyStats(upgrade.ModifiersAffected, null);
        UpdateStats();
    }

    /// <summary>
    /// Apply a specific ModifierType to the playerStats
    /// </summary>
    /// <param name="modifier"></param>
    private void ApplyStat(Modifier modifier, object source)
    {
        switch (modifier.ModifierType)
        {
            case ModifierType.maxHealth:
                maxHealth.AddModifier(Mathf.RoundToInt(modifier.value), source);
                Heal(Mathf.RoundToInt(modifier.value));
                break;
            case ModifierType.attack:
                minAttack.AddModifier(Mathf.RoundToInt(modifier.value), source);
                maxAttack.AddModifier(Mathf.RoundToInt(modifier.value), source);
                break;
            case ModifierType.defense:
                defense.AddModifier(Mathf.RoundToInt(modifier.value), source);
                break;
            case ModifierType.hitSpeed:
                hitDelay.AddModifier(modifier.value, source);
                break;
            case ModifierType.maxFocus:
                maxFocus.AddModifier(Mathf.RoundToInt(modifier.value), source);
                break;
            case ModifierType.damage:
                DamagePlayerDirectly(Mathf.RoundToInt(modifier.value));
                break;
            case ModifierType.healing:
                Heal(modifier.value);
                break;
            case ModifierType.movementDelay:
                movementDelay.AddModifier(modifier.value, source);
                break;
        }
    }

    /// <summary>
    /// Removes a modifier by source
    /// </summary>
    private void RemoveStat(Modifier modifier, object source)
    {
        switch (modifier.ModifierType)
        {
            case ModifierType.maxHealth:
                maxHealth.RemoveSource(source);
                break;
            case ModifierType.attack:
                minAttack.RemoveSource(source);
                maxAttack.RemoveSource(source);
                break;
            case ModifierType.defense:
                defense.RemoveSource(source);
                break;
            case ModifierType.hitSpeed:
                hitDelay.RemoveSource(source);
                break;
            case ModifierType.maxFocus:
                maxFocus.RemoveSource(source);
                break;
            case ModifierType.damage:
                Heal(modifier.value);
                break;
            case ModifierType.healing:
                DamagePlayerDirectly(Mathf.RoundToInt(modifier.value));
                break;
            case ModifierType.movementDelay:
                movementDelay.RemoveSource(source);
                break;
        }
    }

    /// <summary>
    /// Apply a bunch of stats to the playerstat
    /// </summary>
    /// <param name="modifiers"></param>
    public void ApplyStats(Modifier[] modifiers, object source)
    {
        foreach (var stat in modifiers)
        {
            ApplyStat(stat, source);
        }
        UpdateStats();
    }

    /// <summary>
    /// Undo effects of a ModifierType (basically apply -1 * ModifierType)
    /// </summary>
    /// <param name="modifiers"></param>
    public void RemoveStats(Modifier[] modifiers, object source)
    {
        foreach (var stat in modifiers)
        {
            RemoveStat(stat, source);
        }
        UpdateStats();
    }
}

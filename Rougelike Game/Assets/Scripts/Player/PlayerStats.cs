using EZCameraShake;
using UnityEngine;

public enum PlayerStatModifier
{
    maxHealth,
    minAttack,
    maxAttack,
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
    public PlayerStatModifier statToModify;
    public StatModifierType modifierType;
    public float value;

    public Modifier(PlayerStatModifier statToModify, StatModifierType modifierType, float value)
    {
        this.statToModify = statToModify;
        this.modifierType = modifierType;
        this.value = value;
    }
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
    public ItemScriptableObject starterShield;
    public ItemScriptableObject starterHelmet;
    public ItemScriptableObject starterChestplate;
    public ItemScriptableObject starterLeggings;
    public ItemScriptableObject starterBoots;
    public ItemScriptableObject starterNecklace;
    public ItemScriptableObject startedRing;

    private Animator animator;
    private PlayerAnimation playerAnimation;
    private SoundManager soundManager;

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
        animator = GetComponent<Animator>();
        health = maxHealth.GetIntValue();
        focus = maxFocus.GetIntValue();
        UpdateStats();
        playerAnimation = GetComponent<PlayerAnimation>();
        soundManager = GetComponent<SoundManager>();

        InventoryManager.instance.AddItem(new ItemStack(meat.item, 1));
        InventoryManager.instance.AddItemToSlot(new ItemStack(starterSword.item, 1), InventoryManager.instance.equipSlots[0]);
        InventoryManager.instance.AddItemToSlot(new ItemStack(starterShield.item, 1), InventoryManager.instance.equipSlots[1]);
        InventoryManager.instance.AddItemToSlot(new ItemStack(starterHelmet.item, 1), InventoryManager.instance.equipSlots[2]);
        InventoryManager.instance.AddItemToSlot(new ItemStack(starterChestplate.item, 1), InventoryManager.instance.equipSlots[3]);
        InventoryManager.instance.AddItemToSlot(new ItemStack(starterLeggings.item, 1), InventoryManager.instance.equipSlots[4]);
        InventoryManager.instance.AddItemToSlot(new ItemStack(starterBoots.item, 1), InventoryManager.instance.equipSlots[5]);
        InventoryManager.instance.AddItemToSlot(new ItemStack(starterNecklace.item, 1), InventoryManager.instance.equipSlots[6]);
        InventoryManager.instance.AddItemToSlot(new ItemStack(startedRing.item, 1), InventoryManager.instance.equipSlots[7]);
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
            upgradePoints += Mathf.CeilToInt(level / 6f);
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
    private void ApplyDamageEffects(int damage)
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
    public bool EquipItem(ItemStack inst)
    {
        // If the item has the correct stats to equip
        if (level >= inst.item.ItemLevel)
        {
            ApplyStats(inst.item.equipmentModifiers, inst);
            UpdateStats();
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
        RemoveStats(inst.item.equipmentModifiers, inst);
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
    private void ApplyStat(Modifier modifier, object source)
    {
        switch (modifier.statToModify)
        {
            case PlayerStatModifier.maxHealth:
                maxHealth.AddModifier(Mathf.RoundToInt(modifier.value), source, modifier.modifierType);
                if (modifier.modifierType == StatModifierType.linear)
                {
                    Heal(Mathf.RoundToInt(modifier.value));
                }
                health = Mathf.Clamp(health, 0, maxHealth.GetValue());
                break;

            case PlayerStatModifier.minAttack:
                minAttack.AddModifier(Mathf.RoundToInt(modifier.value), source, modifier.modifierType);
                break;

            case PlayerStatModifier.maxAttack:
                maxAttack.AddModifier(Mathf.RoundToInt(modifier.value), source, modifier.modifierType);
                break;

            case PlayerStatModifier.defense:
                defense.AddModifier(Mathf.RoundToInt(modifier.value), source, modifier.modifierType);
                break;

            case PlayerStatModifier.hitSpeed:
                hitDelay.AddModifier(modifier.value, source, modifier.modifierType);
                break;

            case PlayerStatModifier.maxFocus:
                maxFocus.AddModifier(Mathf.RoundToInt(modifier.value), source, modifier.modifierType);
                break;

            case PlayerStatModifier.damage:
                DamagePlayerDirectly(Mathf.RoundToInt(modifier.value));
                break;

            case PlayerStatModifier.healing:
                Heal(modifier.value);
                break;

            case PlayerStatModifier.movementDelay:
                movementDelay.AddModifier(modifier.value, source, modifier.modifierType);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Removes a modifier by source
    /// </summary>
    private void RemoveStat(Modifier modifier, object source)
    {
        switch (modifier.statToModify)
        {
            case PlayerStatModifier.maxHealth:
                maxHealth.RemoveSource(source);
                break;

            case PlayerStatModifier.minAttack:
                minAttack.RemoveSource(source);
                break;

            case PlayerStatModifier.maxAttack:
                maxAttack.RemoveSource(source);
                break;

            case PlayerStatModifier.defense:
                defense.RemoveSource(source);
                break;

            case PlayerStatModifier.hitSpeed:
                hitDelay.RemoveSource(source);
                break;

            case PlayerStatModifier.maxFocus:
                maxFocus.RemoveSource(source);
                break;

            case PlayerStatModifier.damage:
                Heal(modifier.value);
                break;

            case PlayerStatModifier.healing:
                DamagePlayerDirectly(Mathf.RoundToInt(modifier.value));
                break;

            case PlayerStatModifier.movementDelay:
                movementDelay.RemoveSource(source);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Apply a bunch of stats to the playerstat
    /// </summary>
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
    public void RemoveStats(Modifier[] modifiers, object source)
    {
        foreach (var stat in modifiers)
        {
            RemoveStat(stat, source);
        }
        UpdateStats();
    }
}
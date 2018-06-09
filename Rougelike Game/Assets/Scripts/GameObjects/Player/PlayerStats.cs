using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Keeps track of the player's game status, and holds status functions for updating stats.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    // The player's in-game health that is updated regularly
    private int health;

    // The player's maximum health that is fixed at the start of every game
    public int maxHealth;

    // The player's attack power - is the base damage to an enemy before other caluclations are added in
    public int minAttack;
    public int maxAttack;

    // The player's defense - used in the damage calculation when the player is hit
    private int defence;

    // Just standard rpg experience, when the player has enough they will level up
    private int experience;

    // Leveling up will allow the player to choose upgrades from the skill tree and also improve the base stats by a little bit
    private int level = 10;

    // The player's focus bar - used for special skills
    private int focus;

    // The maximum amount of focus a player can store at one time
    public int maxFocus;

    // The speed that a player can it - influenced by strength and weapon weight
    public float hitSpeed;
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
        StaticCanvasList.instance.statUI.UpdateStatUI(level, experience, health, focus, defence, minAttack, maxAttack);
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    public int GetLevel()
    {
        return level;
    }

    /// <summary>
    /// Damage the player, generate the damage counter, and update the health ui
    /// </summary>
    /// <param name="damage"></param>
    public void DamagePlayer(int damage)
    {
        damage = Mathf.Clamp(damage - defence, 0, damage);
        health = Mathf.Clamp(health - damage, 0, health);
        damageCounter.SetTrigger("damage");
        animator.SetTrigger("damage");
        damageText.text = "" + damage;
        StaticCanvasList.instance.gameUI.UpdateHealth(health / (float) maxHealth * 100);
    }

    /// <summary>
    /// Heal the player by some amount
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        StaticCanvasList.instance.gameUI.UpdateHealth(health / (float) maxHealth * 100);
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
        defence = 0;
        minAttack = 0;
        maxAttack = 0;

        // Sum all of the equipment stats
        foreach (EquipSlot slot in StaticCanvasList.instance.inventoryManager.equipSlots)
        {
            if (slot.GetItem() != null)
            {
                Item equippedItem = slot.GetItem().item;
                defence += equippedItem.Defence;
                minAttack += equippedItem.Attack;
                maxAttack += equippedItem.MaxAttack;
            }
        }

        StaticCanvasList.instance.statUI.UpdateStatUI(level, experience, maxHealth, maxFocus, defence, minAttack,
            maxAttack);
    }

    /// <summary>
    /// Adds a new upgrade to the player
    /// </summary>
    /// <param name="upgrade"></param>
    public void AddUpgrade(Upgrade upgrade)
    {
        this.maxHealth += upgrade.maxHealthMultiplier;
        this.minAttack += upgrade.attackMultiplier;
        this.maxAttack += upgrade.attackMultiplier;
        this.defence += upgrade.defenseMultiplier;
        this.maxFocus += upgrade.maxFocusMultiplier;
        this.hitSpeed += upgrade.hitSpeedMultiplier;
        UpdateStats();
    }
}

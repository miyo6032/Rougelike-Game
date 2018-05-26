using UnityEngine;
using UnityEngine.UI;

//Keeps track of the player's game status, and holds status functions for updating stats.
//Will link in between the inventory and equipment as well
//Also handles some ui for damage and stuff
public class PlayerStats : MonoBehaviour {

    private int health; //The player's in-game health that is updated regularly
    public int maxHealth; //The player's maximum health that is fixed at the start of every game
    public int minAttack; //The player's attack power - is the base damage to an enemy before other caluclations are added in
    public int maxAttack;
    private int defence; //The player's defense - used in the damage calculation when the player is hit
    //Influenced by gear
    private int experience; //Just standard rpg experience, when the player has enough they will level up
    private int level = 10; //Determines only the amount of experience needed for the next level
    //Leveling up will allow the player to choose upgrades from the skill tree and also improve the base stats by a little bit
    private int focus; //The player's focus bar - used for special skills
    public int maxFocus; //The maximum amount of focus a player can store at one time
    public float hitSpeed; //The speed that a player can it - influenced by strength and weapon weight

    Animator damageCounter;
    PlayerAnimation playerAnimation;
    Text damageText;

    void Start()
    {
        damageCounter = HelperScripts.GetComponentFromChildrenExc<Animator>(transform);
        damageText = HelperScripts.GetComponentFromChildrenExc<Text>(transform);
        health = maxHealth;
        UpdateEquipStats();
        StaticCanvasList.instance.statUI.UpdateStatUI(level, experience, health, focus, defence, minAttack, maxAttack);
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    public int GetLevel()
    {
        return level;
    }

    //Damage the player, generate the damage counter, and update the health ui
    public void DamagePlayer(int damage)
    {
        damage = Mathf.Clamp(damage - defence, 0, damage);
        health = Mathf.Clamp(health - damage, 0, health);
        damageCounter.SetTrigger("damage");
        damageText.text = "" + damage;
        StaticCanvasList.instance.gameUI.UpdateHealth(health / (float)maxHealth * 100);
    }

    //Called by slots when equipping an item. 
    //If the item is being replaced, then there is no need to call
    //unequip item because the equip item simply replaces it.
    public bool EquipItem(ItemInstance inst, EquipSlot slot)
    {
        //If the item has the correct stats to equip
        if(level >= inst.item.ItemLevel)
        {
            inst.equipped = true;
            UpdateEquipStats();
            playerAnimation.ColorAnimator(inst.item.EquippedSlot, inst.item.ItemColor);
            return true;
        }
        return false;
    }

    public void UnequipItem(ItemInstance inst)
    {
        inst.equipped = false;
        inst.slot.GetComponent<EquipSlot>().SlotImageToEmpty();
        UpdateEquipStats();
    }

    //Update the player's stats by going over all the equipped items and summing their stats
    public void UpdateEquipStats()
    {
        this.defence = 0;
        this.minAttack = 0;
        this.maxAttack = 0;

        //Sum all of the equipment stats
        foreach(EquipSlot slot in StaticCanvasList.instance.inventoryManager.equipSlots)
        {
            if (slot.item != null)
            {
                Item equippedItem = slot.item.item;
                defence += equippedItem.Defence;
                minAttack += equippedItem.Attack;
                maxAttack += equippedItem.MaxAttack;
            }
        }

        StaticCanvasList.instance.statUI.UpdateStatUI(level, experience, health, focus, defence, minAttack, maxAttack);

    }

}

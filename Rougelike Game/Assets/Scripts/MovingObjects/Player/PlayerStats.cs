using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//Keeps track of the player's game status, and holds status functions for updating stats.
//Will link in between the inventory and equipment as well
//Also handles some ui for damage and stuff
public class PlayerStats : MonoBehaviour {

    public int health; //The player's in-game health that is updated regularly
    public int maxHealth; //The player's maximum health that is fixed at the start of every game
    public Vector2Int attack; //The player's attack power - is the base damage to an enemy before other caluclations are added in
    //The vector2 represents minimum attack and maximum attack
    public int defence; //The player's defense - used in the damage calculation when the player is hit
    //Influenced by gear
    public int experience; //Just standard rpg experience, when the player has enough they will level up
    public int level; //Determines only the amount of experience needed for the next level
    //Leveling up will allow the player to choose upgrades from the skill tree and also improve the base stats by a little bit
    public int focus; //The player's focus bar - used for special skills
    public int maxFocus; //The maximum amount of focus a player can store at one time
    public float hitSpeed; //The speed that a player can it - influenced by strength and weapon weight
    public int strength; //Influences hit speed and attack

    Animator damageCounter;
    Text damageText;

    void Start()
    {
        damageCounter = HelperScripts.GetComponentFromChildrenExc<Animator>(transform);
        damageText = HelperScripts.GetComponentFromChildrenExc<Text>(transform);
    }

    //Damage the player, generate the damage counter, and update the health ui
    public void DamagePlayer(int damage)
    {
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
            return true;
        }
        return false;
    }

    public void UnequipItem(ItemInstance inst)
    {
        inst.equipped = false;
        UpdateEquipStats();
    }

    //Update the player's stats by going over all the equipped items and summing their stats
    public void UpdateEquipStats()
    {
        this.defence = 0;
        this.attack = new Vector2Int(1, 1);

        //Sum all of the equipment stats
        foreach(EquipSlot slot in StaticCanvasList.instance.inventoryManager.equipSlots)
        {
            if (slot.item != null)
            {
                Item equippedItem = slot.item.item;
                defence += equippedItem.Defence;
                attack += new Vector2Int(equippedItem.Attack, equippedItem.MaxAttack);
            }
        }

        StaticCanvasList.instance.statUI.UpdateStatUI(level, experience, health, focus, defence, attack);

    }

}

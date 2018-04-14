using UnityEngine;
using UnityEngine.UI;

//Keeps track of the player's game status, and holds status functions for updating stats.
//Will link in between the inventory and equipment as well
//Also handles some ui for damage and stuff
public class PlayerStats : MonoBehaviour {

    public int health; //The player's in-game health that is updated regularly
    public int maxHealth; //The player's maximum health that is fixed at the start of every game
    public int attack; //The player's attack power - is the base damage to an enemy before other caluclations are added in
    //Influenced by strength and gear
    public int defence; //The player's defense - used in the damage calculation when the player is hit
    //Influenced by gear
    public int experience; //Just standard rpg experience, when the player has enough they will level up
    public int level; //Determines only the amount of experience needed for the next level
    //Leveling up will allow the player to choose upgrades from the skill tree and also improve the base stats by a little bit
    public int focus; //The player's focus bar - used for special skills
    public int maxFocus; //The maximum amount of focus a player can store at one time
    public float hitSpeed; //The speed that a player can it - influenced by strength and weapon weight
    public int strength; //Influences hit speed and attack
    public InGameUI inGameUI;

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
        inGameUI.UpdateHealth(health / (float)maxHealth * 100);
    }

}

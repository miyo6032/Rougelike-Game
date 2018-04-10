using UnityEngine;

//Keeps track of the player's game status, and holds status functions for updating stats.
//Will link in between the inventory and equipment as well
public class PlayerStats : MonoBehaviour {

    public int health;
    public int attack;
    public int defence;
    public int experience;
    public int level;
    public int focus;
    public int hitSpeed;

    Animator damageCounter;
    
    void Start()
    {
        damageCounter = HelperScripts.GetComponentFromChildrenExc<Animator>(transform);
    }

    public void DamagePlayer(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, health);
        damageCounter.SetTrigger("damage");
    }

}

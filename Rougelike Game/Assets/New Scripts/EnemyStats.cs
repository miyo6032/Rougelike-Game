using UnityEngine;

//Keeps track of enemy stats like attack and health - nothing too special
public class EnemyStats : MonoBehaviour {

    public int attack;
    public int health;

    Animator damageCounter;

    void Start()
    {
        damageCounter = HelperScripts.GetComponentFromChildrenExc<Animator>(transform);
    }

    public void DamageEnemy(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, health);
        damageCounter.SetTrigger("damage");
    }

}

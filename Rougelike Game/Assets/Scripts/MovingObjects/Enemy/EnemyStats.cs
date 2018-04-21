using UnityEngine;
using UnityEngine.UI;

//Keeps track of enemy stats like attack and health - nothing too special
public class EnemyStats : MonoBehaviour {

    public int attack;
    public int health;
    public int maxHealth;

    Animator damageCounter;
    Text damageText;
    Slider healthSlider;

    void Start()
    {
        damageCounter = HelperScripts.GetComponentFromChildrenExc<Animator>(transform);
        damageText = HelperScripts.GetComponentFromChildrenExc<Text>(transform);
        healthSlider = HelperScripts.GetComponentFromChildrenExc<Slider>(transform);
    }

    //Damage the enemy, generate the damage counter, and update the health ui
    public void DamageEnemy(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, health);
        damageCounter.SetTrigger("damage");
        damageText.text = "" + damage;
        healthSlider.value = health / (float)maxHealth * 100;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

}

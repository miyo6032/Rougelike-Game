using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//Keeps track of enemy stats like attack and health - nothing too special
public class EnemyStats : MonoBehaviour {

    public int minAttack;
    public int maxAttack;
    private int health;
    public int maxHealth;
    public int level;

    public LootBag lootBagPrefab;
    public LayerMask bagLayerMask;
    public Vector2Int dropRange = new Vector2Int(0, 3);

    public Animator damageCounter;
    Animator animator;
    Text damageText;
    Slider healthSlider;

    void Start()
    {
        damageText = HelperScripts.GetComponentFromChildrenExc<Text>(transform);
        healthSlider = HelperScripts.GetComponentFromChildrenExc<Slider>(transform);
        animator = GetComponent<Animator>();
        health = maxHealth;
    }

    //Damage the enemy, generate the damage counter, and update the health ui
    public void DamageEnemy(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, health);
        damageCounter.SetTrigger("damage");
        animator.SetTrigger("damage");
        damageText.text = "" + damage;
        healthSlider.value = health / (float)maxHealth * 100;
        if(health <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        List<ItemSave> itemDrops = StaticCanvasList.instance.itemDropGenerator.GenerateItemDrops(level, dropRange);
        if(itemDrops.Count > 0)
        {
            LootBag existingBag = GetBag();
            if (existingBag)
            {
                existingBag.AddItems(itemDrops);
            }
            else
            {
                DropNewBag(itemDrops);
            }
        }

        Destroy(gameObject);
    }

    void DropNewBag(List<ItemSave> itemDrops)
    {
        LootBag bag = Instantiate(lootBagPrefab);
        bag.AddItems(itemDrops);
        bag.transform.SetParent(transform.parent);
        bag.transform.position = transform.position;
    }

    LootBag GetBag()
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position, bagLayerMask);
        foreach(Collider2D col in colliders)
        {
            LootBag bag = col.GetComponent<LootBag>();
            if (bag)
            {
                return bag;
            }
        }
        return null;
    }

}

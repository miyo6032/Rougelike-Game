﻿using UnityEngine;
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

    Animator damageCounter;
    Text damageText;
    Slider healthSlider;

    void Start()
    {
        damageCounter = HelperScripts.GetComponentFromChildrenExc<Animator>(transform);
        damageText = HelperScripts.GetComponentFromChildrenExc<Text>(transform);
        healthSlider = HelperScripts.GetComponentFromChildrenExc<Slider>(transform);
        health = maxHealth;
    }

    List<Item> GenerateItemDrops()
    {
        List<Item> items = new List<Item>();
        int numItems = Random.Range(dropRange.x, dropRange.y);
        for(int i = 0; i < numItems; i++)
        {
            items.Add(StaticCanvasList.instance.itemGenerator.GenerateItem(level, Random.Range(0, 3)));
        }
        return items;
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
            Death();
        }
    }

    void Death()
    {
        List<Item> itemDrops = GenerateItemDrops();
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

    void DropNewBag(List<Item> itemDrops)
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

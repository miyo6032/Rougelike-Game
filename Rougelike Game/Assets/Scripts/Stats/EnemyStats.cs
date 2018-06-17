﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Keeps track of enemy health and other stats - also handles death
/// </summary>
public class EnemyStats : Stats
{
    public Stat turnDelay;
    public int level;
    public int experienceDrop;
    [Header("Components")]
    public LootBag lootBagPrefab;
    public LayerMask bagLayerMask;
    public Vector2Int dropRange = new Vector2Int(0, 3);
    public Animator damageCounter;
    Animator animator;
    Text damageText;
    Slider healthSlider;
    private PlayerStats playerStats;

    private void Start()
    {
        damageText = HelperScripts.GetComponentFromChildrenExc<Text>(transform);
        healthSlider = HelperScripts.GetComponentFromChildrenExc<Slider>(transform);
        animator = GetComponent<Animator>();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        health = maxHealth.GetIntValue();
    }

    /// <summary>
    /// Damage the enemy, generate the damage counter, and update the health ui
    /// </summary>
    /// <param name="damage">Amount to damage the enemy</param>
    public override void TakeDamage(int damage)
    {
        damage = Mathf.Clamp(damage - defense.GetIntValue(), 0, damage);
        base.TakeDamage(damage);
        damageCounter.SetTrigger("damage");
        animator.SetTrigger("damage");
        damageText.text = "" + damage;
        healthSlider.value = health / (float) maxHealth.GetValue() * 100;
        if (health <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// When the enemy dies, drop its items and destroy the enemy
    /// </summary>
    private void Death()
    {
        List<ItemSave> itemDrops = StaticCanvasList.instance.itemDropGenerator.GenerateItemDrops(level, dropRange);
        if (itemDrops.Count > 0)
        {
            //If a bag already exists, just add the items to that bag
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
        playerStats.AddXP(experienceDrop);
        Destroy(gameObject);
    }

    /// <summary>
    /// If the enemy dies on an empty space, drop a new loot bag
    /// </summary>
    private void DropNewBag(List<ItemSave> itemDrops)
    {
        LootBag bag = Instantiate(lootBagPrefab);
        bag.AddItems(itemDrops);
        bag.transform.SetParent(transform.parent);
        bag.transform.position = transform.position;
    }

    /// <summary>
    /// Tries to get an existing bag where the enemy dies
    /// </summary>
    /// <returns></returns>
    private LootBag GetBag()
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position, bagLayerMask);
        foreach (Collider2D col in colliders)
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
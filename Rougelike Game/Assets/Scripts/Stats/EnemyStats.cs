using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Keeps track of enemy health and other stats - also handles death
/// </summary>
public class EnemyStats : Stats
{
    public Enemy enemy;
    [HideInInspector]
    public Stat turnDelay;
    [Header("Components")]
    public LootBag lootBagPrefab;
    public LayerMask bagLayerMask;
    public Animator damageCounter;
    private Vector2Int dropRange;
    private Animator animator;
    private Text damageText;
    private Slider healthSlider;
    private PlayerStats playerStats;
    private int experienceDrop;
    private int level;

    private void Start()
    {
        damageText = HelperScripts.GetComponentFromChildrenExc<Text>(transform);
        healthSlider = HelperScripts.GetComponentFromChildrenExc<Slider>(transform);
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        InitializeStats();
        InitializeAnimations();
        GetComponent<EnemyMovement>().StartMoving();
    }

    public void InitializeStats()
    {
        minAttack.SetBaseValue(enemy.minAttack);
        maxAttack.SetBaseValue(enemy.maxAttack);
        movementDelay.SetBaseValue(enemy.movementDelay);
        maxHealth.SetBaseValue(enemy.maxHealth);
        defense.SetBaseValue(enemy.defense);
        turnDelay.SetBaseValue(enemy.turnDelay);
        level = enemy.level;
        experienceDrop = enemy.experienceDrop;
        health = maxHealth.GetIntValue();
        dropRange = enemy.dropRange;
    }

    public void InitializeAnimations()
    {
        animator = GetComponent<Animator>();
        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animatorOverrideController["ArmedSkeletonIdle"] = enemy.idle;
        animatorOverrideController["ArmedSkeletonAttack"] = enemy.attack;
        animator.runtimeAnimatorController = animatorOverrideController;
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

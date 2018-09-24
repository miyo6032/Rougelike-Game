using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using EZCameraShake;

/// <summary>
/// Keeps track of enemy health and other stats - also handles death
/// </summary>
public class EnemyStats : Stats
{
    public Enemy enemy;

    [HideInInspector]
    public Stat turnDelay;

    [HideInInspector]
    public Stat attackDelay;

    [Header("Components")]
    public Experience experiencePrefab;

    public ItemPickup itemPrefab;
    public LayerMask bagLayerMask;
    public DamageCounter damageCounterPrefab;
    private Animator animator;
    private Slider healthSlider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        healthSlider = HelperScripts.GetComponentFromChildrenExc<Slider>(transform);
        InitializeStats();
    }

    public void InitializeStats()
    {
        minAttack.SetBaseValue(enemy.minAttack);
        maxAttack.SetBaseValue(enemy.maxAttack);
        movementDelay.SetBaseValue(enemy.movementDelay);
        attackDelay.SetBaseValue(enemy.attackDelay);
        maxHealth.SetBaseValue(enemy.maxHealth);
        defense.SetBaseValue(enemy.defense);
        turnDelay.SetBaseValue(enemy.turnDelay);
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

        ApplyDamageEffects(damage);

        if (health <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// Applies damage effects to indicate to the player that damage is happening!
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamageEffects(int damage)
    {
        SpawnDamageParticles();
        if (damage >= maxHealth.GetValue() / 2f)
        {
            CameraShaker.Instance.ShakeOnce(1f, 1f, 0.0f, 0.3f);
            SoundManager.Instance.PlayRandomizedPitch(SoundDatabase.Instance.PlayerHighAttack);
        }
        else
        {
            SoundManager.Instance.PlayRandomizedPitch(SoundDatabase.Instance.PlayerAttack);
        }
        DamageCounter instance = Instantiate(damageCounterPrefab);
        instance.SetText(damage.ToString());
        instance.transform.position = transform.position;
        animator.SetTrigger("damage");
        healthSlider.value = health / maxHealth.GetValue() * 100;
    }

    /// <summary>
    /// Spawn damage particles - which are basically just colored pixels
    /// </summary>
    public void SpawnDamageParticles()
    {
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
        {
            position = transform.position,
            applyShapeToPosition = true,
            startColor = enemy.particleDamagedColor
        };
        ParticleManager.instance.Emit(emitParams, 4);
    }

    /// <summary>
    /// When the enemy dies, drop its items and destroy the enemy
    /// </summary>
    private void Death()
    {
        if (enemy.dropChance > Random.Range(0, 1f))
        {
            ItemPickup item = Instantiate(itemPrefab, transform.parent);
            item.SetItem(new ItemStack(enemy.itemDrops[Random.Range(0, enemy.itemDrops.Length)], 1));
            item.transform.position = transform.position;
        }

        int numExpOrbs = HelperScripts.RandomVec(enemy.numExpOrbs);
        int expPerOrb = Mathf.RoundToInt(enemy.experienceDrop / (float)numExpOrbs);

        GetComponent<EnemyMovement>().RemoveSpotClaim();
        Destroy(gameObject);

        for (int i = 0; i < numExpOrbs; i++)
        {
            Experience exp = Instantiate(experiencePrefab, transform.parent);
            exp.transform.position = transform.position;
            exp.experienceAmount = expPerOrb;
        }
    }
}
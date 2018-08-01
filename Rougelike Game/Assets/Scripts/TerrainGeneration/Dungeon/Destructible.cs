using UnityEngine;

/// <summary>
/// Destroyable objects in the scene
/// </summary>
public class Destructible : Stats
{
    public DestructibleScriptableObject destructible;

    void Start()
    {
        health = destructible.maxHealth;
        defense.SetBaseValue(destructible.defence);
        GetComponent<SpriteRenderer>().sprite = destructible.sprites[Random.Range(0, destructible.sprites.Length)];
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        SoundManager.Instance.PlayRandomizedPitch(SoundDatabase.Instance.PlayerAttack);
        SpawnDamageParticles();
        if (health <= 0)
        {
            Death();
        }
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
            startColor = destructible.particleDamagedColor
        };
        ParticleManager.instance.Emit(emitParams, 4);
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}

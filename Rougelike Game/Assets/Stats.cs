using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Stats")]
    public Stat minAttack;
    public Stat maxAttack;

    public Stat movementDelay;

    public Stat maxHealth;
    public int health { get; protected set; }

    public Stat defense;

    /// <summary>
    /// Damage the entity
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(int damage)
    {
        health = Mathf.Clamp(health - damage, 0, health);
    }

    /// <summary>
    /// Heal the player by some amount
    /// </summary>
    /// <param name="amount"></param>
    public virtual void Heal(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth.GetIntValue());
    }

}

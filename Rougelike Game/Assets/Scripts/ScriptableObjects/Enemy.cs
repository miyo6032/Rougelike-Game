using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Enemy")]
public class Enemy : ScriptableObject
{
    [Range(0, 100)]
    public float minAttack;

    [Range(0, 100)]
    public float maxAttack;

    [Range(0, 10)]
    public float movementDelay;

    [Range(0, 10)]
    public float turnDelay;

    [Range(0, 10)]
    public float attackDelay;

    [Range(0, 1000)]
    public float maxHealth;

    [Range(0, 100)]
    public float defense;

    [Range(0, 30)]
    public int level;

    [Range(0, 10000)]
    public int experienceDrop;

    public Vector2Int numExpOrbs;
    public ItemScriptableObject[] itemDrops;
    public Vector2Int dropRange;

    [Range(0, 1)]
    public float dropChance;

    public Color particleDamagedColor;

    public AnimatorOverrideController animator;
    public AnimatorOverrideController enemySlashAnimator;
}
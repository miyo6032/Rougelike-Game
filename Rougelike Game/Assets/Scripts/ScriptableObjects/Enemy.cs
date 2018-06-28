using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Enemy")]
public class Enemy : ScriptableObject
{
    public float minAttack;
    public float maxAttack;
    public float movementDelay;
    public float maxHealth;
    public float defense;
    public float turnDelay;
    public int level;
    public int experienceDrop;
    public Vector2Int dropRange;
    public AnimationClip attack;
    public AnimationClip idle;
    public Color damagedColor;
}

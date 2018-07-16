using System.Collections;
using UnityEngine;

/// <summary>
/// Handles enemy movement, timing, and attack
/// </summary>
public class EnemyMovement : MovingObject
{
    public int enemySight = 3;

    // the loot bag detects everytime the collider is turned off and on
    // We want enemy pathfinding to disreguard enemies in the way of doorways and stuff like that,
    public LayerMask firstpassPathfindingLayer;
    public LayerMask secondPassPathfindingLayer;

    // So we only use the environment layer to do the pathfinding
    public Animator attackAnimator;
    protected Animator animator;
    protected EnemyStats stats;

    // Stores the player position
    protected Transform target;

    // Disabled during the linecast for pathfinding
    protected BoxCollider2D playerCol;
    protected BoxCollider2D enemyCol;

    // Responsible for alternating the move priorities - will sometimes be horizontal, and sometimes be vertical
    bool altMove = false;

    protected override void Start()
    {
        stats = GetComponent<EnemyStats>();
        animator = GetComponent<Animator>();
        target = GameObject.Find("Player").transform;
        playerCol = target.gameObject.GetComponent<BoxCollider2D>();
        enemyCol = GetComponent<BoxCollider2D>();
        Invoke("DelayedStart", Random.Range(0f, stats.turnDelay.GetValue()));
        base.Start();
    }

    void DelayedStart()
    {
        StartCoroutine(moveCounter());
    }

    /// <summary>
    /// Handles the basic enemy ai for one turn
    /// </summary>
    /// <returns></returns>
    IEnumerator moveCounter()
    {
        // While the enemy is not dead
        while (this)
        {
            transform.position = (Vector2)Vector2Int.RoundToInt(transform.position);

            // Disable Box colliders for future linecasts
            enemyCol.enabled = false;
            Vector2 nextMove = HelperScripts.GetNextMove(Vector2Int.RoundToInt(transform.position),
                Vector2Int.RoundToInt(target.transform.position), firstpassPathfindingLayer,
                enemySight, altMove);

            // If there is a blockage, it might be because an enemy is in the way, so then we look for a path
            // with the enemy's layer disabled
            if (nextMove.x == Vector2.positiveInfinity.x)
            {
                nextMove = HelperScripts.GetNextMove(Vector2Int.RoundToInt(transform.position),
                    Vector2Int.RoundToInt(target.transform.position), secondPassPathfindingLayer, enemySight, altMove);
            }

            // Make sure that nextMove actually found a path
            if (nextMove.x != Vector2.positiveInfinity.x)
            {
                // As long at the spot is not claimed already something else, we can try to move into the position
                if (!moveManager.SpotClaimed(
                    Vector2Int.RoundToInt(transform.position) + Vector2Int.RoundToInt(nextMove)))
                {
                    AttemptMove<PlayerStats>(Vector2Int.RoundToInt(nextMove));
                    altMove = !altMove;
                }
            }

            enemyCol.enabled = true;
            // Wait for however many second for the turn delay
            SetMoveSpeed(stats.movementDelay.GetValue());
            attackAnimator.speed = 1 / (stats.turnDelay.GetValue());
            attackAnimator.SetTrigger("Animate Indicator");
            yield return new WaitForSeconds(stats.turnDelay.GetValue());
        }
    }

    /// <summary>
    /// When the enemy encounters the player in its direct path
    /// </summary>
    /// <typeparam name="T">The PlayerStats component of the player</typeparam>
    /// <param name="component">PlayerStats</param>
    protected override void Attack<T>(T component)
    {
        PlayerStats hitPlayer = component as PlayerStats;
        animator.SetTrigger("EnemyAttack");
        hitPlayer.TakeDamage(Random.Range(stats.minAttack.GetIntValue(), stats.maxAttack.GetIntValue() + 1));
    }
}

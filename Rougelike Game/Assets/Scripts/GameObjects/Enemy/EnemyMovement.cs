using System.Collections;
using UnityEngine;

/// <summary>
/// Handles enemy movement, timing, and attack
/// </summary>
public class EnemyMovement : MovingObject
{
    // How many seconds an enemy has to wait until they can move again 
    public float turnDelay;
    public int enemySight = 3;

    // a way to ignore the player's collider without actually disabling the loot bag, because otherwise
    public LayerMask playerLayer;

    // the loot bag detects everytime the collider is turned off and on
    // We want enemy pathfinding to disreguard enemies in the way of doorways and stuff like that,
    public LayerMask pathfindingLayer;

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
        attackAnimator.speed = attackAnimator.speed / turnDelay;
        animator = GetComponent<Animator>();
        target = GameObject.Find("Player").transform;
        playerCol = target.gameObject.GetComponent<BoxCollider2D>();
        enemyCol = GetComponent<BoxCollider2D>();
        stats = GetComponent<EnemyStats>();
        base.Start();
        Invoke("DelayedStart", Random.Range(0f, turnDelay));
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
            // Disable Box colliders for future linecasts
            enemyCol.enabled = false;
            Vector2 nextMove = HelperScripts.GetNextMove(Vector2Int.FloorToInt(transform.position),
                Vector2Int.FloorToInt(target.transform.position), blockingLayer + pathfindingLayer - playerLayer,
                enemySight, altMove);
            // If there is a blockage, it might be because an enemy is in the way, so then we look for a path
            // with the enemy's layer disabled
            if (nextMove.x == Vector2.positiveInfinity.x)
            {
                nextMove = HelperScripts.GetNextMove(Vector2Int.FloorToInt(transform.position),
                    Vector2Int.FloorToInt(target.transform.position), pathfindingLayer, enemySight, altMove);
            }

            // Make sure that nextMove actually found a path
            if (nextMove.x != Vector2.positiveInfinity.x)
            {
                // As long at the spot is not claimed already something else, we can try to move into the position
                if (!moveManager.SpotClaimed(
                    Vector2Int.FloorToInt(transform.position) + Vector2Int.FloorToInt(nextMove)))
                {
                    AttemptMove<PlayerStats>(Vector2Int.FloorToInt(nextMove));
                    altMove = !altMove;
                }
            }

            enemyCol.enabled = true;
            // Wait for however many second for the turn delay
            attackAnimator.SetTrigger("Animate Indicator");
            yield return new WaitForSeconds(turnDelay);
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
        hitPlayer.DamagePlayer(Random.Range(stats.minAttack, stats.maxAttack + 1));
    }
}

using System.Collections;
using UnityEngine;

/// <summary>
/// Handles enemy movement, timing, and attack
/// </summary>
public class EnemyMovement : MovingObject
{
    public int enemySight = 3;

    private Vector2Int attackDir;

    // We want enemy pathfinding to disreguard enemies in the way of doorways and stuff like that
    public LayerMask firstpassPathfindingLayer;

    public LayerMask secondPassPathfindingLayer;

    protected Animator animator;
    protected EnemyStats stats;

    // Stores the player position
    protected Transform target;

    // Disabled during the linecast for pathfinding
    protected BoxCollider2D enemyCol;

    // Responsible for alternating the move priorities - will sometimes be horizontal, and sometimes be vertical
    private bool altMove = false;

    protected override void Start()
    {
        stats = GetComponent<EnemyStats>();
        animator = GetComponent<Animator>();
        target = PlayerStats.instance.transform;
        enemyCol = GetComponent<BoxCollider2D>();
        Invoke("DelayedStart", Random.Range(0f, stats.turnDelay.GetValue()));
        base.Start();
    }

    private void DelayedStart()
    {
        StartCoroutine(moveCounter());
    }

    private bool GetNextMove(out Vector2 nextMove)
    {
        nextMove = HelperScripts.GetNextMove(Vector2Int.RoundToInt(transform.position),
            Vector2Int.RoundToInt(target.transform.position), firstpassPathfindingLayer,
            enemySight, altMove);

        if (nextMove.x != Vector2.positiveInfinity.x) return true;

        // If there is a blockage, it might be because an enemy is in the way, so then we look for a path
        // with the enemy's layer disabled
        nextMove = HelperScripts.GetNextMove(Vector2Int.RoundToInt(transform.position),
                Vector2Int.RoundToInt(target.transform.position), secondPassPathfindingLayer, enemySight, altMove);

        if (nextMove.x != Vector2.positiveInfinity.x) return true;

        return false;
    }

    /// <summary>
    /// Handles the basic enemy ai for one turn
    /// </summary>
    /// <returns></returns>
    private IEnumerator moveCounter()
    {
        // While the enemy is not dead
        while (this)
        {
            // Disable Box colliders for future linecasts
            enemyCol.enabled = false;

            Vector2 nextMove;

            // Make sure that nextMove actually found a path
            if (GetNextMove(out nextMove))
            {
                // As long at the spot is not claimed already something else, we can try to move into the position
                if (!moveManager.SpotClaimed(Vector2Int.RoundToInt(transform.position) + Vector2Int.RoundToInt(nextMove)))
                {
                    AttemptMove(Vector2Int.RoundToInt(nextMove));
                    altMove = !altMove;
                }
            }

            enemyCol.enabled = true;
            // Wait for however many second for the turn delay
            SetMoveSpeed(stats.movementDelay.GetValue());

            yield return new WaitUntil(() => !moving);
            yield return new WaitForSeconds(stats.turnDelay.GetValue());
        }
    }

    protected override void OnStopMove()
    {
        enemyCol.enabled = false;
        Vector2 nextMove;
        if (GetNextMove(out nextMove))
        {
            TryToAttack(Vector2Int.RoundToInt(nextMove));
        }
        enemyCol.enabled = true;
    }

    /// <summary>
    /// When the enemy encounters the player in its direct path
    /// </summary>
    private void Attack()
    {
        enemyCol.enabled = false;
        animator.SetTrigger("EnemyAttack");
        if (PlayerInTheWay(attackDir))
            PlayerStats.instance.TakeDamage(Random.Range(stats.minAttack.GetIntValue(), stats.maxAttack.GetIntValue() + 1));
        moving = false;
        enemyCol.enabled = true;
        OnStopMove();
    }

    /// <summary>
    /// If the player is in the way of the movement
    /// </summary>
    private bool PlayerInTheWay(Vector2Int dir)
    {
        RaycastHit2D hit;
        if (CanMove(dir, out hit)) return false;
        PlayerStats player = hit.transform.GetComponent<PlayerStats>();
        if (player == null) return false;
        return true;
    }

    /// <summary>
    /// If the player is in the way, initiate an attack sequence
    /// </summary>
    private void TryToAttack(Vector2Int dir)
    {
        if (PlayerInTheWay(dir))
        {
            moving = true;
            attackDir = dir;
            Invoke("Attack", stats.attackDelay.GetValue());
        }
    }

    /// <summary>
    /// If nothing is in the way, initiate a move sequence
    /// </summary>
    private void AttemptMove(Vector2Int dir)
    {
        RaycastHit2D hit;
        if (!CanMove(dir, out hit))
        {
            TryToAttack(dir);
            return;
        }

        Vector2 start = transform.position;
        Vector2 end = start + dir;
        moveManager.ClaimSpot(Vector2Int.FloorToInt(end));
        StartCoroutine(SmoothMovement(end));
    }
}
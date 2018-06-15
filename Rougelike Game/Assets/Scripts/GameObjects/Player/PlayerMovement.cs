using UnityEngine;

/// <summary>
/// Handles the player movement and when the player can't move
/// </summary>
public class PlayerMovement : MovingObject
{
    //Automove calculation range
    public int range = 10;
    public LayerMask pathfindingLayer;
    public LayerMask enemyLayer;
    public LayerMask chestLayer;
    PlayerStats stats;
    PlayerAnimation animatorHandler;
    bool hitting;
    [HideInInspector] public Vector2Int facingdirection;

    // Variables that handle automatic movement when the player clicks a position
    bool automoving;
    bool altMove;
    Vector2 automovePosition;
    Transform automoveTarget;

    protected override void Start()
    {
        base.Start();
        stats = GetComponent<PlayerStats>();
        animatorHandler = GetComponent<PlayerAnimation>();
    }

    /// <summary>
    /// Automove with a specific enemy in mind
    /// </summary>
    /// <param name="target"></param>
    public void StartAutomoveWithTarget(Transform target)
    {
        automoveTarget = target;
        automoving = true;
    }

    /// <summary>
    /// Automove to an empty location if a route exists
    /// </summary>
    /// <param name="target"></param>
    public void StartAutomove(Vector2 target)
    {
        automoveTarget = null;
        automovePosition = target;
        automoving = true;
    }

    public void StopAutomove()
    {
        automoving = false;
        automoveTarget = null;
    }

    public bool CanUseSkill()
    {
        return !hitting && !moving && !(Time.timeScale == 0);
    }

    void Update()
    {
        if (moving || hitting || Time.timeScale == 0) return;

        SetMoveSpeed(stats.movementDelay.GetValue());

        InputMove();

        if (automoving)
        {
            Automove();
        }
    }

    /// <summary>
    /// Regular movement from the arrow keys
    /// </summary>
    private void InputMove()
    {
        Vector2Int input = new Vector2Int((int) Input.GetAxisRaw("Horizontal"), (int) Input.GetAxisRaw("Vertical"));

        // Keep the player from going diagonal, however epic that would be
        if (input.x != 0) input.y = 0;

        // Update the animator to align with the player's input
        animatorHandler.SetAttackAnimationDirection(input);

        // Execute the MOVE
        if (input.x != 0 || input.y != 0)
        {
            // check to make sure the spot is not claimed - and keeps enemies and players from moving into the same spot.
            if (!moveManager.SpotClaimed(Vector2Int.FloorToInt((Vector2) transform.position + input)))
            {
                StopAutomove();
                facingdirection = input;
                AttemptMove<EnemyStats>(input); // The player moves (or at least tries to)
            }
        }
    }

    /// <summary>
    /// Performs all calculations for automovement
    /// </summary>
    private void Automove()
    {
        Vector2 nextMove;
        if (automoveTarget != null)
        {
            // Automovement to an enemy target
            automovePosition = Vector2Int.FloorToInt(automoveTarget.position);
            nextMove = HelperScripts.GetNextMove(Vector2Int.FloorToInt(transform.position),
                Vector2Int.FloorToInt(automovePosition), pathfindingLayer + chestLayer, range, altMove);
        }
        else
        {
            // Automovement to a position
            nextMove = HelperScripts.GetNextMove(Vector2Int.FloorToInt(transform.position),
                Vector2Int.FloorToInt(automovePosition), pathfindingLayer + enemyLayer, range, altMove);
        }

        // We are done with automoving (we reached the target position)
        if (Vector2Int.FloorToInt(automovePosition) == Vector2Int.FloorToInt(transform.position))
        {
            StopAutomove();
            return;
        }

        if (nextMove.x != Vector2.positiveInfinity.x &&
            !moveManager.SpotClaimed(Vector2Int.FloorToInt(transform.position) + Vector2Int.FloorToInt(nextMove)))
        {
            // Update the animator to align with the player's input
            facingdirection = Vector2Int.RoundToInt(nextMove);
            animatorHandler.SetAttackAnimationDirection(Vector2Int.RoundToInt(nextMove));
            AttemptMove<EnemyStats>(Vector2Int.FloorToInt(nextMove)); // The player moves (or at least tries to)
            altMove = !altMove;
        }
    }

    /// <summary>
    /// Tries to move - handles different cases when the player can and cannot move
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dir"></param>
    protected override void AttemptMove<T>(Vector2Int dir)
    {
        // Hit to see if something is in the way
        RaycastHit2D hit;
        bool canMove = Move(dir, out hit);
        if (hit.transform == null)
        {
            // We were able to move, so animate that movement
            animatorHandler.AnimateMovement(dir);
            StaticCanvasList.instance.chestInventory.CloseChest();
            return;
        }

        // Get the specified component
        T hitComponent = hit.transform.GetComponent<T>();

        // Only call OnCantMove if the hit actually has the component
        if (!canMove && hitComponent != null)
        {
            StopAutomove();
            Attack(hitComponent);
        }
        else
        {
            Chest chest = hit.transform.GetComponent<Chest>();
            if (!canMove && chest != null)
            {
                StaticCanvasList.instance.chestInventory.OpenChest(chest);
                StopAutomove();
            }

            animatorHandler.SetIdle(dir);
        }
    }

    /// <summary>
    /// When the player tries to move into an enemy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    protected override void Attack<T>(T component)
    {
        if (hitting) return;
        hitting = true;
        Invoke("HittingFalse", stats.hitDelay.GetValue());
        animatorHandler.AnimateAttack();
        EnemyStats hitEnemy = component as EnemyStats;
        hitEnemy.TakeDamage(Random.Range(stats.minAttack.GetIntValue(), stats.maxAttack.GetIntValue() + 1));
    }

    /// <summary>
    /// Called by invoke with a delay to keep hitting timed
    /// </summary>
    void HittingFalse()
    {
        hitting = false;
    }
}

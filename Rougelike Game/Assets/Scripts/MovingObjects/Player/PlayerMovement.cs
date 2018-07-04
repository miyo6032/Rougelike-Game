using UnityEngine;

/// <summary>
/// Handles the player movement and when the player can't move
/// </summary>
public class PlayerMovement : MovingObject
{
    private PlayerStats stats;
    private PlayerAnimation animatorHandler;
    private Lighting lighting;
    private bool hitting;
    [HideInInspector]
    public Vector2Int facingdirection;

    // Variables that handle automatic movement when the player clicks a position
    private bool clickMove;
    private Vector2 automovePosition;

    protected override void Start()
    {
        base.Start();
        stats = GetComponent<PlayerStats>();
        animatorHandler = GetComponent<PlayerAnimation>();
        lighting = GetComponent<Lighting>();
    }

    /// <summary>
    /// Automove to an empty location if a route exists
    /// </summary>
    /// <param name="target"></param>
    public void StartAutomove(Vector2 target)
    {
        automovePosition = target;
        clickMove = true;
    }

    /// <summary>
    /// Automove with a specific enemy in mind
    /// </summary>
    /// <param name="target"></param>
    public void StartAutomoveWithTarget(Transform target)
    {
        automovePosition = target.transform.position;
        clickMove = true;
    }

    public void StopAutomove()
    {
        clickMove = false;
    }

    /// <summary>
    /// Stop - even if in the middle of the smooth movement coroutine
    /// </summary>
    public override void EmergencyStop()
    {
        base.EmergencyStop();
        StopAutomove();
    }

    public void TeleportPlayer(Vector3 pos)
    {
        EmergencyStop();
        transform.position = pos;
        lighting.ResetLight();
    }

    public bool CanUseSkill()
    {
        return !moving && !(Time.timeScale == 0);
    }

    void Update()
    {
        if (moving || Time.timeScale == 0) return;

        transform.position = (Vector2)Vector2Int.RoundToInt(transform.position);

        SetMoveSpeed(stats.movementDelay.GetValue());

        InputMove();

        if (clickMove)
        {
            Automove();
        }
    }

    protected override void OnStopMove()
    {
        lighting.GenerateLight();
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
            if (!moveManager.SpotClaimed(Vector2Int.RoundToInt((Vector2) transform.position + input)))
            {
                StopAutomove();
                facingdirection = input;
                AttemptMove<DungeonLevelGenerator>(input); // The player moves (or at least tries to)
            }
        }
    }

    /// <summary>
    /// Performs all calculations for automovement
    /// </summary>
    private void Automove()
    {
        Vector2 nextMove = automovePosition - Vector2Int.RoundToInt(transform.position);

        bool isNextTo = nextMove == Vector2.down || nextMove == Vector2.left || nextMove == Vector2.right || nextMove == Vector2.up;

        if (isNextTo && !moveManager.SpotClaimed(Vector2Int.RoundToInt(transform.position) + Vector2Int.RoundToInt(nextMove)))
        {
            // Update the animator to align with the player's input
            facingdirection = Vector2Int.RoundToInt(nextMove);
            animatorHandler.SetAttackAnimationDirection(Vector2Int.RoundToInt(nextMove));
            AttemptMove<EnemyStats>(Vector2Int.RoundToInt(nextMove)); // The player moves (or at least tries to)
        }

        StopAutomove();
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
            Attack(hitComponent);
        }
        else
        {
            Chest chest = hit.transform.GetComponent<Chest>();
            if (!canMove && chest != null)
            {
                StaticCanvasList.instance.chestInventory.OpenChest(chest);
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

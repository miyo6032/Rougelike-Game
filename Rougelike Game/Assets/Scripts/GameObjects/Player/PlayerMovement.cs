using UnityEngine;

//Handles the player movement and when the player can't move
public class PlayerMovement : MovingObject
{

    public int range = 10;

    public LayerMask pathfindingLayer;
    public LayerMask enemyLayer;
    public LayerMask chestLayer;

    PlayerStats stats;
    PlayerAnimation animatorHandler;
    bool hitting;

    //Variables that handle automatic movement when the player clicks a position
    bool automoving;
    bool altMove;
    Vector2 automovePosition;
    Transform automoveTarget;

    protected override void Start() {
        base.Start();
        stats = GetComponent<PlayerStats>();
        animatorHandler = GetComponent<PlayerAnimation>();
    }

    public void StartAutomoveWithTarget(Transform target)
    {
        automoveTarget = target;
        automoving = true;
    }

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

    void Update() {
        if (moving || hitting || Time.timeScale == 0) return;

        Vector2Int input = new Vector2Int((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));

        //Keep the player from going diagonal, however epic that would be
        if(input.x != 0)
            input.y = 0;

        //Update the animator to align with the player's input
        animatorHandler.SetAnimationDirection(input);

        //Execute the MOVE
        if (input.x != 0 || input.y != 0) {
            //check to make sure the spot is not claimed - and keeps enemies and players from moving into the same spot.
            if (!moveManager.SpotClaimed(Vector2Int.FloorToInt((Vector2)transform.position + input)))
            {
                automoving = false;
                AttemptMove<EnemyStats>(input);//The player moves (or at least tries to)
            }
        }

        if (automoving)
        {
            Vector2 nextMove;
            if (automoveTarget != null)
            {
                //Automovement to an enemy target
                automovePosition = Vector2Int.FloorToInt(automoveTarget.position);
                nextMove = HelperScripts.GetNextMove(Vector2Int.FloorToInt(transform.position), Vector2Int.FloorToInt(automovePosition), pathfindingLayer + chestLayer, range, altMove);
            }
            else
            {
                //Automovement to a position
                nextMove = HelperScripts.GetNextMove(Vector2Int.FloorToInt(transform.position), Vector2Int.FloorToInt(automovePosition), pathfindingLayer + enemyLayer, range, altMove);
            }

            //We are done with automoving (we reached the target position)
            if (Vector2Int.FloorToInt(automovePosition) == Vector2Int.FloorToInt(transform.position))
            {
                automoving = false;
                return;
            }

            if (nextMove.x != Vector2.positiveInfinity.x && !moveManager.SpotClaimed(Vector2Int.FloorToInt((Vector2)transform.position) + Vector2Int.FloorToInt(nextMove)))
            {
                //Update the animator to align with the player's input
                animatorHandler.SetAnimationDirection(nextMove);
                AttemptMove<EnemyStats>(Vector2Int.FloorToInt(nextMove));//The player moves (or at least tries to)
                altMove = !altMove;
            }
        }
    }

    protected override void AttemptMove<T>(Vector2Int dir)
    {
        //Hit to see if something is in the way
        RaycastHit2D hit;
        bool canMove = Move(dir, out hit);

        if (hit.transform == null)
        {
            //We were able to move, so animate that movement
            animatorHandler.AnimateMovement(dir);
            StaticCanvasList.instance.chestInventory.CloseChest();
            return;
        }

        //Get the specified component
        T hitComponent = hit.transform.GetComponent<T>();

        //Only call OnCantMove if the hit actually has the component
        if (!canMove && hitComponent != null)
        {
            automoving = false;
            Attack(hitComponent);
        }
        else
        {
            Chest chest = hit.transform.GetComponent<Chest>();
            if (!canMove && chest != null)
            {
                StaticCanvasList.instance.chestInventory.OpenChest(chest);
                automoving = false;
            }
            animatorHandler.SetIdle(dir);
        }
    }

    protected override void Attack<T>(T component) {
        if (hitting) return;
        hitting = true;
        Invoke("HittingFalse", stats.hitSpeed);
        animatorHandler.AnimateAttack();
        EnemyStats hitEnemy = component as EnemyStats;
        hitEnemy.DamageEnemy(Random.Range(stats.minAttack, stats.maxAttack + 1));
    }

    //Called by invoke with a delay to keep hitting timed
    void HittingFalse()
    {
        hitting = false;
    }

}

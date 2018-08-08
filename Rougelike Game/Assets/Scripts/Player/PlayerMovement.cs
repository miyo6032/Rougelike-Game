using UnityEngine;

/// <summary>
/// Handles the player movement and when the player can't move
/// </summary>
public class PlayerMovement : MovingObject
{
    public static PlayerMovement instance;
    private PlayerAnimation animatorHandler;
    private Lighting lighting;

    [HideInInspector]
    public Vector2Int facingdirection;

    // Variables that handle automatic movement when the player clicks a position
    private bool clickMove;

    //Keeps track of the hit direction to keep the player from hitting too rapidly
    private Vector2Int hitDirection;

    private Vector2 automovePosition;

    protected override void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Duplicate " + this.GetType().Name);
            Destroy(gameObject);
        }
        base.Start();
        animatorHandler = GetComponent<PlayerAnimation>();
        lighting = GetComponent<Lighting>();
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

    private void Update()
    {
        if (moving || Time.timeScale == 0) return;

        transform.position = (Vector2)Vector2Int.RoundToInt(transform.position);

        InputMove();
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
        Vector2Int input = new Vector2Int((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));

        // Keep the player from going diagonal, however epic that would be
        if (input.x != 0) input.y = 0;

        // Update the animator to align with the player's input
        animatorHandler.SetAttackAnimationDirection(input);

        // Execute the MOVE
        if (input.x != 0 || input.y != 0)
        {
            // check to make sure the spot is not claimed - and keeps enemies and players from moving into the same spot.
            if (!moveManager.SpotClaimed(Vector2Int.RoundToInt((Vector2)transform.position + input)))
            {
                facingdirection = input;
                AttemptMove(input); // The player moves (or at least tries to)
            }
        }
    }

    /// <summary>
    /// Tries to move - handles different cases when the player can and cannot move
    /// </summary>
    private void AttemptMove(Vector2Int dir)
    {
        if (hitDirection == dir) return;

        // Hit to see if something is in the way
        RaycastHit2D hit;
        if (!CanMove(dir, out hit))
        {
            hitDirection = dir;
            Invoke("ResetHitDirection", PlayerStats.instance.hitDelay.GetValue());

            Stats destructible = hit.transform.GetComponent<Stats>();
            Chest chest = hit.transform.GetComponent<Chest>();
            NPC npc = hit.transform.GetComponent<NPC>();

            if (destructible)
            {
                Attack(destructible);
            }
            else if (chest)
            {
                ChestInventory.instance.OpenChest(chest);
            }
            else if (npc)
            {
                npc.OnNPCClicked();
            }
            else
            {
                animatorHandler.SetIdle(dir);
            }

            return;
        }

        SetMoveSpeed(PlayerStats.instance.movementDelay.GetValue());

        // Initiate the movement
        Vector2 start = transform.position;
        Vector2 end = start + dir;
        moveManager.ClaimSpot(this, Vector2Int.FloorToInt(end));
        StartCoroutine(SmoothMovement(end));

        // Update certain other events
        DialoguePanel.instance.EndDialogue();
        ShopManager.instance.CloseShop();
        ChestInventory.instance.CloseChest();
        animatorHandler.AnimateMovement(dir);
    }

    /// <summary>
    /// When the player tries to move into an enemy
    /// </summary>
    protected void Attack(Stats hitObj)
    {
        animatorHandler.AnimateAttack();
        hitObj.TakeDamage(Random.Range(PlayerStats.instance.minAttack.GetIntValue(), PlayerStats.instance.maxAttack.GetIntValue() + 1));
    }

    /// <summary>
    /// Called by invoke with a delay to keep hitting timed
    /// </summary>
    private void ResetHitDirection()
    {
        hitDirection = Vector2Int.zero;
    }
}
using UnityEngine;
using System.Collections;

/// <summary>
/// Used by Player and Enemy movement classes: handles smooth translation and when the player cannot move
/// </summary>
public abstract class MovingObject : MonoBehaviour
{
    // Keeps the player from moving extra
    public LayerMask blockingLayer;
    [HideInInspector]
    public bool moving;
    protected MovementTracker moveManager;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        moveManager = GameObject.Find("GameManager").GetComponent<MovementTracker>();
    }

    protected void SetMoveSpeed(float moveSpeed)
    {
        inverseMoveTime = 1f / moveSpeed;
    }

    /// <summary>
    /// Will do a raycast to determine if the object can move - then will activate smooth movement
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    protected bool Move(Vector2Int dir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + dir;

        // Do the detection to see if there is anyting in the way
        Physics2D.queriesHitTriggers = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        Physics2D.queriesHitTriggers = true;

        // If the way is clear, go ahead and move
        if (hit.transform == null)
        {
            moveManager.ClaimSpot(Vector2Int.FloorToInt(end));
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        // Something is in the way
        return false;
    }

    public virtual void EmergencyStop()
    {
        moving = false;
    }

    /// <summary>
    /// Performs the physical move to the end
    /// </summary>
    /// <param name="end"></param>
    /// <returns></returns>
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        moving = true;
        // The remaining distance to the target position
        float sqrRemainingDistance = (rb2D.position - (Vector2)end).sqrMagnitude;
        // Keep going until we have reached our destination
        while (sqrRemainingDistance > float.Epsilon)
        {
            //Emergency Stop!
            if (!moving) break;

            // Determine the incremental position to move do based on the current position and the move time
            // Use fixed delta time because we are waiting for the fixed update
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.fixedDeltaTime);
            rb2D.MovePosition(newPosition);

            // Update our distance
            sqrRemainingDistance = (rb2D.position - (Vector2)end).sqrMagnitude;

            // Fixed update
            yield return new WaitForFixedUpdate();
        }

        OnStopMove();

        moveManager.RemoveClaim(Vector2Int.FloorToInt(end));

        moving = false;
    }

    protected virtual void OnStopMove()
    {
    }

    /// <summary>
    /// Try to move, if not, call attack() if the object hit is a target
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dir"></param>
    protected virtual void AttemptMove<T>(Vector2Int dir) where T : Component
    {
        // Hit to see if something is in the way
        RaycastHit2D hit;
        bool canMove = Move(dir, out hit);
        if (hit.transform == null)
        {
            return;
        }

        // Get the specified component
        T hitComponent = hit.transform.GetComponent<T>();

        // Only call OnCantMove if the hit actually has the component
        if (!canMove && hitComponent != null) Attack(hitComponent);
    }

    protected abstract void Attack<T>(T component) where T : Component;
}
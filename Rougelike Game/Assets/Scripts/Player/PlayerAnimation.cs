using UnityEngine;

/// <summary>
/// Handles player animations for movement and attacking
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    public Animator slashAnimator;
    public Animator animator;
    public SpriteRenderer sprite;

    // Used in animation to face left or right depending on movement
    private bool facingRight = true;

    // Used in animations to tell which way to face for an attack
    private Vector2Int attackDirection = Vector2Int.zero;

    // tell which way to face when moving
    private Vector2Int moveDirection = Vector2Int.zero;

    // which way to face when idle
    private Vector2Int idleDirection = Vector2Int.zero;

    /// <summary>
    /// Called by PlayerMovement when the player was able to move
    /// </summary>
    public void AnimateMovement(Vector2Int dir)
    {
        moveDirection = dir;
    }

    /// <summary>
    /// Called by PlayerMovement in case the player needs to attack
    /// </summary>
    /// <param name="dir"></param>
    public void SetAttackAnimationDirection(Vector2Int dir)
    {
        // We aren't moving necessarily, so reset
        moveDirection = Vector2Int.zero;
        idleDirection = Vector2Int.zero;

        // If the player is moving side to side
        if (dir.x != 0)
        {
            if (dir.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (dir.x < 0 && facingRight)
            {
                Flip();
            }
        }

        attackDirection = dir;
    }

    /// <summary>
    /// Called when the player successfully lands an attack
    /// </summary>
    public void AnimateAttack()
    {
        slashAnimator.SetTrigger("slash");
        if (attackDirection.x != 0)
        {
            animator.SetTrigger("attackSide");
            slashAnimator.transform.rotation = Quaternion.Euler(0, 0, attackDirection.x == -1 ? 180 : 0);
            slashAnimator.transform.localPosition = new Vector3(attackDirection.x, 0, 0);
        }
        else if (attackDirection.y < 0)
        {
            animator.SetTrigger("attackFront");
            slashAnimator.transform.rotation = Quaternion.Euler(0, 0, -90);
            slashAnimator.transform.localPosition = new Vector3(0, -1, 0);
        }
        if (attackDirection.y > 0)
        {
            animator.SetTrigger("attackBack");
            slashAnimator.transform.rotation = Quaternion.Euler(0, 0, 90);
            slashAnimator.transform.localPosition = new Vector3(0, 1, 0);
        }
    }

    /// <summary>
    /// Sets the idle direction to be used when the player is idle
    /// </summary>
    /// <param name="dir"></param>
    public void SetIdle(Vector2Int dir)
    {
        idleDirection = dir;
    }

    // Realtime animation based on whether or not the player is moving
    private void Update()
    {
        if (Time.timeScale > 0)
        {
            animator.SetFloat("horizontalMove", Mathf.Abs(moveDirection.x));
            animator.SetFloat("verticalMove", moveDirection.y);
            animator.SetFloat("horizontalIdle", Mathf.Abs(idleDirection.x));
            animator.SetFloat("verticalIdle", idleDirection.y);
        }
    }

    /// <summary>
    /// Flip the player when they move right/left
    /// </summary>
    private void Flip()
    {
        sprite.flipX = facingRight;
        facingRight = !facingRight;
    }
}
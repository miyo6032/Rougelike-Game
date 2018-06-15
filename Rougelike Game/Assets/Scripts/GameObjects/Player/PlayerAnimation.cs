using UnityEngine;

/// <summary>
/// Handles player animations for movement and attacking
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    public Animator[] animators;
    public SpriteRenderer leftSword;
    public SpriteRenderer rightSword;
    public SpriteRenderer armor;
    public SpriteRenderer helmet;
    public SpriteRenderer skin;

    // Used in animation to face left or right depending on movement
    bool facingRight = true;

    // Used in animations to tell which way to face for an attack
    [HideInInspector]
    public Vector2Int attackDirection = Vector2Int.zero;

    // tell which way to face when moving
    Vector2Int moveDirection = Vector2Int.zero;

    // which way to face when idle
    Vector2Int idleDirection = Vector2Int.zero;

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
        if (attackDirection.x != 0) SetTriggers("attackSide");
        if (attackDirection.y < 0) SetTriggers("attackFront");
        if (attackDirection.y > 0) SetTriggers("attackBack");
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
    void Update()
    {
        if (Time.timeScale > 0)
        {
            SetFloats("horizontalMove", Mathf.Abs(moveDirection.x));
            SetFloats("verticalMove", moveDirection.y);
            SetFloats("horizontalIdle", Mathf.Abs(idleDirection.x));
            SetFloats("verticalIdle", idleDirection.y);
        }
    }

    /// <summary>
    /// Flip the player when they move right/left
    /// </summary>
    void Flip()
    {
        leftSword.flipX = facingRight;
        rightSword.flipX = facingRight;
        armor.flipX = facingRight;
        helmet.flipX = facingRight;
        skin.flipX = facingRight;
        facingRight = !facingRight;
    }

    void SetTriggers(string trigger)
    {
        foreach (Animator animator in animators)
        {
            animator.SetTrigger(trigger);
        }
    }

    void SetFloats(string trigger, float dir)
    {
        foreach (Animator animator in animators)
        {
            animator.SetFloat(trigger, dir);
        }
    }

    /// <summary>
    /// Color the player's armor and swords
    /// </summary>
    /// <param name="equipmentType"></param>
    /// <param name="color"></param>
    public void ColorAnimator(string equipmentType, string color)
    {
        Color itemColor;
        if (ColorUtility.TryParseHtmlString(color, out itemColor))
        {
            switch (equipmentType)
            {
                case "LeftSwordSlot":
                    leftSword.color = itemColor;
                    break;
                case "RightSwordSlot":
                    rightSword.color = itemColor;
                    break;
                case "ChestSlot":
                    armor.color = itemColor;
                    break;
                case "HelmetSlot":
                    helmet.color = itemColor;
                    break;
            }
        }
    }
}

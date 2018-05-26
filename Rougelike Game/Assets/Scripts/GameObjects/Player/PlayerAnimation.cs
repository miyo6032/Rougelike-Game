using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

    public Animator[] animators;

    public SpriteRenderer sword;
    public SpriteRenderer armor;
    public SpriteRenderer helmet;

    bool facingRight = true; //Used in animation to face left or right depending on movement
    Vector2 attackDirection = Vector2.zero; //Used in animations to tell which way to face for an attack
    Vector2 moveDirection = Vector2.zero; //tell which way to face when moving
    Vector2 idleDirection = Vector2.zero; //which way to face when idle

    //Called when the player was able to move - the update will do the rest
    public void AnimateMovement(Vector2 dir)
    {
        moveDirection = dir;
    }

    public void SetAnimationDirection(Vector2 dir)
    {

        //We aren't moving necessarily, so reset
        moveDirection = Vector2.zero;
        idleDirection = Vector2.zero;

        //If the player is moving side to side
        if (dir.x != 0)
        {
            if (dir.x > 0 && !facingRight)
            {
                flip();
            }
            else if (dir.x < 0 && facingRight)
            {
                flip();
            }
        }

        attackDirection = dir;

    }

    //Called when the player successfully lands an attack
    public void AnimateAttack()
    {
        if (attackDirection.x != 0) SetTriggers("attackSide");
        if (attackDirection.y < 0) SetTriggers("attackFront");
        if (attackDirection.y > 0) SetTriggers("attackBack");
    }

    //Sets the idle direction to be used when the player is idle
    public void SetIdle(Vector2 dir)
    {
        idleDirection = dir;
    }

    //Realtime animation based on whether or not the player is moving
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

    //Flip the player when they move right/left
    void flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void SetTriggers(string trigger)
    {
        foreach(Animator animator in animators)
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

    public void ColorAnimator(int equipmentType, string color)
    {
        Color itemColor;
        if (ColorUtility.TryParseHtmlString(color, out itemColor))
        {
            switch (equipmentType)
            {
                case 0:
                    sword.color = itemColor;
                    break;
                case 1:
                    armor.color = itemColor;
                    break;
                case 2:
                    helmet.color = itemColor;
                    break;
            }
        }
    }

}

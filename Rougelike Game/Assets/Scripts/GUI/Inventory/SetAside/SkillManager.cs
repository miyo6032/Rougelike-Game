using UnityEngine;

public enum Skills
{
    CriticalHit
}

//Handles the input for skills, the active and passive skills.
public class SkillManager : MonoBehaviour
{
    public PlayerStats playerStats;
    private PlayerMovement playerMovement;
    private PlayerAnimation playerAnimator;

    void Start()
    {
        playerMovement = playerStats.GetComponent<PlayerMovement>();
        playerAnimator = playerStats.GetComponent<PlayerAnimation>();
    }

    //Figures out what to do for a skill passed in, and execute the skill
    public void DoTheSkill(Skills type)
    {
        if (playerMovement.CanUseSkill())
        {
            switch (type)
            {
                case Skills.CriticalHit:
                    CriticalHit();
                    break;
            }
        }
    }

    void CriticalHit()
    {
        EnemyStats enemy = FindEnemy(playerMovement.facingdirection + (Vector2)playerMovement.transform.position);
        if (enemy != null)
        {
            playerAnimator.SetAttackAnimationDirection(playerMovement.facingdirection);
            playerAnimator.AnimateAttack();
            enemy.TakeDamage(Random.Range(playerStats.minAttack.GetIntValue() * 2, playerStats.maxAttack.GetIntValue() * 2 + 1));
            playerStats.ChangeFocus(-10);
        }
    }

    protected EnemyStats FindEnemy(Vector2 pos)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(pos, playerMovement.blockingLayer);

        foreach (Collider2D col in colliders)
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            if (enemy)
            {
                return enemy;
            }
        }

        return null;
    }
}

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

    /// <summary>
    /// Based on the skill type, do that skill, and subtract focus
    /// </summary>
    /// <param name="type"></param>
    /// <param name="focus"></param>
    public void DoTheSkill(Skills type, int focus)
    {
        if (playerMovement.CanUseSkill() && playerStats.focus >= focus)
        {
            switch (type)
            {
                case Skills.CriticalHit:
                    CriticalHit(focus);
                    break;
            }
        }
    }

    /// <summary>
    /// Executes the critical hit skill
    /// </summary>
    /// <param name="focus"></param>
    void CriticalHit(int focus)
    {
        EnemyStats enemy = FindEnemy(playerMovement.facingdirection + (Vector2)playerMovement.transform.position);
        if (enemy != null)
        {
            playerAnimator.SetAttackAnimationDirection(playerMovement.facingdirection);
            playerAnimator.AnimateAttack();
            enemy.TakeDamage(Random.Range(playerStats.minAttack.GetIntValue() * 2, playerStats.maxAttack.GetIntValue() * 2 + 1));
            playerStats.ChangeFocus(-focus);
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

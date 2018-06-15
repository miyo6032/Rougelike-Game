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
        EnemyStats enemy = FindEnemy(playerAnimator.attackDirection);
        if (enemy != null)
        {
            playerAnimator.AnimateAttack();
            enemy.TakeDamage(Random.Range(playerStats.minAttack.GetIntValue(), playerStats.maxAttack.GetIntValue() + 1));
        }
    }

    protected EnemyStats FindEnemy(Vector2Int dir)
    {
        Vector2 start = transform.position;
        Vector2 end = start + dir;

        // Do the detection to see if there is anyting in the way
        Physics2D.queriesHitTriggers = false;
        RaycastHit2D hit = Physics2D.Linecast(start, end, playerMovement.blockingLayer);
        Physics2D.queriesHitTriggers = true;

        // If the way is clear, go ahead and move
        if (hit.transform != null)
        {
            return hit.transform.GetComponent<EnemyStats>();
        }

        return null;
    }

}

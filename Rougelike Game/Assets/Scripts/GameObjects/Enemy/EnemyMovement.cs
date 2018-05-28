using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles movement and when the enemy cannot move
//Also hold the ai/pathfinding for the enemy
public class EnemyMovement : MovingObject
{
    public float turnDelay; //How many seconds an enemy has to wait until they can move again 
    public int enemySight = 3;
    public LayerMask playerLayer;//a way to ignore the player's collider without actually disabling the loot bag, because otherwise
    //the loot bag detects everytime the collider is turned off and on
    public LayerMask pathfindingLayer; //We want enemy pathfinding to disreguard enemies in the way of doorways and stuff like that,
    //So we only use the environment layer to do the pathfinding

    protected Animator animator;
    protected EnemyStats stats;
    protected Transform target; //Stores the player position
    protected BoxCollider2D playerCol; //Disabled during the linecast for pathfinding
    protected BoxCollider2D enemyCol;
    bool altMove = false; //Responsible for alternating the move priorities - will sometimes be horizontal, and sometimes be vertical

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.Find("Player").GetComponent<PlayerMovement>().transform;
        playerCol = target.gameObject.GetComponent<BoxCollider2D>();
        enemyCol = GetComponent<BoxCollider2D>();
        stats = GetComponent<EnemyStats>();
        base.Start();
        Invoke("DelayedStart", Random.Range(0f, turnDelay));
    }

    void DelayedStart()
    {
        StartCoroutine(moveCounter());
    }

    //The enemy's movement
    IEnumerator moveCounter()
    {
        while (this) //Will be changed to while the enemy is not dead
        {
            //Disable Box colliders for future linecasts
            enemyCol.enabled = false;
            Vector2 nextMove = HelperScripts.GetNextMove(Vector2Int.FloorToInt(transform.position), Vector2Int.FloorToInt(target.transform.position), blockingLayer + pathfindingLayer - playerLayer, enemySight, altMove);
            //If there is a blockage, it might be because an enemy is in the way, so then we look for a path
            //with the enemy's layer disabled
            if (nextMove.x == Vector2.positiveInfinity.x)
            {
                nextMove = HelperScripts.GetNextMove(Vector2Int.FloorToInt(transform.position), Vector2Int.FloorToInt(target.transform.position), pathfindingLayer, enemySight, altMove);
            }

            //Make sure that nextMove actually found a path
            if (nextMove.x != Vector2.positiveInfinity.x)
            {
                //As long at the spot is not claimed already something else, we can try to move into the position
                if (!moveManager.SpotClaimed(Vector2Int.RoundToInt((Vector2)transform.position + nextMove)))
                {
                    AttemptMove<PlayerStats>((int)nextMove.x, (int)nextMove.y);
                    altMove = !altMove;
                }
            }
            enemyCol.enabled = true;
            //Wait for however many second for the turn delay
            yield return new WaitForSeconds(turnDelay);
        }
    }

    //When the enemy encounters the player
    protected override void Attack<T>(T component)
    {
        PlayerStats hitPlayer = component as PlayerStats;
        animator.SetTrigger("EnemyAttack");
        hitPlayer.DamagePlayer(Random.Range(stats.minAttack, stats.maxAttack + 1));
    }
}

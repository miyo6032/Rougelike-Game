using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles movement and when the enemy cannot move
public class EnemyMovement : MovingObject {
    public int turnDelay; //How many seconds an enemy has to wait until they can move again 
    public int enemySight = 3;

    protected Animator animator;
    protected EnemyStats stats;
    protected Transform target; //Stores the player position
    protected BoxCollider2D playerCol; //Disabled during the linecast for pathfinding
    bool altMove = false; //Responsible for alternating the move priorities - will sometimes be horizontal, and sometimes be vertical

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.Find("Player").GetComponent<PlayerMovement>().transform;
        playerCol = target.gameObject.GetComponent<BoxCollider2D>();
        stats = GetComponent<EnemyStats>();
        base.Start();
        StartCoroutine(moveCounter());
    }

    //The enemy's movement
    IEnumerator moveCounter()
    {
        while (this) //Will be changed to while the enemy is not dead
        {
            //Disable Box colliders for future linecasts
            boxCollider.enabled = false;
            playerCol.enabled = false;
            Vector2 nextMove = GetNextMove();
            boxCollider.enabled = true;
            playerCol.enabled = true;

            //check to see if the next move is valid - if the path was found and the spot is not claimed
            if (nextMove.x != Vector2.positiveInfinity.x &&
                !moveManager.SpotClaimed(Vector2Int.RoundToInt( (Vector2)transform.position + nextMove * moveScale))) {
                AttemptMove<PlayerStats>((int)nextMove.x, (int)nextMove.y);
                altMove = !altMove;
            }
            //Wait for however many second for the turn delay
            yield return new WaitForSeconds(turnDelay);
        }
    }

    //LINECAST our way to VICTORY
    //Uses a modified sssp algorithm to find the next move for the enemy
    public Vector2 GetNextMove()
    {

        //Helper directional vectors
        Vector2Int up = new Vector2Int(0, 1);
        Vector2Int down = new Vector2Int(0, -1);
        Vector2Int right = new Vector2Int(1, 0);
        Vector2Int left = new Vector2Int(-1, 0);

        //The two positions we are interested in
        Vector2Int targetPos = new Vector2Int((int)(target.position.x / moveScale), (int)(target.position.y / moveScale));
        Vector2Int currentPos = new Vector2Int((int)(transform.position.x / moveScale), (int)(transform.position.y / moveScale));

        //Now, tranform those position so we can work with them on the matrix from 0, 0 to n, n
        Vector2Int relCurPos = new Vector2Int(enemySight, enemySight);
        Vector2Int relTarPos = targetPos - currentPos + relCurPos;

        //Return no path - player out of vision
        if (Mathf.Abs((targetPos - currentPos).x) > enemySight || Mathf.Abs((targetPos - currentPos).y) > enemySight)
        {
            return Vector2.positiveInfinity;
        }

        //The overall height/width of the search graph. 
        int boxSize = enemySight * 2 + 1;

        //Initialize the visisted and predecessor arrays
        bool[,] visited = new bool[boxSize, boxSize];
        Vector2Int[,] pred = new Vector2Int[boxSize, boxSize];
        //Initialize Values
        for (int i = 0; i < boxSize; i++)
        {
            for (int j = 0; j < boxSize; j++)
            {
                visited[i, j] = false;
            }
        }

        Queue<Vector2Int> Q = new Queue<Vector2Int>();
        //Start at the player's position and work back to the enemy
        Q.Enqueue(relTarPos);

        //Do the search
        while (Q.Count > 0)
        {
            Vector2Int v = Q.Dequeue();
            if (!visited[v.x, v.y])
            {

                //Found the path
                if (v == relCurPos)
                {
                    return pred[v.x, v.y] - v;
                }

                visited[v.x, v.y] = true;

                //Helper directions
                Vector2Int[] adj = { (v + right), (v + left), (v + up), (v + down) };

                //Makes the movement a little more interesting aka, just make the enemy move diagonally rather than in L shape
                if (altMove)
                {
                    Vector2Int temp = adj[0];
                    adj[0] = adj[2];
                    adj[2] = temp;
                    temp = adj[1];
                    adj[1] = adj[3];
                    adj[3] = temp;
                }

                //Add all edges - all for of them using linecasts to make sure nothing is in the way
                for (int i = 0; i < 4; i++)
                {
                    //This statement checks the bounds
                    if (adj[i].y <= boxSize - 1 && adj[i].y >= 0 && adj[i].x <= boxSize - 1 && adj[i].x >= 0)
                    {
                        //Calculate the to and from of the line cast
                        Vector2 from = v + currentPos - relCurPos;
                        Vector2 to = adj[i] + currentPos - relCurPos;
                        //Line cast, converting back to real space rather than 1 x 1 space
                        if (Physics2D.Linecast(from * moveScale, to * moveScale, blockingLayer).transform == null)
                        {
                            if (!visited[adj[i].x, adj[i].y])
                            {
                                Q.Enqueue(adj[i]);
                                pred[adj[i].x, adj[i].y] = v;
                            }
                        }
                    }
                }
            }
        }
        return Vector2.positiveInfinity;
    }

    //When the enemy encounters the player
    protected override void OnCantMove<T>(T component)
    {
        PlayerStats hitPlayer = component as PlayerStats;
        animator.SetTrigger("EnemyAttack");
        hitPlayer.DamagePlayer(stats.attack);
    }
}

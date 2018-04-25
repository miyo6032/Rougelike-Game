using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Enemy : MovingObject
{
    public static bool enemyInfoOpen = false;

    public int playerDamage;
    public int enemyLevel;
    public int enemySpeed;
    public string enemyDesc;
    public int turnCounter = 0;

    protected Animator animator;
    protected Transform target;
    protected bool altMove = true;
    protected Inventory inventory;
    protected Vector2 nextMove;

    Vertex[] graph = new Vertex[51];

    int graphIndex = 0;

    private EventSystem _eventSystem;

    protected override void Start()
    {
        inventory = GameObject.Find("ItemDatabase").GetComponent<Inventory>();
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = Player.instance.transform;
        _eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        base.Start();
    }

    public void OnMouseDown()
    {
        if (_eventSystem.IsPointerOverGameObject())
        {
            return;
        }

        if (enemyInfoOpen)
        {
            inventory.deactivateInfo();
        }
        else
        {
            string data = this.name.Remove(this.name.Length - 7) + "\n\n" + this.enemyDesc + "\n\n";
            data = data + "Health: " + this.GetComponent<Wall>().hp + " hp out of " + this.GetComponent<Wall>().maxHp + "\n";
            data = data + "Attack: " + this.playerDamage + "-" + (this.playerDamage * 2 - 1) + "\n";
            data = data + "Defence: " + this.GetComponent<Wall>().defence + "\n";
            data = data + "Difficulty: " + this.enemyLevel + "\n";
            inventory.activateEnemyInfo(data, true);
        }
    }

    public virtual void MoveEnemy()
    {
        if ((int)nextMove.x != 0 || (int)nextMove.y != 0)
            AttemptMove<Player>((int)nextMove.x, (int)nextMove.y);
        nextMove = new Vector2(0, 0);
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (altMove)
        {
            base.AttemptMove<T>(xDir, yDir);
            altMove = !altMove;
        }
        else if (!altMove)
        {
            base.AttemptMove<T>(xDir, yDir);
            altMove = !altMove;
        }

    }

    public virtual void SetupNextMove()
    {
        graphIndex = 0;

        Vertex player = new Vertex();
        player.position = GameManager.instance.claimedSpots[0];
        player.visited = false;
        graph[graphIndex] = player;
        graphIndex++;

        Vertex enemy = new Vertex();
        enemy.position = new Vector2(transform.position.x, transform.position.y);
        enemy.visited = false;
        graph[graphIndex] = enemy;
        graphIndex++;

        Vertex vec2 = new Vertex();
        vec2.position = new Vector2(target.position.x, target.position.y);
        vec2.visited = false;
        graph[graphIndex] = vec2;
        graphIndex++;

        int agroSize = 3;
        for (int X = -agroSize; X < agroSize + 1; X++)//Finds all of the possible spaces to move in an area.
        {
            for (int Y = -agroSize; Y < agroSize + 1; Y++)
            {
                Vector2 vec1 = new Vector2(X + (int)transform.position.x, Y + (int)transform.position.y);
                if (BoardManager.instance.FloorPositionTaken((int)vec1.x, (int)vec1.y) && !BoardManager.instance.ObjectPositionTaken((int)vec1.x, (int)vec1.y)
                    && !GameManager.instance.SpotClaimed(vec1) && vec1 != player.position && vec1 != enemy.position && vec1 != vec2.position)
                {
                    Vertex v = new Vertex();
                    v.position = vec1;
                    v.visited = false;
                    graph[graphIndex] = v;
                    graphIndex++;
                }
            }
        }

        for (int i = 0; i < graphIndex; i++)//For each space, find all adjacent spaces.
        {
            for (int j = 0; j < graphIndex; j++)
            {
                if (graph[i] != null && graph[j] != null)
                {
                    Vector2 diff = graph[i].position - graph[j].position;
                    if ((Mathf.Abs(diff.x) == 1 && Mathf.Abs(diff.y) == 0) ^ (Mathf.Abs(diff.x) == 0 && Mathf.Abs(diff.y) == 1))
                    {
                        //Debug.Log(graph[i].position.x + " " + graph[i].position.y + " , " + graph[j].position.x + " " + graph[j].position.y);
                        graph[i].Add(graph[j]);
                    }
                }
            }
        }

        Queue<Vertex> q = new Queue<Vertex>();//Works backwards from the players position to the enemy
        graph[0].setVisited(true);
        graph[0].isRoot = true;
        q.Enqueue(graph[0]);

        while (q.Count != 0)
        {
            Vertex current = q.Dequeue();
            if (current != null)
            {
                for (int i = 0; i < current.Count(); i++)
                {
                    if (current.adj[i] != null && !current.adj[i].visited)
                    {
                        current.adj[i].setVisited(true);
                        current.adj[i].parent = current;
                        q.Enqueue(current.adj[i]);

                        if (current.adj[i].position == enemy.position && (current.adj[i].parent.position != player.position || player.position == vec2.position))//If the enemy position is found, and if the player's claimed spot isn't in the way. Also, coveres the case when the player waits.
                        {
                            Vertex nextMoveVertex = current.adj[i].parent;
                            GameManager.instance.claimedSpots.Add(nextMoveVertex.position);
                            Vector2 direction = nextMoveVertex.position - (Vector2)transform.position;
                            nextMove = direction;
                        }
                    }
                }
            }
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        if (Mathf.Abs(target.position.x - transform.position.x) < 2 && Mathf.Abs(target.position.y - transform.position.y) < 2)
        {
            animator.SetTrigger("EnemyAttack");
            hitPlayer.LoseFood(playerDamage + Random.Range(0, playerDamage));
        }
    }
}

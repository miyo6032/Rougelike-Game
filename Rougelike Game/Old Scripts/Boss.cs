using UnityEngine;
using System.Collections;

public class Boss : Enemy {

    public GameObject enemyToSummon;
    public int maxEnemyCount = 0;
    public int turnCooldown;
    int enemyCount = 0;
    int counter = 0;
    private Vector2 summonDirection;

    public override void SetupNextMove()
    {

        if (maxEnemyCount > enemyCount && counter % turnCooldown == 0)
        {

            int posX = (int)transform.position.x;
            int posY = (int)transform.position.y;

            enemyCount++;

            if (!BoardManager.instance.ObjectPositionTaken(posX + 2, posY) && BoardManager.instance.FloorPositionTaken(posX + 2, posY) && !GameManager.instance.SpotClaimed(new Vector2(posX + 2, posY)))
            {
                summonDirection = new Vector2(posX + 2, posY);
            }
            else if (!BoardManager.instance.ObjectPositionTaken(posX + 2, posY + 1) && BoardManager.instance.FloorPositionTaken(posX + 2, posY + 1) && !GameManager.instance.SpotClaimed(new Vector2(posX + 2, posY + 1)))
            {
                summonDirection = new Vector2(posX + 2, posY + 1);
            }
            else if (!BoardManager.instance.ObjectPositionTaken(posX, posY + 2) && BoardManager.instance.FloorPositionTaken(posX, posY + 2) && !GameManager.instance.SpotClaimed(new Vector2(posX, posY + 2)))
            {
                summonDirection = new Vector2(posX, posY + 2);
            }
            else if (!BoardManager.instance.ObjectPositionTaken(posX + 1, posY + 2) && BoardManager.instance.FloorPositionTaken(posX + 1, posY + 2) && !GameManager.instance.SpotClaimed(new Vector2(posX + 1, posY + 2)))
            {
                summonDirection = new Vector2(posX + 1, posY + 2);
            }
            else if (!BoardManager.instance.ObjectPositionTaken(posX - 1, posY + 1) && BoardManager.instance.FloorPositionTaken(posX - 1, posY + 1) && !GameManager.instance.SpotClaimed(new Vector2(posX - 1, posY + 1)))
            {
                summonDirection = new Vector2(posX - 1, posY + 1);
            }
            else if (!BoardManager.instance.ObjectPositionTaken(posX - 1, posY) && BoardManager.instance.FloorPositionTaken(posX - 1, posY) && !GameManager.instance.SpotClaimed(new Vector2(posX - 1, posY)))
            {
                summonDirection = new Vector2(posX - 1, posY);
            }
            else if (!BoardManager.instance.ObjectPositionTaken(posX + 1, posY - 1) && BoardManager.instance.FloorPositionTaken(posX + 1, posY - 1) && !GameManager.instance.SpotClaimed(new Vector2(posX + 1, posY - 1)))
            {
                summonDirection = new Vector2(posX + 1, posY - 1);
            }
            else if (!BoardManager.instance.ObjectPositionTaken(posX, posY - 1) && BoardManager.instance.FloorPositionTaken(posX, posY - 1) && !GameManager.instance.SpotClaimed(new Vector2(posX, posY - 1)))
            {
                summonDirection = new Vector2(posX, posY - 1);
            }
            else
            {
                enemyCount--;
            }

        }

        base.SetupNextMove();

        counter++;

    }
    public override void MoveEnemy()
    {
        BoardManager.instance.AddEnemy(summonDirection.x, summonDirection.y, enemyToSummon);
        base.MoveEnemy();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {

        if (altMove)
        {
            AttemptBossMove<T>(xDir, yDir);
            altMove = !altMove;
        }
        else if (!altMove)
        {
            AttemptBossMove<T>(xDir, yDir);
            altMove = !altMove;
        }

    }

    private void AttemptBossMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        RaycastHit2D hit2;
        bool canMove = MoveBoss(xDir, yDir, out hit, out hit2);

        if (hit.transform == null && hit2.transform == null)
        {
            return;
        }

        T hitComponent;

        if (hit.transform != null)
        {

            hitComponent = hit.transform.GetComponent<T>();

            if (!canMove && hitComponent != null)
                OnCantMove(hitComponent);

        }

        if (hit2.transform != null)
        {
            hitComponent = hit2.transform.GetComponent<T>();

            if (!canMove && hitComponent != null)
                OnCantMove(hitComponent);
        }
    }

    protected bool MoveBoss(int xDir, int yDir, out RaycastHit2D hit, out RaycastHit2D hit2)
    {

        Vector2 origin = transform.position;
        Vector2 destination = origin + new Vector2(xDir, yDir);

        Vector2 start;
        Vector2 end;

        Vector2 start2;
        Vector2 end2;

        if (xDir == 0) {
            end2 = new Vector2(xDir + 0.5f, yDir * 1.5f) + origin;
            end = new Vector2(xDir - 0.5f, yDir * 1.5f) + origin;
            start2 = new Vector2(transform.position.x + 0.5f, transform.position.y);
            start = new Vector2(transform.position.x - 0.5f, transform.position.y);
        }
        else if (yDir == 0)
        {
            end2 = new Vector2(xDir * 1.5f, yDir + 0.5f) + origin;
            end = new Vector2(xDir * 1.5f, yDir - 0.5f) + origin;
            start2 = new Vector2(transform.position.x, transform.position.y + 0.5f);
            start = new Vector2(transform.position.x, transform.position.y - 0.5f);
        }
        else
        {
            end2 = destination;
            end = destination;
            start = origin;
            start2 = origin;
        }

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);

        hit2 = Physics2D.Linecast(start2, end2, blockingLayer);

        boxCollider.enabled = true;

        if (hit.transform == null && hit2.transform == null)
        {
            StartCoroutine(SmoothMovement(destination));
            return true;
        }

        return false;
    }
}

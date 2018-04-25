using UnityEngine;
using System.Collections;

public class EnemySummoner : Enemy
{

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

            if (!BoardManager.instance.ObjectPositionTaken(posX + 1, posY) && BoardManager.instance.FloorPositionTaken(posX + 1, posY) && !GameManager.instance.SpotClaimed(new Vector2(posX + 1, posY)))
            {
                summonDirection = new Vector2(posX + 1, posY);
            }
            else if (!BoardManager.instance.ObjectPositionTaken(posX, posY + 1) && BoardManager.instance.FloorPositionTaken(posX, posY + 1) && !GameManager.instance.SpotClaimed(new Vector2(posX, posY + 1)))
            {
                summonDirection = new Vector2(posX, posY + 1);
            }
            else if (!BoardManager.instance.ObjectPositionTaken(posX, posY - 1) && BoardManager.instance.FloorPositionTaken(posX, posY - 1) && !GameManager.instance.SpotClaimed(new Vector2(posX, posY - 1)))
            {
                summonDirection = new Vector2(posX, posY - 1);
            }
            else if (!BoardManager.instance.ObjectPositionTaken(posX - 1, posY) && BoardManager.instance.FloorPositionTaken(posX - 1, posY) && !GameManager.instance.SpotClaimed(new Vector2(posX - 1, posY)))
            {
                summonDirection = new Vector2(posX - 1, posY);
            }
            else
            {
                enemyCount--;
            }

        }

        base.SetupNextMove();

        counter++;

    }

    public override void MoveEnemy() {

        if ((int)summonDirection.x != 0 || (int)summonDirection.y != 0)
            BoardManager.instance.AddEnemy(summonDirection.x, summonDirection.y, enemyToSummon);

        summonDirection = new Vector2();

        base.MoveEnemy();

    }

}
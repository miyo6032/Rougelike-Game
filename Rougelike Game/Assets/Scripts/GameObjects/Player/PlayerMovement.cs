﻿using UnityEngine;

//Handles the player movement and when the player can't move
public class PlayerMovement : MovingObject
{
    PlayerStats stats;
    PlayerAnimation animatorHandler;
    bool hitting;

    protected override void Start() {
        base.Start();
        stats = GetComponent<PlayerStats>();
        animatorHandler = GetComponent<PlayerAnimation>();
    }

    void Update() {
        if (moving || hitting || Time.timeScale == 0) return;

        Vector2Int input = new Vector2Int((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));

        //Keep the player from going diagonal, however epic that would be
        if(input.x != 0)
            input.y = 0;

        //Update the animator to align with the player's input
        animatorHandler.SetAnimationDirection(input);

        //Execute the MOVE
        if (input.x != 0 || input.y != 0) {
            //check to make sure the spot is not claimed - and keeps enemies and players from moving into the same spot.
            if (!moveManager.SpotClaimed(Vector2Int.RoundToInt((Vector2)transform.position + (Vector2)input * moveScale)))
                AttemptMove<EnemyStats>(input.x, input.y);//The player moves (or at least tries to)
        }

    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //Hit to see if something is in the way
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
        {
            //We were able to move, so animate that movement
            animatorHandler.AnimateMovement(new Vector2(xDir, yDir));
            StaticCanvasList.instance.chestInventory.CloseChest();
            return;
        }

        //Get the specified component
        T hitComponent = hit.transform.GetComponent<T>();

        //Only call OnCantMove if the hit actually has the component
        if (!canMove && hitComponent != null)
        {
            Attack(hitComponent);
        }
        else
        {
            Chest chest = hit.transform.GetComponent<Chest>();
            if (!canMove && chest != null)
            {
                StaticCanvasList.instance.chestInventory.OpenChest(chest);
            }
            animatorHandler.SetIdle(new Vector2(xDir, yDir));
        }
    }

    protected override void Attack<T>(T component) {
        if (hitting) return;
        hitting = true;
        Invoke("HittingFalse", stats.hitSpeed);
        animatorHandler.AnimateAttack();
        EnemyStats hitEnemy = component as EnemyStats;
        hitEnemy.DamageEnemy(Random.Range(stats.minAttack, stats.maxAttack + 1));
    }

    //Called by invoke with a delay to keep hitting timed
    void HittingFalse()
    {
        hitting = false;
    }

}
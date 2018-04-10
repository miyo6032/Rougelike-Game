using UnityEngine;

//Handles the player movement and when the player can't move
public class PlayerMovement : MovingObject
{
    PlayerStats stats;
    bool hitting;

    protected override void Start() {
        base.Start();
        stats = GetComponent<PlayerStats>();
    }

    void Update() {

        if (moving) return;

        Vector2Int input = new Vector2Int((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical"));

        //Keep the player from going diagonal, however epic that would be
        if(input.x != 0)
            input.y = 0;

        //Execute the MOVE
        if (input.x != 0 || input.y != 0) {
            //check to make sure the spot is not claimed - and keeps enemies and players from moving into the same spot.
            if (!moveManager.SpotClaimed(Vector2Int.RoundToInt((Vector2)transform.position + (Vector2)input * moveScale)))
                AttemptMove<EnemyStats>(input.x, input.y);//The player moves (or at least tries to)
        }

    }

    //When the player collides with something, anything
    protected override void OnCantMove<T>(T component) {
        if (hitting) return;
        hitting = true;
        Invoke("HittingFalse", stats.hitSpeed);
        EnemyStats hitEnemy = component as EnemyStats;
        hitEnemy.DamageEnemy(stats.attack);
    }

    //Called by invoke with a delay to keep hitting timed
    void HittingFalse()
    {
        hitting = false;
    }

}

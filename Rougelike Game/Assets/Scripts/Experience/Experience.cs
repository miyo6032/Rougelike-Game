using UnityEngine;

/// <summary>
/// The experience gameobject
/// </summary>
public class Experience : PlayerEnterDetector
{
    public float spawnVel;
    public int experienceAmount;

    void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-spawnVel, spawnVel), Random.Range(-spawnVel, spawnVel)), ForceMode2D.Impulse);
    }

    public override void PlayerEnter(Collider2D player)
    {
        player.GetComponent<PlayerStats>().AddXP(experienceAmount);
        Destroy(gameObject);
    }
}

using UnityEngine;

/// <summary>
/// The downstairs trigger for the dungeon
/// </summary>
public class DungeonDownstairs : PlayerEnterDetector
{
    public override void PlayerEnter(Collider2D player)
    {
        DungeonGenerator.Instance.Downstairs();
    }
}

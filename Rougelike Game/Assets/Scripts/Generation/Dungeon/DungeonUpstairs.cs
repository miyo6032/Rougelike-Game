using UnityEngine;

/// <summary>
/// The upstairs trigger for a dungeon
/// </summary>
public class DungeonUpstairs : PlayerEnterDetector
{
    public override void PlayerEnter(Collider2D player)
    {
        DungeonGenerator.Instance.Upstairs();
    }
}

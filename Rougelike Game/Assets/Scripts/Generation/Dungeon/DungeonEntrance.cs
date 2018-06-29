using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the entrance of the dungeon - also holds the dungeon data
/// </summary>
public class DungeonEntrance : PlayerEnterDetector
{
    public List<DungeonLevel> DungeonLevels;

    public override void PlayerEnter(Collider2D player)
    {
        DungeonGenerator.Instance.Enter(DungeonLevels);
        DungeonGenerator.Instance.DungeonEntrance = transform;
    }
}

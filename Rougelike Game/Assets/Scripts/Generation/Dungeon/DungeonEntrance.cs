using System.Collections.Generic;

/// <summary>
/// Represent the entrance of the dungeon - also holds the dungeon data
/// </summary>
public class DungeonEntrance : PlayerEnterDetector
{
    public List<DungeonLevel> DungeonLevels;

    public override void PlayerEnter()
    {
        DungeonGenerator.Instance.Enter(DungeonLevels);
        DungeonGenerator.Instance.DungeonEntrance = transform;
    }
}

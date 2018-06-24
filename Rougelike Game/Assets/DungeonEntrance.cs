using System.Collections.Generic;
using UnityEngine;

public class DungeonEntrance : PlayerEnterDetector
{
    public List<DungeonLevel> DungeonLevels;

    public override void PlayerEnter()
    {
        DungeonGenerator.Instance.Enter(DungeonLevels);
        DungeonGenerator.Instance.DungeonEntrance = transform;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates the dungeon level based on what level the player is in
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    private List<DungeonLevel> DungeonLevels;
    private int CurrentLevel;
    public DungeonLevelGenerator DungeonLevelGenerator;
    public PlayerMovement player;
    public static DungeonGenerator Instance;
    public Transform DungeonEntrance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            throw new Exception("Duplicate Dungeon Generator!");
        }
    }

    /// <summary>
    /// Enter the first level of the dungeon
    /// </summary>
    public void Enter(List<DungeonLevel> dungeonLevels)
    {
        DungeonLevels = dungeonLevels;
        CurrentLevel = -1;
        Downstairs();
    }

    /// <summary>
    /// Send the player downstairs
    /// </summary>
    public void Downstairs()
    {
        CurrentLevel++;
        if (CurrentLevel == DungeonLevels.Count)
        {
            DungeonLevelGenerator.GenerateBottom();
        }
        else
        {
            GenerateLevel(CurrentLevel);
        }
        // Teleport the player just to the left of the stairs
        player.TeleportPlayer(DungeonLevelGenerator.dungeonExits.ToVector2()[0] + new Vector2(-1, 0));
    }

    /// <summary>
    /// Send the player upstairs
    /// </summary>
    public void Upstairs()
    {
        CurrentLevel--;
        Debug.Log(CurrentLevel);
        if (CurrentLevel == -1)
        {
            Exit();
        }
        else
        {
            GenerateLevel(CurrentLevel);
            // Teleport the player just right of the stairs
            player.TeleportPlayer(DungeonLevelGenerator.dungeonExits.ToVector2()[1] + new Vector2(1, 0));
        }
    }

    private void Exit()
    {
        // Teleport the player just to the left of the exit
        player.TeleportPlayer(new Vector3(Mathf.FloorToInt(DungeonEntrance.position.x), Mathf.FloorToInt(DungeonEntrance.position.y)) + new Vector3(1, 0, 0));
        DungeonLevelGenerator.ClearDungeon();
        DungeonLevels = null;
    }

    private void GenerateLevel(int level)
    {
        DungeonLevelGenerator.DungeonLevel = DungeonLevels[level];
        DungeonLevelGenerator.Generate();
    }
}

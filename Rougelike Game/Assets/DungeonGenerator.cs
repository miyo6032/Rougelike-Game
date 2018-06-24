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

    void Awake()
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

        PlacePlayerInDungeon(new Vector3(-1, 0));
    }

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
            PlacePlayerInDungeon(new Vector3(1, 0));
        }
    }

    private void PlacePlayerInDungeon(Vector3 vec3)
    {
        player.EmergencyStop();
        player.transform.position = 
            new Vector3(Mathf.FloorToInt((float)DungeonLevelGenerator.dungeonExits.v0.x), Mathf.FloorToInt((float)DungeonLevelGenerator.dungeonExits.v0.y)) + vec3;
    }

    private void Exit()
    {
        player.EmergencyStop();
        player.transform.position = new Vector3(Mathf.FloorToInt(DungeonEntrance.position.x), Mathf.FloorToInt(DungeonEntrance.position.y)) + new Vector3(1, 0, 0);
        DungeonLevelGenerator.ClearDungeon();
        DungeonLevels = null;
    }

    private void GenerateLevel(int level)
    {
        DungeonLevelGenerator.DungeonLevel = DungeonLevels[level];
        DungeonLevelGenerator.Generate();
    }

}

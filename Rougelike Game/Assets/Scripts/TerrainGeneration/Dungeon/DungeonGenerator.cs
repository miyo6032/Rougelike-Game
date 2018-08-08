using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates the dungeon level based on what level the player is in
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    private List<DungeonLevel> DungeonLevels;
    private int currentLevel;
    public PlayerMovement player;
    public static DungeonGenerator Instance;
    public Effect dungeonCurseEffect;
    private Transform DungeonEntrance;
    private int lowestLevel;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            throw new Exception("Duplicate Dungeon Generator!");
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Enter the first level of the dungeon
    /// </summary>
    public void Enter(List<DungeonLevel> dungeonLevels, Transform transform)
    {
        DescensionPanel.instance.StartAnimation("Descending");
        DungeonEntrance = transform;
        DungeonLevels = dungeonLevels;
        currentLevel = -1;
        gameObject.SetActive(true);
        Downstairs();
    }

    /// <summary>
    /// Send the player downstairs
    /// </summary>
    public void Downstairs()
    {
        DescensionPanel.instance.StartAnimation("Descending");
        currentLevel++;
        if (currentLevel > lowestLevel)
        {
            lowestLevel = currentLevel;
        }
        if (currentLevel == DungeonLevels.Count)
        {
            DungeonLevelGenerator.instance.GenerateBottom();
        }
        else
        {
            GenerateLevel(currentLevel);
        }
        ApplyDungeonCurse();
        // Teleport the player just to the left of the stairs
        player.TeleportPlayer(DungeonLevelGenerator.instance.dungeonExits.ToVector2()[0] + new Vector2(-1, 0));
    }

    /// <summary>
    /// Send the player upstairs
    /// </summary>
    public void Upstairs()
    {
        DescensionPanel.instance.StartAnimation("Ascending");
        currentLevel--;
        if (currentLevel == -1)
        {
            Exit();
        }
        else
        {
            GenerateLevel(currentLevel);
            // Teleport the player just right of the stairs
            player.TeleportPlayer(DungeonLevelGenerator.instance.dungeonExits.ToVector2()[1] + new Vector2(1, 0));
            ApplyDungeonCurse();
        }
    }

    private void ApplyDungeonCurse()
    {
        EffectManager.instance.RemoveEffectBySource(this);
        for (int i = 0; i < dungeonCurseEffect.ModifiersAffected.Length; i++)
        {
            float curseValue = 10 * (currentLevel - lowestLevel);
            // Don't apply any effect if the value is 0
            if (curseValue == 0) return;
            dungeonCurseEffect.ModifiersAffected[i].value = curseValue;
        }
        EffectManager.instance.AddNewEffect(dungeonCurseEffect, this);
    }

    private void Exit()
    {
        // Teleport the player just to the left of the exit
        player.TeleportPlayer(new Vector3(Mathf.FloorToInt(DungeonEntrance.position.x), Mathf.FloorToInt(DungeonEntrance.position.y)) + new Vector3(1, 0, 0));
        DungeonLevelGenerator.instance.ClearTilemap();
        DungeonLevels = null;
        gameObject.SetActive(false);
    }

    private void GenerateLevel(int level)
    {
        DungeonLevelGenerator.instance.DungeonLevel = DungeonLevels[level];
        DungeonLevelGenerator.instance.Generate();
    }
}
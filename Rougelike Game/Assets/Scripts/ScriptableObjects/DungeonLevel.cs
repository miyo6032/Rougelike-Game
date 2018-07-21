using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The DungeonLevel scriptable object that allows a DungeonLevel floor to be customized
/// </summary>
[CreateAssetMenu(menuName = "Custom/DungeonLevel")]
public class DungeonLevel : ScriptableObject
{
    [Header("Dungeon Materials")]
    public TileType FloorTile;
    public TileType WallTile; // The tile that will be the wall texture
    public TileType FreeStandingWallTile; // A freestanding wall tile
    public TileType DecorativeFloorTile;
    public TileType FreeStandingDecor;
    public Sprite UpStairs;
    public Sprite DownStairs;
    public Sprite Door;
    public Sprite HiddenDoor;
    [Header("Decoration")]
    [Range(0, 1)]
    public float FloorDecorationDensity;
    [Range(0, 1)]
    public float HiddenDoorDensity;
    [Range(0, 100)]
    public int FreeStandingDecorationCount;
    [Header("Dungeon Size")]
    [Range(3, 100)]
    public int Height = 3;
    [Range(3, 100)]
    public int Width = 3;
    [Header("Room Generation")]
    [Range(1, 500)]
    public int InitialRoomDensity = 1;
    [Range(0, 1)]
    public float RoomConnectedness;
    [Range(1, 10)]
    public int HallwaySize = 1;
    [Range(3, 30)]
    public int MinRoomHeight = 3;
    [Range(3, 30)]
    public int MaxRoomHeight = 3;
    [Range(3, 30)]
    public int MinRoomWidth = 3;
    [Range(3, 30)]
    public int MaxRoomWidth = 3;
    [Header("Enemies")]
    [Range(0, 100)]
    public int EnemiesPerLevel;
    public EnemyType Enemies;
    [Header("Chests")]
    [Range(0, 30)]
    public int ChestsPerLevel;
    [Range(1, 30)]
    public int ChestLevel = 1;
    [Header("Cave")]
    public TileType CaveFloor;
    public TileType CaveFreeStandingDecor;
    [Range(0, 100)]
    public int CaveDecorationCount;
    [Range(0, 100)]
    public int fillPercent;
    [Range(0, 10)]
    public int smoothingCycles;
    [Range(0, 8)]
    public int birthLimit;
    [Range(0, 8)]
    public int deathLimit;
    [Range(0, 100)]
    public int wallThresholdSize = 50;
    [Range(0, 100)]
    public int roomThresholdSize = 50;
    public string seed;
    public bool useRandomSeed = true;
}

[System.Serializable]
public class WeightedEnemy
{
    public Enemy enemy;
    [Range(1, 100)]
    public int weight = 1;
}

[System.Serializable]
public class EnemyType
{
    public WeightedEnemy[] enemies;

    public Enemy GetEnemy()
    {
        List<Enemy> e = new List<Enemy>();
        foreach (var weightedEnemy in enemies)
        {
            for (int i = 0; i < weightedEnemy.weight; i++)
            {
                e.Add(weightedEnemy.enemy);
            }
        }

        return e[Random.Range(0, e.Count)];
    }
}

using UnityEngine;
using System.Collections.Generic;
using Microsoft.Win32;

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
    public Sprite UpStairs;
    public Sprite DownStairs;
    public Sprite Door;
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
    [Range(0, 10)]
    public int MinEnemiesPerRoom;
    [Range(0, 10)]
    public int MaxEnemiesPerRoom;
    public EnemyType Enemies;
    [Header("Chests")]
    [Range(0, 30)]
    public int MinChestPerLevel;
    [Range(0, 30)]
    public int MaxChestsPerLevel;
    [Range(1, 30)]
    public int ChestLevel = 1;
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

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The DungeonLevel scriptable object that allows a DungeonLevel floor to be customized
/// </summary>
[CreateAssetMenu(menuName = "Custom/DungeonLevel")]
public class DungeonLevel : ScriptableObject
{
    public TileType floorTile;
    public TileType wallTile; // The tile that will be the wall texture
    public TileType freeStandingWallTile; // A freestanding wall tile
    public TileType voidTile; // The tile that will be a empty void tile
    public Sprite upStairs;
    public Sprite downStairs;
    public Sprite door;
    public Sprite openedDoor;
    public int Height;
    public int Width;
    public int InitialRoomDensity;
    public float RoomConnectedness;
    public int HallwaySize;
    public Vector2Int RoomHeightBounds;
    public Vector2Int RoomWidthBounds;
    public EnemyType Enemies;
    public Vector2Int enemiesPerRoom;
    public Vector2Int chestsPerLevel;
    public int chestLevel;
}

[System.Serializable]
public class WeightedEnemy
{
    public Enemy enemy;
    public int weight;
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

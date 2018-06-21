using UnityEngine;

/// <summary>
/// The dungeon scriptable object that allows a dungeon floor to be customized
/// </summary>
[CreateAssetMenu(menuName = "Custom/Dungeon")]
public class Dungeon : ScriptableObject
{
    public TileType floorTile;
    public TileType wallTile; // The tile that will be the wall texture
    public TileType freeStandingWallTile; // A freestanding wall tile
    public TileType voidTile; // The tile that will be a empty void tile
    public Sprite upStairs;
    public Sprite downStairs;
    public Sprite door;
    public int Height;
    public int Width;
    public int InitialRoomDensity;
    public float RoomConnectedness;
    public int HallwaySize;
    public Vector2Int RoomHeightBounds;
    public Vector2Int RoomWidthBounds;
}

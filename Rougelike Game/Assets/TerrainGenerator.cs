using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tiles
{
    floorTile,
    wallTile,
    voidTile,
    freeStandingWallTile,
}

/// <summary>
/// Base class for terrain in editor generators
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    public Tilemap floor;
    public Tilemap walls;
    public Tile floorTile;
    public Tile wallTile; // The tile that will be the wall texture
    public Tile freeStandingWallTile; // A freestanding wall tile
    public Tile voidTile; // The tile that will be a empty void tile

    // Called to generate the entire map from start to finish
    public virtual void Generate()
    {

    }

}

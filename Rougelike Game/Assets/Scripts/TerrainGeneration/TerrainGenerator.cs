using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum Tiles
{
    floorTile,
    wallTile,
    voidTile,
    holeTile,
    liquidTile,
    freeStandingWallTile,
    caveTile,
}

/// <summary>
/// Base class for terrain in editor generators
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    public Tilemap floor;
    public Tilemap walls;
    public Tilemap upperFloor;

    // Called to generate the entire map from start to finish
    public virtual void Generate()
    {
    }

    public virtual void ClearTilemap()
    {
        floor.ClearAllTiles();
        walls.ClearAllTiles();
        upperFloor.ClearAllTiles();
    }
}

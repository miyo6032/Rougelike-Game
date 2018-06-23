﻿using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum Tiles
{
    floorTile,
    wallTile,
    voidTile,
    freeStandingWallTile,
}

[System.Serializable]
public class WeightedTile
{
    public Tile tile;
    public int weight = 1;
}

[System.Serializable]
public class TileType
{
    public WeightedTile[] tiles;

    public Tile GetTile()
    {
        List<Tile> t = new List<Tile>();
        foreach (var weightedTile in tiles)
        {
            for (int i = 0; i < weightedTile.weight; i++)
            {
                t.Add(weightedTile.tile);
            }
        }

        return t[Random.Range(0, t.Count)];
    }
}

/// <summary>
/// Base class for terrain in editor generators
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    public Tilemap floor;
    public Tilemap walls;

    // Called to generate the entire map from start to finish
    public virtual void Generate()
    {
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates basic cave shapes in the form of a list of vectors
/// Based on Sebastian Lague's cellular automata youtube series
/// </summary>
public class CellularAutomata
{
    private int width;
    private int height;

    private int smoothingCycles;

    private int birthLimit;
    private int deathLimit;

    private int wallThresholdSize = 50;
    private int roomThresholdSize = 50;

    private string seed;
    private bool useRandomSeed = true;

    private int fillPercent;

    private int[,] map;

    public CellularAutomata(int width, int height, int fillPercent, int smoothingCycles, int birthLimit, int deathLimit, int wallThresholdSize, int roomThresholdSize, bool useRandomSeed)
    {
        this.width = width;
        this.height = height;
        this.fillPercent = fillPercent;
        this.smoothingCycles = smoothingCycles;
        this.birthLimit = birthLimit;
        this.deathLimit = deathLimit;
        this.wallThresholdSize = wallThresholdSize;
        this.roomThresholdSize = roomThresholdSize;
        this.useRandomSeed = useRandomSeed;
    }

    public CellularAutomata(DungeonLevel level)
    {
        this.width = level.Width;
        this.height = level.Height;
        this.fillPercent = level.fillPercent;
        this.smoothingCycles = level.smoothingCycles;
        this.birthLimit = level.birthLimit;
        this.deathLimit = level.deathLimit;
        this.wallThresholdSize = level.wallThresholdSize;
        this.roomThresholdSize = level.roomThresholdSize;
        this.useRandomSeed = level.useRandomSeed;
    }

    public List<List<Vector2Int>> Generate()
    {
        map = new int[width, height];
        Init();
        for (int i = 0; i < smoothingCycles; i++)
        {
            int[,] newMap = new int[width, height];
            Smooth(newMap);
            map = newMap;
        }
        CleanRegions();
        return GetRegions(0);
    }

    /// <summary>
    /// Initialize the map with a random 1's and 0's
    /// </summary>
    private void Init()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random random = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = random.Next(0, 100) < fillPercent ? 1 : 0;
                }
            }
        }
    }

    /// <summary>
    /// Change the map 1's and 0's based on their neighbors
    /// </summary>
    private void Smooth(int[,] map)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int count = GetSurroundingWallCount(x, y);
                if (count >= birthLimit)
                {
                    map[x, y] = 1;
                }
                else if (count <= deathLimit)
                {
                    map[x, y] = 0;
                }
            }
        }
    }

    /// <summary>
    /// Gets the amount of walls nearby
    /// </summary>
    private int GetSurroundingWallCount(int x, int y)
    {
        int wallCount = 0;
        for (int neighborX = x - 1; neighborX <= x + 1; neighborX++)
        {
            for (int neighborY = y - 1; neighborY <= y + 1; neighborY++)
            {
                if (InMap(neighborX, neighborY))
                {
                    if (neighborX != x || neighborY != y)
                    {
                        wallCount += map[neighborX, neighborY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    /// <summary>
    /// Remove small fragment region walls and rooms
    /// </summary>
    private void CleanRegions()
    {
        List<List<Vector2Int>> wallRegions = GetRegions(1);

        foreach (List<Vector2Int> region in wallRegions)
        {
            if (region.Count < wallThresholdSize)
            {
                foreach (Vector2Int tile in region)
                {
                    map[tile.x, tile.y] = 0;
                }
            }
        }

        List<List<Vector2Int>> roomRegions = GetRegions(0);

        foreach (List<Vector2Int> region in roomRegions)
        {
            if (region.Count < roomThresholdSize)
            {
                foreach (Vector2Int tile in region)
                {
                    map[tile.x, tile.y] = 1;
                }
            }
        }
    }

    /// <summary>
    /// Gets all of the regions of a certain tile type
    /// </summary>
    private List<List<Vector2Int>> GetRegions(int tileType)
    {
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        int[,] mapFlags = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                {
                    List<Vector2Int> region = GetRegionTiles(x, y);
                    regions.Add(region);
                    foreach (Vector2Int tile in region)
                    {
                        mapFlags[tile.x, tile.y] = 1;
                    }
                }
            }
        }
        return regions;
    }

    /// <summary>
    /// Gets all region tiles connected to the tile position passed in
    /// </summary>
    private List<Vector2Int> GetRegionTiles(int x, int y)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        int[,] mapFlags = new int[width, height];
        int tileType = map[x, y];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(x, y));
        mapFlags[x, y] = 1;

        while (queue.Count > 0)
        {
            Vector2Int tile = queue.Dequeue();
            tiles.Add(tile);
            for (int i = tile.x - 1; i <= tile.x + 1; i++)
            {
                for (int j = tile.y - 1; j <= tile.y + 1; j++)
                {
                    if (InMap(i, j) && (i == tile.x || j == tile.y))
                    {
                        if (mapFlags[i, j] == 0 && map[i, j] == tileType)
                        {
                            mapFlags[i, j] = 1;
                            queue.Enqueue(new Vector2Int(i, j));
                        }
                    }
                }
            }
        }
        return tiles;
    }

    /// <summary>
    /// If in the bounds of the map
    /// </summary>
    private bool InMap(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
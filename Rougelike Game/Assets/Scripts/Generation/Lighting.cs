using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The lighting engine for the player
/// </summary>
public class Lighting : MonoBehaviour
{
    public enum LightType
    {
        flood,
        ray
    }
    public Tilemap LightingTilemap;
    public Tilemap Walls;
    public int LightingRange;
    public LayerMask lightBlock;
    public LightType LightingType;

    private List<Vector2Int> circlePoints = new List<Vector2Int>(); // The circle for ray lighting
    private Dictionary<Vector2Int, bool> rayCollisions = new Dictionary<Vector2Int, bool>(); // The points where the ray collided with something. Used to remove artifacts
    private List<Vector2Int> litTiles = new List<Vector2Int>(); // Keeps track of all lit tiles so they can be unlit easily
    private List<Vector2Int> brightTiles = new List<Vector2Int>(); // Keeps track of all lit tiles so they can be unlit easily
    private Vector2Int[] directions = { Vector2Int.down, Vector2Int.up, Vector2Int.left, Vector2Int.right };

    void Start()
    {
        GetCirclePoints(LightingRange);
    }

    public void GenerateLight()
    {
        if (LightingType == LightType.flood)
        {
            FloodLight();
        }
        else if (LightingType == LightType.ray)
        {
            RayLight();
        }
    }

    /// <summary>
    /// Uses a bfs tree search algorithm to provide simple flood lighting
    /// </summary>
    private void FloodLight()
    {
        int detectionSize = LightingRange * 2;
        Vector2Int uninitializedVector = new Vector2Int(-1, -1);
        Vector2Int worldPos = Vector2Int.RoundToInt(transform.position);
        Vector2Int center = new Vector2Int(LightingRange, LightingRange);
        Vector2Int relativePos = worldPos - center;

        Dictionary<Vector2Int, Vector2Int> prev = new Dictionary<Vector2Int, Vector2Int>();

        // When lightStrength = 0, the tile is unlit
        Dictionary<Vector2Int, int> lightStrength = new Dictionary<Vector2Int, int>();

        for (int i = 0; i < detectionSize; i++)
        {
            for (int j = 0; j < detectionSize; j++)
            {
                prev.Add(new Vector2Int(i, j), uninitializedVector);
            }
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        queue.Enqueue(center);
        lightStrength.Add(center, LightingRange - 1);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // Bfs in the four directions up, down, left, and right
            foreach (var direction in directions)
            {
                Vector2Int adj = direction + current;
                if (WithinBounds(adj, detectionSize) && prev[adj] == uninitializedVector)
                {
                    prev[adj] = current;
                    if (!lightStrength.ContainsKey(adj))
                    {
                        lightStrength.Add(adj, lightStrength[current] - 1);
                    }

                    Color lighting = (lightStrength[adj] <= 0) ? new Color(0, 0, 0, 0.5f) : Color.clear;
                    SetTileColor(adj + relativePos, lighting);

                    // Don't enqueue if on a wall - stop light from flooding over walls
                    if (!ObjectExists(adj + relativePos))
                    {
                        queue.Enqueue(adj);
                    }
                }
            }
        }
    }

    private bool WithinBounds(Vector2Int pos, int bounds)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < bounds && pos.y < bounds;
    }

    /// <summary>
    /// Generate ray light by drawing brensemham lines between the player's position and the circle points surrounding the player
    /// </summary>
    private void RayLight()
    {
        DimLight();
        rayCollisions.Clear();

        foreach (var pos in circlePoints)
        {
            Vector2 end = pos + (Vector2)transform.position;
            BresenhamLine(Vector2Int.RoundToInt(transform.position), Vector2Int.RoundToInt(end));
        }

        FixLightingArtifacts();
    }

    /// <summary>
    /// Calculates the Brensenham line. Taken from: http://members.chello.at/~easyfilter/bresenham.html
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void BresenhamLine(Vector2Int start, Vector2Int end)
    {
        var dx = Mathf.Abs(end.x - start.x);
        var sx = -1;
        if (start.x < end.x)
        {
            sx = 1;
        }
        var dy = Mathf.Abs(end.y - start.y);
        var sy = -1;
        if (start.y < end.y)
        {
            sy = 1;
        }
        int err = -dy / 2;
        if (dx > dy)
        {
            err = dx / 2;
        }
        do
        {
            SetTileColor(start, Color.clear);
            if (ObjectExists(start))
            {
                if (!rayCollisions.ContainsKey(start))
                {
                    rayCollisions.Add(start, true);
                }
                return;
            }
            var e2 = err;
            if (e2 > -dx)
            {
                err -= dy;
                start.x += sx;
            }

            if (e2 < dy)
            {
                err += dx;
                start.y += sy;
            }
        } while (start.x != end.x || start.y != end.y);
    }

    /// <summary>
    /// Fixes certain lighting artifacts to make the lighting a little choppy
    /// </summary>
    private void FixLightingArtifacts()
    {
        int[] x = {1, 2, 1, 1};
        int[] y = {0, 0, -1, 1};
        foreach (var pos in rayCollisions.Keys)
        {
            CheckQuadrant(pos, x, y, 1);
            CheckQuadrant(pos, x, y, -1);
            CheckQuadrant(pos, y, x, 1);
            CheckQuadrant(pos, y, x, -1);
        }
    }

    /*
     * Detect artifacts of this nature: 'l' for lit and 'u' for unlit
     *
     * u l u -> u u u
     *
     * l u -> l l
     *   l      l
     *
     *   l      l
     * l u -> l l
     *
     * Of course, the added complexity is to check in all four directions
     */
    public void CheckQuadrant(Vector2Int pos, int[] x, int[] y, int sign)
    {
        Vector2Int pos0 = new Vector2Int(x[0], y[0]) * sign;
        Vector2Int pos1 = new Vector2Int(x[1], y[1]) * sign;
        Vector2Int pos2 = new Vector2Int(x[2], y[2]) * sign;
        Vector2Int pos3 = new Vector2Int(x[3], y[3]) * sign;
        Vector2Int unlit = pos + pos0;
        if ((rayCollisions.ContainsKey(pos + pos1) && !rayCollisions.ContainsKey(unlit)) ||
            (rayCollisions.ContainsKey(pos + pos2) && !rayCollisions.ContainsKey(unlit)) ||
            (rayCollisions.ContainsKey(pos + pos3) && !rayCollisions.ContainsKey(unlit))) {
            SetTileColor(unlit, Color.clear);
        }
    }

    /// <summary>
    /// Set the lighting tile's color (namely the alpha) to do the actual lighting effect
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="color"></param>
    private void SetTileColor(Vector2Int pos, Color color)
    {
        // Set tile flags to allow us to change the color
        LightingTilemap.SetTileFlags(new Vector3Int(pos.x, pos.y, 0), TileFlags.LockTransform);
        LightingTilemap.SetColor(new Vector3Int(pos.x, pos.y, 0), color);
        if (color == Color.clear)
        {
            litTiles.Add(pos);
            brightTiles.Add(pos);
        }
    }

    /// <summary>
    /// If a wall or door exists at this position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool ObjectExists(Vector2Int pos)
    {
        if (Physics2D.OverlapPoint(pos, lightBlock))
        {
            return true;
        }

        if (Walls.HasTile(new Vector3Int(pos.x, pos.y, 0)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Calculate a brensenham circle. Taken from: http://members.chello.at/~easyfilter/bresenham.html
    /// </summary>
    /// <param name="r"></param>
    private void GetCirclePoints(int r)
    {
        int x = -r;
        int y = 0;
        int err = 2 - 2 * r;

        do
        {
            circlePoints.Add(new Vector2Int(-x, y));
            circlePoints.Add(new Vector2Int(-y, -x));
            circlePoints.Add(new Vector2Int(x, -y));
            circlePoints.Add(new Vector2Int(y, x));
            r = err;
            if (r <= y) err += ++y * 2 + 1;
            if (r > x || err > y) err += ++x * 2 + 1;
        } while (x < 0);
    }

    /// <summary>
    /// Set the light to all unlit
    /// </summary>
    public void ResetLight()
    {
        foreach (var tile in litTiles)
        {
            SetTileColor(tile, Color.black);
        }
        litTiles.Clear();
        brightTiles.Clear();
    }

    /// <summary>
    /// Set the light to all unlit
    /// </summary>
    public void DimLight()
    {
        foreach (var tile in brightTiles)
        {
            SetTileColor(tile, new Color(0, 0, 0, 0.5f));
        }
        brightTiles.Clear();
    }
}

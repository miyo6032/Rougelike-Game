using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Simple flood lighting
/// </summary>
public class Lighting : MonoBehaviour
{
    public Tilemap LightingTilemap;
    public Tilemap Walls;
    public Tilemap Floor;
    public int LightingRange;
    public LayerMask lightBlock;

    public int Radius;

    private List<Vector2Int> circlePoints = new List<Vector2Int>();
    private Dictionary<Vector2Int, bool> rayCollisions = new Dictionary<Vector2Int, bool>();
    private Vector3Int[] directions = { Vector3Int.down, Vector3Int.up, Vector3Int.left, Vector3Int.right };

    void Start()
    {
        LightingTilemap.color = Color.black;
        GetCirclePoints(Radius);
    }

    public void FloodLight()
    {
        int detectionSize = LightingRange * 2;
        Vector3Int uninitializedVector = new Vector3Int(-1, -1, -1);
        Vector3Int worldPos = Vector3Int.RoundToInt(transform.position);
        Vector3Int center = new Vector3Int(LightingRange, LightingRange, 0);
        Vector3Int relativePos = worldPos - center;

        Vector3Int[,] prev = new Vector3Int[detectionSize, detectionSize];
        int[,] fogOpacity = new int[detectionSize, detectionSize];

        for (int i = 0; i < detectionSize; i++)
        {
            for (int j = 0; j < detectionSize; j++)
            {
                prev[i, j] = uninitializedVector;
            }
        }

        Queue<Vector3Int> queue = new Queue<Vector3Int>();

        queue.Enqueue(center);
        fogOpacity[center.x, center.y] = LightingRange - 1;

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            foreach (var direction in directions)
            {
                Vector3Int adj = direction + current;
                if (WithinBounds(adj, detectionSize) && prev[adj.x, adj.y] == uninitializedVector)
                {
                    prev[adj.x, adj.y] = current;
                    fogOpacity[adj.x, adj.y] = fogOpacity[current.x, current.y] - 1;
                    LightingTilemap.SetTileFlags(adj + relativePos, TileFlags.LockTransform);
                    Color lighting = (fogOpacity[adj.x, adj.y] <= 0) ? Color.black : Color.clear;
                    LightingTilemap.SetColor(adj + relativePos, lighting);
                    if (!Walls.HasTile(adj + relativePos) && !SomethingInTheWay(adj + relativePos))
                    {
                        queue.Enqueue(adj);
                    }
                }
            }
        }
    }

    public void RayLight()
    {
        ResetLight();
        rayCollisions.Clear();
        List<Vector2Int> lightRays = new List<Vector2Int>();

        foreach (var pos in circlePoints)
        {
            Vector2 end = pos + (Vector2)transform.position;
            RaycastHit2D ray = Physics2D.Linecast(transform.position, end, lightBlock);
            BresenhamLine(Vector2Int.RoundToInt(transform.position), Vector2Int.RoundToInt(end), lightRays);
        }

        FixLightingArtifacts();
    }

    private void LightWalls(Vector3Int pos)
    {
        LightingTilemap.SetTileFlags(pos, TileFlags.LockTransform);
        LightingTilemap.SetColor(pos, Color.clear);
        foreach (var direction in directions)
        {
            if (Walls.HasTile(pos + direction))
            {
                LightingTilemap.SetTileFlags(pos + direction, TileFlags.LockTransform);
                LightingTilemap.SetColor(pos + direction, Color.clear);
            }
        }
    }

    private void BresenhamLine(Vector2Int start, Vector2Int end, List<Vector2Int> lightRays)
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
        int infCounter = 0;
        do
        {
            LightingTilemap.SetTileFlags(new Vector3Int(start.x, start.y, 0), TileFlags.LockTransform);
            LightingTilemap.SetColor(new Vector3Int(start.x, start.y, 0), Color.clear);

            Vector2Int next = start;

            var e2 = err;
            if (e2 > -dx)
            {
                err -= dy;
                next.x += sx;
            }

            if (e2 < dy)
            {
                err += dx;
                next.y += sy;
            }

            if (ObjectExists(next))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(next.x, next.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(next.x, next.y, 0), Color.clear);
                if (!rayCollisions.ContainsKey(next))
                {
                    rayCollisions.Add(next, true);
                }
                return;
            }

            start = next;

            infCounter++;
            if (infCounter == 100)
            {
                Debug.Log("InfLoop!");
                break;
            }
        } while (start.x != end.x || start.y != end.y);
    }

    private void FixLightingArtifacts()
    {
        foreach (var pos in rayCollisions.Keys)
        {
            /*
             * Detect artifacts of this nature: 'l' for lit and 'u' for unlit
             *
             * l u l -> l l l
             *
             * l u -> l l
             *   l      l
             *
             *   l      l
             * l u -> l l
             *
             * Of course in all four directions
             */
            Vector2Int unlit = pos + new Vector2Int(1, 0);
            if (rayCollisions.ContainsKey(pos + new Vector2Int(2, 0)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
            else if (rayCollisions.ContainsKey(pos + new Vector2Int(1, -1)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
            else if (rayCollisions.ContainsKey(pos + new Vector2Int(1, 1)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
            unlit = pos + new Vector2Int(-1, 0);
            if (rayCollisions.ContainsKey(pos + new Vector2Int(-2, 0)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
            else if (rayCollisions.ContainsKey(pos + new Vector2Int(-1, -1)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
            else if (rayCollisions.ContainsKey(pos + new Vector2Int(-1, 1)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
            unlit = pos + new Vector2Int(0, 1);
            if (rayCollisions.ContainsKey(pos + new Vector2Int(0, 2)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
            else if (rayCollisions.ContainsKey(pos + new Vector2Int(-1, 1)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
            else if (rayCollisions.ContainsKey(pos + new Vector2Int(1, 1)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
            unlit = pos + new Vector2Int(0, -1);
            if (rayCollisions.ContainsKey(pos + new Vector2Int(0, -2)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
            else if (rayCollisions.ContainsKey(pos + new Vector2Int(1, -1)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
            else if (rayCollisions.ContainsKey(pos + new Vector2Int(-1, -1)) && !rayCollisions.ContainsKey(unlit))
            {
                LightingTilemap.SetTileFlags(new Vector3Int(unlit.x, unlit.y, 0), TileFlags.LockTransform);
                LightingTilemap.SetColor(new Vector3Int(unlit.x, unlit.y, 0), Color.clear);
            }
        }
    }

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

    public void OnDrawGizmos()
    {
        Vector3Int worldPos = Vector3Int.RoundToInt(transform.position);
        foreach (var pos in rayCollisions.Keys)
        {
            Gizmos.DrawSphere((Vector2)pos, 0.2f);
        }
    }

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

    private void SymmetricAdd(int x, int y)
    {
        circlePoints.Add(new Vector2Int(x, y));
        circlePoints.Add(new Vector2Int(-x, y));
        circlePoints.Add(new Vector2Int(x, -y));
        circlePoints.Add(new Vector2Int(-x, -y));
        circlePoints.Add(new Vector2Int(y, x));
        circlePoints.Add(new Vector2Int(-y, x));
        circlePoints.Add(new Vector2Int(y, -x));
        circlePoints.Add(new Vector2Int(-y, -x));
    }

    public void ResetLight()
    {
        int detectionSize = LightingRange * 2;
        Vector3Int worldPos = Vector3Int.RoundToInt(transform.position);
        Vector3Int center = new Vector3Int(LightingRange, LightingRange, 0);
        Vector3Int relativePos = worldPos - center;

        for (int i = 0; i < detectionSize; i++)
        {
            for (int j = 0; j < detectionSize; j++)
            {
                LightingTilemap.SetColor(relativePos + new Vector3Int(i, j, 0), Color.black);
            }
        }
    }

    private bool WithinBounds(Vector3Int pos, int bounds)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < bounds && pos.y < bounds;
    }

    private bool SomethingInTheWay(Vector3Int pos)
    {
        return Physics2D.OverlapPoint((Vector3)pos, lightBlock);
    }
}

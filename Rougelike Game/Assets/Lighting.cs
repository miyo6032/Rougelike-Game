using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Simple flood lighting
/// </summary>
public class Lighting : MonoBehaviour
{
    public Tilemap LightingTilemap;
    public Tilemap Walls;
    public int LightingRange;
    public LayerMask lightBlock;

    public int Radius;

    private List<Vector2Int> circlePoints = new List<Vector2Int>();
    private Vector3Int[] directions = { Vector3Int.down, Vector3Int.up, Vector3Int.left, Vector3Int.right };

    void Start()
    {
        LightingTilemap.color = Color.black;
        GetCirclePoints(Radius);
    }

	public void FloodLight ()
	{
        int detectionSize = LightingRange * 2;
        Vector3Int uninitializedVector = new Vector3Int(-1, -1, -1);
        Vector3Int worldPos = Vector3Int.RoundToInt(transform.position);
	    Vector3Int center = new Vector3Int(LightingRange, LightingRange, 0);
        Vector3Int relativePos = worldPos - center;

	    Vector3Int[,] prev = new Vector3Int[detectionSize, detectionSize];
	    int[,] fogOpacity = new int[detectionSize, detectionSize];

	    for(int i = 0; i < detectionSize; i++)
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
	                if(!Walls.HasTile(adj + relativePos) && !SomethingInTheWay(adj + relativePos)){
	                    queue.Enqueue(adj);
	                }
	            }
	        }
	    }
    }

    public void RayLight()
    {
        ResetLight();
        List<Vector2Int> lightRays = new List<Vector2Int>();

        Vector3Int worldPos = Vector3Int.FloorToInt(transform.position);

        foreach (var pos in circlePoints)
        {
            RaycastHit2D ray = Physics2D.Linecast((Vector3)worldPos, (Vector3)(worldPos + new Vector3Int(pos.x, pos.y, 0)), lightBlock);
            Vector2Int end = new Vector2Int(worldPos.x, worldPos.y) + pos;
            if (ray.point != Vector2.zero)
            {
                int x;
                int y;
                if (pos.x > 0)
                {
                    x = Mathf.CeilToInt(ray.point.x);
                }
                else
                {
                    x = Mathf.FloorToInt(ray.point.x);
                }

                if (pos.y > 0)
                {
                    y = Mathf.CeilToInt(ray.point.y);
                }
                else
                {
                    y = Mathf.FloorToInt(ray.point.y);
                }
                end = new Vector2Int(x, y);
            }
            BresenhamLine(new Vector2Int(worldPos.x, worldPos.y), end, lightRays);
        }

        foreach (var pos in lightRays)
        {
            Vector3Int vec3 = new Vector3Int(pos.x, pos.y, 0);
            LightingTilemap.SetTileFlags(vec3, TileFlags.LockTransform);
            LightingTilemap.SetColor(vec3, Color.clear);
        }
    }

    private void BresenhamLine(Vector2Int start, Vector2Int end, List<Vector2Int> lightRays)
    {
        int dx = Mathf.Abs(end.x - start.x);
        int sx = start.x < end.x ? 1 : -1;
        int dy = -Mathf.Abs(end.y - start.y);
        int sy = start.y < end.y ? 1 : -1;
        int err = dx + dy;
        int e2;
        for(;;)
        {
            lightRays.Add(new Vector2Int(start.x, start.y));
            if (start.x == end.x && start.y == end.y) break;
            e2 = 2 * err;
            if (e2 > dy) { err += dy; start.x += sx; } /* e_xy+e_x > 0 */
            if (e2 < dx) { err += dx; start.y += sy; } /* e_xy+e_y < 0 */
        }
    }

    public void OnDrawGizmos()
    {
        Vector3Int worldPos = Vector3Int.RoundToInt(transform.position);
        foreach (var pos in circlePoints)
        {
            RaycastHit2D ray = Physics2D.Linecast((Vector3)worldPos, (Vector3)(worldPos + new Vector3Int(pos.x, pos.y, 0)), lightBlock);
            Vector2Int end = new Vector2Int(worldPos.x, worldPos.y) + pos;
            if (ray.point != Vector2.zero)
            {
                end = Vector2Int.RoundToInt(ray.point);
            }
            Gizmos.DrawLine(worldPos, (Vector2)end);
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

using UnityEngine;
using TriangleNet.Geometry;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Get a triangiulates mst from Triangulator and make it fit for dungeon generation
/// </summary>
public class DungeonMst
{
    public VertexPair dungeonExits { get; private set; }
    private Triangulator triangulator;

    /// <summary>
    /// Get the dungeon graph from a list of vectors
    /// </summary>
    /// <param name="vectors"></param>
    /// <param name="connectedness"></param> Determines how connected the dungeon is - 0.0 being a minimum spanning tree
    /// <returns></returns>
    public Dictionary<Vector2Int, List<Vector2Int>> GetDungeonMap(List<Vector2Int> vectors, float connectedness)
    {
        triangulator = new Triangulator();
        triangulator.GenerateMst(vectors);
        dungeonExits = FindStartAndEnd();

        // Prime mst to be more like dungeon hallways
        RemoveMstFromGraph();
        RemoveLongShortEdges(Mathf.RoundToInt(triangulator.connectedVertices.Count / 3f));
        AddRandomEdgesToMst(Mathf.RoundToInt(triangulator.connectedVertices.Count * connectedness));

        // Convert back to vectors
        Dictionary<Vector2Int, List<Vector2Int>> links = new Dictionary<Vector2Int, List<Vector2Int>>();
        foreach (var kV in triangulator.mst)
        {
            List<Vector2Int> adj = new List<Vector2Int>();
            foreach (var vertex in kV.Value)
            {
                adj.Add(new Vector2Int(Mathf.RoundToInt((float) vertex.X), Mathf.RoundToInt((float) vertex.Y)));
            }

            links.Add(new Vector2Int(Mathf.RoundToInt((float) kV.Key.X), Mathf.RoundToInt((float) kV.Key.Y)), adj);
        }

        return links;
    }

    /// <summary>
    /// Finds the starting and ending point of the dungeon
    /// </summary>
    /// <returns></returns>
    VertexPair FindStartAndEnd()
    {
        Dictionary<Vertex, bool> visited = new Dictionary<Vertex, bool>();
        foreach (var vertex in triangulator.mst.Keys)
        {
            visited.Add(vertex, false);
        }

        // Get a random vertex and choose that to be the starting value
        Vertex start = triangulator.mst.Keys.ToList()[Random.Range(0, triangulator.mst.Keys.Count)];
        visited[start] = true;
        Queue<Vertex> queue = new Queue<Vertex>();
        queue.Enqueue(start);
        Vertex end = start;
        while (queue.Count > 0)
        {
            Vertex v = queue.Dequeue();
            foreach (var adj in triangulator.mst[v])
            {
                if (!visited[adj])
                {
                    visited[adj] = true;
                    queue.Enqueue(adj);
                    end = adj;
                }
            }
        }

        return new VertexPair(start, end);
    }

    /// <summary>
    /// Remove the mst from the connection to avoid re-choosing the edge
    /// </summary>
    void RemoveMstFromGraph()
    {
        foreach (var kV in triangulator.mst)
        {
            foreach (var vertex in kV.Value)
            {
                triangulator.connectedVertices[kV.Key].Remove(vertex);
            }
        }
    }

    /// <summary>
    /// Remove long and short edges from the connections so we don't choose really long or stubby connections accidentally
    /// </summary>
    /// <param name="num"></param>
    void RemoveLongShortEdges(int num)
    {
        num = Mathf.RoundToInt(num / 2f);
        for (int i = 0; i < num; i++)
        {
            float longestLength = 0;
            VertexPair longestEdge = null;
            float shortestLength = float.PositiveInfinity;
            VertexPair shortestEdge = null;
            foreach (var kV in triangulator.connectedVertices)
            {
                foreach (var vertex in kV.Value)
                {
                    VertexPair edge = new VertexPair(kV.Key, vertex);
                    float distance = triangulator.EdgeLength(edge);
                    if (distance > longestLength)
                    {
                        longestLength = distance;
                        longestEdge = edge;
                    }

                    if (distance < shortestLength)
                    {
                        shortestLength = distance;
                        shortestEdge = edge;
                    }
                }
            }

            triangulator.connectedVertices[longestEdge.v1].Remove(longestEdge.v0);
            triangulator.connectedVertices[longestEdge.v0].Remove(longestEdge.v1);
            triangulator.connectedVertices[shortestEdge.v1].Remove(shortestEdge.v0);
            triangulator.connectedVertices[shortestEdge.v0].Remove(shortestEdge.v1);
        }
    }

    /// <summary>
    /// Add in some edges back to make the dungeon more connected
    /// </summary>
    /// <param name="num"></param>
    void AddRandomEdgesToMst(int num)
    {
        for (int i = 0; i < num; i++)
        {
            List<VertexPair> allEdges = new List<VertexPair>();
            foreach (var kV in triangulator.connectedVertices)
            {
                foreach (var vertex in kV.Value)
                {
                    allEdges.Add(new VertexPair(kV.Key, vertex));
                }
            }

            VertexPair randomEdge = allEdges[Random.Range(0, allEdges.Count)];
            triangulator.mst[randomEdge.v0].Add(randomEdge.v1);
            triangulator.mst[randomEdge.v1].Add(randomEdge.v0);
        }
    }
}

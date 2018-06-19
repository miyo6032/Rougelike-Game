using UnityEngine;
using TriangleNet.Geometry;
using System.Collections.Generic;
using UnityEngine.Assertions.Comparers;

/// <summary>
/// Does delaunay triangulation to determine hallway connections between rooms
/// </summary>
public class Triangulator {

    public TriangleNet.Mesh mesh;

    private Dictionary<Vertex, List<Vertex>> connectedVertices;

    private Dictionary<Vertex, List<Vertex>> mst;

    /// <summary>
    /// Get hallway connections from a list of vectors by triangulating and finding a mst
    /// </summary>
    /// <param name="vectors"></param>
    /// <returns></returns>
    public Dictionary<Vector2Int, List<Vector2Int>> GetLinks(List<Vector2Int> vectors, float connectedness)
    {
        // Convert to vertices for triangulation
        List<Vertex> vertices = new List<Vertex>();
        vectors.ForEach((vector) => vertices.Add(new Vertex(vector.x, vector.y)));

        // Put into a polygon and triangulate
        Polygon polygon = new Polygon();
        vertices.ForEach((vertex) => polygon.Add(vertex));
        TriangleNet.Meshing.ConstraintOptions options = new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = true, SegmentSplitting = 2 };
        mesh = (TriangleNet.Mesh)polygon.Triangulate(options);

        //Convert the mesh into an adjacency list
        connectedVertices = GenerateConnectedVertices();

        // Generate the mst from the adjacenty list
        mst = JarnikPrimsMst();

        // Prime mst to be more like dungeon hallways
        RemoveMstFromGraph();
        RemoveLongShortEdges(Mathf.RoundToInt(connectedVertices.Count / 3f));
        AddRandomEdgesToMst(Mathf.RoundToInt(connectedVertices.Count * connectedness));

        // Convert back to vectors
        Dictionary<Vector2Int, List<Vector2Int>> links = new Dictionary<Vector2Int, List<Vector2Int>>();
        foreach (var kV in mst)
        {
            List<Vector2Int> adj = new List<Vector2Int>();
            foreach (var vertex in kV.Value)
            {
                adj.Add(new Vector2Int(Mathf.RoundToInt((float)vertex.X), Mathf.RoundToInt((float)vertex.Y)));
            }
            links.Add(new Vector2Int(Mathf.RoundToInt((float)kV.Key.X), Mathf.RoundToInt((float)kV.Key.Y)), adj);
        }

        return links;
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
            foreach (var kV in connectedVertices)
            {
                foreach (var vertex in kV.Value)
                {
                    allEdges.Add(new VertexPair(kV.Key, vertex));
                }
            }

            VertexPair randomEdge = allEdges[Random.Range(0, allEdges.Count)];
            mst[randomEdge.v0].Add(randomEdge.v1);
            mst[randomEdge.v1].Add(randomEdge.v0);
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
            foreach (var kV in connectedVertices)
            {
                foreach (var vertex in kV.Value)
                {
                    VertexPair edge = new VertexPair(kV.Key, vertex);
                    float distance = EdgeLength(edge);
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

            connectedVertices[longestEdge.v1].Remove(longestEdge.v0);
            connectedVertices[longestEdge.v0].Remove(longestEdge.v1);
            connectedVertices[shortestEdge.v1].Remove(shortestEdge.v0);
            connectedVertices[shortestEdge.v0].Remove(shortestEdge.v1);
        }
    }

    /// <summary>
    /// Remove the mst from the connection to avoid re-choosing the edge
    /// </summary>
    void RemoveMstFromGraph()
    {
        foreach (var kV in mst)
        {
            foreach (var vertex in kV.Value)
            {
                connectedVertices[kV.Key].Remove(vertex);
            }
        }
    }

    /// <summary>
    /// Find the mst of the triangulated vertices
    /// </summary>
    /// <returns></returns>
    Dictionary<Vertex, List<Vertex>> JarnikPrimsMst()
    {
        Dictionary<Vertex, Vertex> parents = new Dictionary<Vertex, Vertex>(); // Keeps track of the parents for every vertex
        Dictionary<Vertex, List<Vertex>> mst = new Dictionary<Vertex, List<Vertex>>(); // Put the edges into this dictionary

        foreach (var vertex in connectedVertices.Keys)
        {
            parents.Add(vertex, null);
        }

        Vertex root = mesh.vertices[0];
        parents[root] = root; // Make the root's parent not null

        mst.Add(root, new List<Vertex>());

        while (mst.Count < connectedVertices.Count)
        {
            VertexPair edge = GetNextSafeEdge(parents);

            //Add the new vertex - it has to be one or the other
            if (mst.ContainsKey(edge.v0))
            {
                mst.Add(edge.v1, new List<Vertex>());
                parents[edge.v1] = edge.v0;
            }
            else
            {
                mst.Add(edge.v0, new List<Vertex>());
                parents[edge.v0] = edge.v1;
            }

            //Add the edge that connects the two
            mst[edge.v0].Add(edge.v1);
            mst[edge.v1].Add(edge.v0);
        }

        return mst;
    }

    /// <summary>
    /// Gets the next safe edge for the mst
    /// </summary>
    /// <param name="parents"></param>
    /// <returns></returns>
    VertexPair GetNextSafeEdge(Dictionary<Vertex, Vertex> parents)
    {
        List<VertexPair> safeEdges = new List<VertexPair>();
        foreach (var kV in connectedVertices)
        {
            foreach (var vertex in kV.Value)
            {
                Vertex v0 = kV.Key;
                Vertex v1 = vertex;
                // If one vertex is a part of the mst, and one is not
                if ((parents[v0] == null && parents[v1] != null) || (parents[v1] == null && parents[v0] != null))
                {
                    safeEdges.Add(new VertexPair(v0, v1));
                }
            }
        }

        // Find the safe edge with the smallest distance
        VertexPair closestSafeEdge = null;
        float smallestDistance = float.PositiveInfinity;
        foreach (var edge in safeEdges)
        {
            float distance = EdgeLength(edge);
            if (distance < smallestDistance)
            {
                closestSafeEdge = edge;
                smallestDistance = distance;
            }
        }

        return closestSafeEdge;
    }

    /// <summary>
    /// Calculate length of an edge
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    float EdgeLength(VertexPair edge)
    {
        Vector2 vec1 = new Vector2((float)edge.v0.X, (float)edge.v0.Y);
        Vector2 vec2 = new Vector2((float)edge.v1.X, (float)edge.v1.Y);
        return Vector2.Distance(vec2, vec1);
    }

    /// <summary>
    /// Converts the mesh into a adjacency list
    /// </summary>
    /// <returns></returns>
    Dictionary<Vertex, List<Vertex>> GenerateConnectedVertices()
    {
        Dictionary<Vertex, List<Vertex>> connectedVertices = new Dictionary<Vertex, List<Vertex>>();

        foreach (var vertex in mesh.Vertices)
        {
            connectedVertices.Add(vertex, new List<Vertex>());
        }

        foreach (var edge in mesh.Edges)
        {
            Vertex v0 = mesh.vertices[edge.P0];
            Vertex v1 = mesh.vertices[edge.P1];
            connectedVertices[v0].Add(v1);
            connectedVertices[v1].Add(v0);
        }

        return connectedVertices;
    }

    class VertexPair
    {
        public Vertex v0;
        public Vertex v1;

        public VertexPair(Vertex v0, Vertex v1)
        {
            this.v0 = v0;
            this.v1 = v1;
        }
    }

}

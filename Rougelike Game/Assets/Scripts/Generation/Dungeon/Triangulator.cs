using UnityEngine;
using TriangleNet.Geometry;
using System.Collections.Generic;

/// <summary>
/// Does delaunay triangulation given vertices and returns an mst of that triangulation
/// </summary>
public class Triangulator
{
    private TriangleNet.Mesh mesh;
    public Dictionary<Vertex, List<Vertex>> connectedVertices { get; private set; }
    public Dictionary<Vertex, List<Vertex>> mst { get; private set; }

    /// <summary>
    /// Get hallway connections from a list of vectors by triangulating and finding a mst
    /// </summary>
    /// <param name="vectors"></param>
    /// <returns></returns>
    public void GenerateMst(List<Vector2Int> vectors)
    {
        // Convert to vertices for triangulation
        List<Vertex> vertices = new List<Vertex>();
        vectors.ForEach((vector) => vertices.Add(new Vertex(vector.x, vector.y)));

        // Put into a polygon and triangulate
        Polygon polygon = new Polygon();
        vertices.ForEach((vertex) => polygon.Add(vertex));
        TriangleNet.Meshing.ConstraintOptions options = new TriangleNet.Meshing.ConstraintOptions() {ConformingDelaunay = true, SegmentSplitting = 2};
        mesh = (TriangleNet.Mesh) polygon.Triangulate(options);

        //Convert the mesh into an adjacency list
        GenerateConnectedVertices();

        // Generate the mst from the adjacenty list
        JarnikPrimsMst();
    }

    /// <summary>
    /// Find the mst of the triangulated vertices
    /// </summary>
    /// <returns></returns>
    private void JarnikPrimsMst()
    {
        // Keeps track of the parents for every vertex
        Dictionary<Vertex, Vertex> parents = new Dictionary<Vertex, Vertex>();
        mst = new Dictionary<Vertex, List<Vertex>>();
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
    }

    /// <summary>
    /// Gets the next safe edge for the mst
    /// </summary>
    /// <param name="parents"></param>
    /// <returns></returns>
    private VertexPair GetNextSafeEdge(Dictionary<Vertex, Vertex> parents)
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
    public float EdgeLength(VertexPair edge)
    {
        Vector2 vec1 = new Vector2((float) edge.v0.X, (float) edge.v0.Y);
        Vector2 vec2 = new Vector2((float) edge.v1.X, (float) edge.v1.Y);
        return Vector2.Distance(vec2, vec1);
    }

    /// <summary>
    /// Converts the mesh into a adjacency list
    /// </summary>
    /// <returns></returns>
    private void GenerateConnectedVertices()
    {
        connectedVertices = new Dictionary<Vertex, List<Vertex>>();
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
    }
}
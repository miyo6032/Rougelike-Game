using UnityEngine;
using TriangleNet.Geometry;
using System.Collections.Generic;

public class TEST : MonoBehaviour {

    public TriangleNet.Mesh mesh;

    private Dictionary<Vertex, List<Edge>> connectedVertices;

    private Dictionary<Vertex, List<Edge>> mst;

    public Dictionary<Vertex, List<Edge>> Generate(List<Vertex> vertices)
    {
        Polygon polygon = new Polygon();
        foreach (var vertex in vertices)
        {
            polygon.Add(vertex);
        }
        TriangleNet.Meshing.ConstraintOptions options = new TriangleNet.Meshing.ConstraintOptions() { ConformingDelaunay = true, SegmentSplitting = 2};
        mesh = (TriangleNet.Mesh)polygon.Triangulate(options);

        connectedVertices = GenerateConnectedVertices();

        mst = JarnikPrimsMst();

        RemoveMst();
        RemoveLongEdges(Mathf.RoundToInt(connectedVertices.Count / 3f));
        AddRandomEdgesToMst(Mathf.RoundToInt(connectedVertices.Count / 3f));

        return mst;
    }

    void AddRandomEdgesToMst(int num)
    {
        for (int i = 0; i < num; i++)
        {
            List<Edge> allEdges = new List<Edge>();
            foreach (var edges in connectedVertices.Values)
            {
                foreach (var edge in edges)
                {
                    allEdges.Add(edge);
                }
            }

            Edge randomEdge = allEdges[Random.Range(0, allEdges.Count)];
            Vertex v0 = mesh.vertices[randomEdge.P0];
            Vertex v1 = mesh.vertices[randomEdge.P1];
            mst[v0].Add(randomEdge);
            mst[v1].Add(randomEdge);
        }
    }

    void RemoveLongEdges(int num)
    {
        for (int i = 0; i < num; i++)
        {
            float longestLength = 0;
            Edge longestEdge = null;
            foreach (var edges in connectedVertices.Values)
            {
                foreach (var edge in edges)
                {
                    float distance = Distance(edge);
                    if (distance > longestLength)
                    {
                        longestLength = distance;
                        longestEdge = edge;
                    }
                }
            }

            RemoveEdge(longestEdge);
        }
    }

    void RemoveMst()
    {
        foreach (var edges in mst.Values)
        {
            foreach (var edge in edges)
            {
                RemoveEdge(edge);
            }
        }
    }

    void RemoveEdge(Edge edge)
    {
        Vertex v0 = mesh.vertices[edge.P0];
        Vertex v1 = mesh.vertices[edge.P1];

        connectedVertices[v0].Remove(edge);
        connectedVertices[v1].Remove(edge);
    }

    Dictionary<Vertex, List<Edge>> JarnikPrimsMst()
    {
        Dictionary<Vertex, Vertex> parents = new Dictionary<Vertex, Vertex>(); // Keeps track of the parents for every vertex
        Dictionary<Vertex, List<Edge>> mst = new Dictionary<Vertex, List<Edge>>(); // Put the edges into this dictionary

        foreach (var vertex in connectedVertices.Keys)
        {
            parents.Add(vertex, null);
        }

        Vertex root = mesh.vertices[0];
        parents[root] = root; // Make the root's parent not null

        mst.Add(root, new List<Edge>());

        while (mst.Count < connectedVertices.Count)
        {
            Edge edge = GetNextSafeEdge(parents);
            Vertex v0 = mesh.vertices[edge.P0];
            Vertex v1 = mesh.vertices[edge.P1];

            //Add the new vertex - it has to be one or the other
            if (mst.ContainsKey(v0))
            {
                mst.Add(v1, new List<Edge>());
                parents[v1] = v0;
            }
            else
            {
                mst.Add(v0, new List<Edge>());
                parents[v0] = v1;
            }

            //Add the edge that connects the two
            mst[v0].Add(edge);
            mst[v1].Add(edge);
        }

        return mst;
    }

    Edge GetNextSafeEdge(Dictionary<Vertex, Vertex> parents)
    {
        List<Edge> safeEdges = new List<Edge>();
        foreach (var edges in connectedVertices.Values)
        {
            foreach (var edge in edges)
            {
                Vertex v0 = mesh.vertices[edge.P0];
                Vertex v1 = mesh.vertices[edge.P1];
                // If one vertex is a part of the mst, and one is not
                if ((parents[v0] == null && parents[v1] != null) || (parents[v1] == null && parents[v0] != null))
                {
                    safeEdges.Add(edge);
                }
            }
        }

        // Find the safe edge with the smallest distance
        Edge closestSafeEdge = null;
        float smallestDistance = float.PositiveInfinity;
        foreach (var edge in safeEdges)
        {
            float distance = Distance(edge);
            if (distance < smallestDistance)
            {
                closestSafeEdge = edge;
                smallestDistance = distance;
            }
        }

        return closestSafeEdge;
    }

    float Distance(Edge edge)
    {
        Vertex v0 = mesh.vertices[edge.P0];
        Vertex v1 = mesh.vertices[edge.P1];
        Vector2 vec1 = new Vector2((float) v0.X, (float) v0.Y);
        Vector2 vec2 = new Vector2((float) v1.X, (float) v1.Y);
        return Vector2.Distance(vec2, vec1);
    }

    Dictionary<Vertex, List<Edge>> GenerateConnectedVertices()
    {
        Dictionary<Vertex, List<Edge>> connectedVertices = new Dictionary<Vertex, List<Edge>>();
        
        foreach (var vertex in mesh.Vertices)
        {
            connectedVertices.Add(vertex, new List<Edge>());
        }

        foreach (var edge in mesh.Edges)
        {
            Vertex v0 = mesh.vertices[edge.P0];
            Vertex v1 = mesh.vertices[edge.P1];
            connectedVertices[v0].Add(edge);
            connectedVertices[v1].Add(edge);
        }

        return connectedVertices;
    }

}

using TriangleNet.Geometry;
using UnityEngine;

public class VertexPair
{
    public Vertex v0;
    public Vertex v1;

    public VertexPair(Vertex v0, Vertex v1)
    {
        this.v0 = v0;
        this.v1 = v1;
    }

    public Vector2[] ToVector2()
    {
        Vector2[] vecs = new Vector2[2];
        vecs[0] = new Vector2((float)v0.x, (float)v0.y);
        vecs[1] = new Vector2((float)v1.x, (float)v1.y);
        return vecs;
    }
}
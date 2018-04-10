using UnityEngine;

public class Vertex{

    public Vector2 position;
    public bool visited;
    public Vertex[] adj = new Vertex[4];
    public Vertex parent;
    private int adjIndex = 0;
    public bool isRoot = false;

    public void setVisited(bool b)
    {
        visited = b;
    }

    public void Add(Vertex v)
    {
        adj[adjIndex] = v;
        adjIndex++;
    }

    public int Count()
    {
        return adjIndex;
    }

}

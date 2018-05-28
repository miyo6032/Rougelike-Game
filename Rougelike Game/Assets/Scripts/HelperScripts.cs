using UnityEngine;
using System.Collections.Generic;

/*
 * Update - changes UI
 * 
 * */

//Random hodgepodge of scripts that help with many different things
public class HelperScripts {

    //Finds a component in exclusively the children of the parent (because the original doesn't do that)
    public static T GetComponentFromChildrenExc<T>(Transform parent)
    where T : Component
    {
        T[] children = parent.GetComponentsInChildren<T>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].transform != parent)
            {
                return children[i];
            }
        }
        return null;
    }

    //LINECAST our way to VICTORY
    //Uses a modified BFS algorithm to find the next move based on two positions
    public static Vector2 GetNextMove(Vector2Int currentPos, Vector2Int targetPos, LayerMask layers, int range, bool altMove)
    {

        //Helper directional vectors
        Vector2Int up = new Vector2Int(0, 1);
        Vector2Int down = new Vector2Int(0, -1);
        Vector2Int right = new Vector2Int(1, 0);
        Vector2Int left = new Vector2Int(-1, 0);

        //Now, tranform those position so we can work with them on the matrix from 0, 0 to n, n
        Vector2Int relCurPos = new Vector2Int(range, range);
        Vector2Int relTarPos = targetPos - currentPos + relCurPos;

        //Return no path - player out of vision
        if (Vector2.SqrMagnitude(targetPos - currentPos) > range * range)
        {
            return Vector2.positiveInfinity;
        }

        //The overall height/width of the search graph. 
        int boxSize = range * 2 + 1;

        //Initialize the visisted and predecessor arrays
        bool[,] visited = new bool[boxSize, boxSize];
        Vector2Int[,] pred = new Vector2Int[boxSize, boxSize];
        //Initialize Values
        for (int i = 0; i < boxSize; i++)
        {
            for (int j = 0; j < boxSize; j++)
            {
                visited[i, j] = false;
            }
        }

        Queue<Vector2Int> Q = new Queue<Vector2Int>();
        //Start at the player's position and work back to the enemy
        Q.Enqueue(relTarPos);
        //Do the search
        while (Q.Count > 0)
        {
            Vector2Int v = Q.Dequeue();
            if (!visited[v.x, v.y])
            {

                //Found the path
                if (v == relCurPos)
                {
                    return pred[v.x, v.y] - v;
                }

                visited[v.x, v.y] = true;

                //Helper directions
                Vector2Int[] adj = { (v + right), (v + left), (v + up), (v + down) };

                //Makes the movement a little more interesting - it justs make the enemy move diagonally rather than in L shape
                if (altMove)
                {
                    Vector2Int temp = adj[0];
                    adj[0] = adj[2];
                    adj[2] = temp;
                    temp = adj[1];
                    adj[1] = adj[3];
                    adj[3] = temp;
                }

                //Add all edges - all for of them using linecasts to make sure nothing is in the way
                for (int i = 0; i < 4; i++)
                {
                    //This statement checks the bounds
                    if (adj[i].y <= boxSize - 1 && adj[i].y >= 0 && adj[i].x <= boxSize - 1 && adj[i].x >= 0)
                    {
                        //Calculate the to and from of the line cast
                        Vector2 from = v + currentPos - relCurPos;
                        Vector2 to = adj[i] + currentPos - relCurPos;
                        //Line cast, converting back to real space rather than 1 x 1 space
                        if (Physics2D.Linecast(from, to , layers).transform == null)
                        {
                            if (!visited[adj[i].x, adj[i].y])
                            {
                                Q.Enqueue(adj[i]);
                                pred[adj[i].x, adj[i].y] = v;
                            }
                        }
                    }
                }
            }
        }
        return Vector2.positiveInfinity;
    }
}

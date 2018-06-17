﻿using UnityEngine;

public class Room
{
    public Vector2Int upperRightCorner;
    public Vector2Int lowerLeftCorner;

    public Room(Vector2Int lowerLeftCorner, Vector2Int upperRightCorner)
    {
        this.upperRightCorner = upperRightCorner;
        this.lowerLeftCorner = lowerLeftCorner;
    }

    public Vector2Int GetCenter()
    {
        return lowerLeftCorner + Vector2Int.CeilToInt(new Vector2(GetWidth() / 2f, GetHeight() / 2f));
    }

    public int GetHeight()
    {
        return Mathf.Abs(upperRightCorner.y - lowerLeftCorner.y);
    }

    public int GetWidth()
    {
        return Mathf.Abs(upperRightCorner.x - lowerLeftCorner.x);
    }
}
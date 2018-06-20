using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine.Tilemaps;

/// <summary>
/// Generate a dungeon floor
/// </summary>
public class DungeonGenerator : TerrainGenerator
{
    public GameObject upStairs;
    public GameObject downStairs;
    public GameObject door;
    public int Height;
    public int Width;
    public int InitialRoomDensity;
    public float RoomConnectedness;
    public int HallwaySize;
    public Vector2Int RoomHeightBounds;
    public Vector2Int RoomWidthBounds;
    private Tiles[,] map;
    private Dictionary<Vector2Int, List<Vector2Int>> links;
    private List<Edge> edges;
    private Transform mapGameObjects;
    private VertexPair dungeonExits;

    public override void Generate()
    {
        InitMap();
        GenerateDungeon(); ;
        MapToTilemap();
    }

    /// <summary>
    /// Clear the map
    /// </summary>
    void InitMap()
    {
        map = new Tiles[Width, Height];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                map[x, y] = Tiles.voidTile;
            }
        }
    }

    /// <summary>
    /// Generates the dungeon and writes it to the map 2d array
    /// </summary>
    void GenerateDungeon()
    {
        Triangulator triangulator = new Triangulator();
        RoomGenerator roomGenerator = new RoomGenerator();

        Dictionary<Vector2Int, Room> rooms = roomGenerator.GenerateRooms(InitialRoomDensity, Width, Height, RoomHeightBounds, RoomWidthBounds);
        links = triangulator.GetLinks(rooms.Keys.ToList(), RoomConnectedness);
        edges = LinksIntoHallways(rooms);
        dungeonExits = triangulator.dungeonExits;

        // Adds hubs
        foreach (var room in rooms.Values)
        {
            WriteRoomToMap(room);
        }

        foreach (var edge in edges)
        {
            WriteEdgeToMap(edge);
        }
    }

    /// <summary>
    /// Translates the edge into positions on the map array
    /// </summary>
    /// <param name="edge"></param>
    private void WriteEdgeToMap(Edge edge)
    {
        if (HallwaySize == 0) return;
        Vector2 step = edge.v1 - edge.v0;
        step.Normalize();
        Vector2Int start = edge.v0;
        int infCounter = 0;
        while (start != edge.v1)
        {
            for (int i = 0; i < HallwaySize; i++)
            {
                for (int j = 0; j < HallwaySize; j++)
                {
                    int x = Mathf.Clamp(start.x + i - Mathf.FloorToInt(HallwaySize / 2f), 0, Width - 1);
                    int y = Mathf.Clamp(start.y + j - Mathf.FloorToInt(HallwaySize / 2f), 0, Height - 1);
                    map[x, y] = Tiles.floorTile;
                }
            }
            start += Vector2Int.FloorToInt(step);
            infCounter++;
            if (infCounter > Height * Width)
            {
                return;
            }
        }
    }
    
    /// <summary>
    /// Translates a room into positions on the map array
    /// </summary>
    /// <param name="room"></param>
    private void WriteRoomToMap(Room room)
    {
        for (int y = 0; y < room.GetHeight() + 1; y++)
        {
            for (int x = 0; x < room.GetWidth() + 1; x++)
            {
                map[room.lowerLeftCorner.x + x, room.lowerLeftCorner.y + y] = Tiles.floorTile;
            }
        }
    }

    /// <summary>
    /// Turn linked vertices into hallways depending on the rooms
    /// </summary>
    List<Edge> LinksIntoHallways(Dictionary<Vector2Int, Room> rooms)
    {
        List<Edge> edges = new List<Edge>();
        RemoveDuplicates();
        foreach (var kV in links)
        {
            foreach (var vector in kV.Value)
            {
                if (RoomsAreAligned(rooms[kV.Key], rooms[vector]))
                {
                    Edge edge = GetLinkingEdge(rooms[kV.Key], rooms[vector]) ??
                                GetLinkingEdge(rooms[vector], rooms[kV.Key]);
                    edges.Add(edge);
                }
                else
                {
                    Edge xEdge = new Edge(kV.Key, new Vector2Int(vector.x, kV.Key.y));
                    Edge yEdge = new Edge(new Vector2Int(vector.x, kV.Key.y), vector);
                    edges.Add(yEdge);
                    edges.Add(xEdge);
                }
            }
        }

        return edges;
    }

    /// <summary>
    /// If the rooms can be connected by a straight hallway, without any turns
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    /// <returns></returns>
    bool RoomsAreAligned(Room room1, Room room2)
    {
        if (room1.lowerLeftCorner.x > room2.upperRightCorner.x)
        {
            if (room1.lowerLeftCorner.y > room2.upperRightCorner.y)
            {
                return false;
            }

            if (room1.upperRightCorner.y < room2.lowerLeftCorner.y)
            {
                return false;
            }
        }

        if (room2.lowerLeftCorner.x > room1.upperRightCorner.x)
        {
            if (room1.lowerLeftCorner.y > room2.upperRightCorner.y)
            {
                return false;
            }

            if (room1.upperRightCorner.y < room2.lowerLeftCorner.y)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Removes duplicate links, which cause more hallways than necessary
    /// </summary>
    void RemoveDuplicates()
    {
        foreach (var kV in links)
        {
            foreach (var vector in kV.Value)
            {
                if (links[vector].Contains(kV.Key))
                {
                    links[vector].Remove(kV.Key);
                }
            }
        }
    }

    /// <summary>
    /// If two rooms are aligned, get the direct path instead of a bent path
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    /// <returns></returns>
    Edge GetLinkingEdge(Room room1, Room room2)
    {
        if (room1.lowerLeftCorner.y <= room2.lowerLeftCorner.y && room2.lowerLeftCorner.y <= room1.upperRightCorner.y)
        {
            Vector2Int start = room2.lowerLeftCorner;
            Vector2Int end = new Vector2Int(room1.GetCenter().x, start.y);
            return new Edge(start, end);
        }

        if (room1.lowerLeftCorner.x <= room2.lowerLeftCorner.x && room2.lowerLeftCorner.x <= room1.upperRightCorner.x)
        {
            Vector2Int start = room2.lowerLeftCorner;
            Vector2Int end = new Vector2Int(room2.lowerLeftCorner.x, room1.GetCenter().y);
            return new Edge(start, end);
        }

        return null;
    }

    // Converts our integer 2d array into the tilemap!
    void MapToTilemap()
    {
        ClearGameObjects();
        floor.ClearAllTiles();
        walls.ClearAllTiles();
        AddFloorsAndWalls();
        PlaceDoors();
        Instantiate(downStairs, new Vector3((float)dungeonExits.v0.x, (float)dungeonExits.v0.y), Quaternion.identity, mapGameObjects);
        Instantiate(upStairs, new Vector3((float)dungeonExits.v1.x, (float)dungeonExits.v1.y), Quaternion.identity, mapGameObjects);
    }

    public void ClearGameObjects()
    {
        if (mapGameObjects != null) DestroyImmediate(mapGameObjects.gameObject);
        mapGameObjects = new GameObject("MapGameObjects").GetComponent<Transform>();
        mapGameObjects.transform.SetParent(transform);
    }

    /// <summary>
    /// Translate the 2d map array into actual tiles on the tilemap
    /// </summary>
    public void AddFloorsAndWalls()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (map[x, y] == Tiles.floorTile)
                {
                    // Offset to keep the tilemap at the expected position
                    floor.SetTile(new Vector3Int(x, y, 0), floorTile.GetTile());
                }

                // Fill in anything else with walls
                else
                {
                    map[x, y] = Tiles.freeStandingWallTile;
                    floor.SetTile(new Vector3Int(x, y, 0), freeStandingWallTile.GetTile());

                    if (y > 0 && (IsWallTile(map[x, y - 1])))
                    {
                        map[x, y] = Tiles.wallTile;
                        floor.SetTile(new Vector3Int(x, y, 0), wallTile.GetTile());
                    }
                }

                //Remove filler walls
                bool containsFloor = false;
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (x + i > 0 && x + i < Width - 1 && y + j > 0 && y + j < Height - 1)
                        {
                            if (map[x + i, y + j] == Tiles.floorTile)
                            {
                                containsFloor = true;
                            }
                        }
                    }
                }
                if (!containsFloor)
                {
                    map[x, y] = Tiles.voidTile;
                    floor.SetTile(new Vector3Int(x, y, 0), voidTile.GetTile());
                }
            }
        }
    }

    public void PlaceDoors()
    {
        for (int y = 1; y < Height - 1; y++)
        {
            for (int x = 1; x < Width - 1; x++)
            {
                if (map[x, y] == Tiles.floorTile)
                {
                    Vector2Int[] up = { Vector2Int.up, Vector2Int.left, Vector2Int.right };
                    Vector2Int[] down = { Vector2Int.down, Vector2Int.right, Vector2Int.left };
                    Vector2Int[] left = { Vector2Int.left, Vector2Int.down, Vector2Int.up };
                    Vector2Int[] right = { Vector2Int.right, Vector2Int.up, Vector2Int.down };
                    Vector2Int[][] directions = { up, down, left, right };

                    foreach (var direction in directions)
                    {
                        if (map[x + direction[0].x, y + direction[0].y] == Tiles.floorTile &&
                            map[x + direction[0].x * 2, y + direction[0].y * 2] == Tiles.floorTile &&
                            (map[x + direction[0].x + direction[1].x, y + direction[0].y + direction[1].y] ==
                             Tiles.floorTile ||
                             map[x + direction[0].x + direction[2].x, y + direction[0].y + direction[2].y] ==
                             Tiles.floorTile) &&
                            IsWallTile(map[x + direction[1].x, y + direction[1].y]) &&
                            IsWallTile(map[x + direction[2].x, y + direction[2].y]))
                        {
                            Instantiate(door, new Vector3(x, y), Quaternion.identity, mapGameObjects);
                        }
                    }
                }
            }
        }
    }

    public bool IsWallTile(Tiles tile)
    {
        return tile == Tiles.wallTile || tile == Tiles.freeStandingWallTile;
    }

    class Edge
    {
        public Vector2Int v0;
        public Vector2Int v1;

        public Edge(Vector2Int v0, Vector2Int v1)
        {
            this.v0 = v0;
            this.v1 = v1;
        }
    }
}

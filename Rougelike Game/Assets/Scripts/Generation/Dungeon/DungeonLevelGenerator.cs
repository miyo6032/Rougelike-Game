using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Generates a DungeonLevel floor customized by the DungeonLevel scriptable object
/// </summary>
public class DungeonLevelGenerator : TerrainGenerator
{
    public DungeonLevel DungeonLevel;
    [Header("References")]
    public EnemyStats enemyPrefab;
    public Chest chestPrefab;
    public DungeonUpstairs upStairs;
    public DungeonDownstairs downStairs;
    public DungeonDoor door;
    private Tiles[,] map;
    private Dictionary<Vector2Int, List<Vector2Int>> links;
    private List<Edge> hallways;
    private Dictionary<Vector2Int, Room> rooms;
    private Transform mapGameObjects;
    public VertexPair dungeonExits { get; private set; }
    private bool generateDownStairs;

    public override void Generate()
    {
        generateDownStairs = true;
        InitMap();
        GenerateDungeon();
        MapToTilemap();
    }

    public void GenerateBottom()
    {
        generateDownStairs = false;
        InitMap();
        GenerateDungeon();
        MapToTilemap();
    }

    public void ClearDungeon()
    {
        floor.ClearAllTiles();
        walls.ClearAllTiles();
        ClearGameObjects();
    }

    /// <summary>
    /// Clear the map
    /// </summary>
    private void InitMap()
    {
        ClearDungeon();
        map = new Tiles[DungeonLevel.Width, DungeonLevel.Height];
        for (int y = 0; y < DungeonLevel.Height; y++)
        {
            for (int x = 0; x < DungeonLevel.Width; x++)
            {
                map[x, y] = Tiles.voidTile;
            }
        }
    }

    /// <summary>
    /// Generates the DungeonLevel and writes it to the map 2d array
    /// </summary>
    private void GenerateDungeon()
    {
        DungeonMst dungeonMst = new DungeonMst();
        RoomGenerator roomGenerator = new RoomGenerator();
        rooms = roomGenerator.GenerateRooms(DungeonLevel.InitialRoomDensity, DungeonLevel.Width, DungeonLevel.Height, DungeonLevel.RoomHeightBounds, DungeonLevel.RoomWidthBounds);
        links = dungeonMst.GetDungeonMap(rooms.Keys.ToList(), DungeonLevel.RoomConnectedness);
        LinksIntoHallways();
        dungeonExits = dungeonMst.dungeonExits;

        PlaceExits();
        PlaceChests();

        // Adds hubs
        foreach (var room in rooms.Values)
        {
            WriteRoomToMap(room);
            SpawnEnemies(room);
        }

        foreach (var edge in hallways)
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
        if (DungeonLevel.HallwaySize == 0) return;
        Vector2 step = edge.v1 - edge.v0;
        step.Normalize();
        Vector2Int start = edge.v0;
        int infCounter = 0;
        while (start != edge.v1)
        {
            for (int i = 0; i < DungeonLevel.HallwaySize; i++)
            {
                for (int j = 0; j < DungeonLevel.HallwaySize; j++)
                {
                    int x = Mathf.Clamp(start.x + i - Mathf.FloorToInt(DungeonLevel.HallwaySize / 2f), 0, DungeonLevel.Width - 1);
                    int y = Mathf.Clamp(start.y + j - Mathf.FloorToInt(DungeonLevel.HallwaySize / 2f), 0,
                        DungeonLevel.Height - 1);
                    map[x, y] = Tiles.floorTile;
                }
            }

            start += Vector2Int.FloorToInt(step);
            infCounter++;
            if (infCounter > DungeonLevel.Height * DungeonLevel.Width)
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

    private void SpawnEnemies(Room room)
    {
        int numEnemies = HelperScripts.RandomVec(DungeonLevel.enemiesPerRoom);

        for (int i = 0; i < numEnemies; i++)
        {
            Vector3 enemyPosition;
            do
            {
                enemyPosition = new Vector3(room.lowerLeftCorner.x + Random.Range(1, room.GetWidth()), room.lowerLeftCorner.y + Random.Range(1, room.GetHeight()));
            }
            while (room.SpotTaken(Vector2Int.RoundToInt(enemyPosition)));

            enemyPrefab.enemy = DungeonLevel.Enemies.GetEnemy();
            EnemyStats enemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
            enemy.transform.SetParent(mapGameObjects);
            room.ClaimRoomSpot(Vector2Int.RoundToInt(enemyPosition));
        }
    }

    private void PlaceChests()
    {
        int numChests = HelperScripts.RandomVec(DungeonLevel.chestsPerLevel);
        for (int i = 0; i < numChests; i++)
        {
            Room room;
            do
            {
                room = rooms.Values.ToList()[Random.Range(0, rooms.Count)];
            } while (room.SpotTaken(room.GetCenter()));

            chestPrefab.lootLevel = DungeonLevel.chestLevel;
            Chest chest = Instantiate(chestPrefab, (Vector2)room.GetCenter(), Quaternion.identity);
            chest.transform.SetParent(mapGameObjects);
            room.ClaimRoomSpot(room.GetCenter());
        }
    }

    /// <summary>
    /// Turn linked vertices into hallways depending on the rooms
    /// </summary>
    private void LinksIntoHallways()
    {
        hallways = new List<Edge>();
        RemoveDuplicates();
        foreach (var kV in links)
        {
            foreach (var vector in kV.Value)
            {
                if (RoomsAreAligned(rooms[kV.Key], rooms[vector]))
                {
                    Edge edge = GetLinkingEdge(rooms[kV.Key], rooms[vector]) ??
                                GetLinkingEdge(rooms[vector], rooms[kV.Key]);
                    hallways.Add(edge);
                }
                else
                {
                    Edge xEdge = new Edge(kV.Key, new Vector2Int(vector.x, kV.Key.y));
                    Edge yEdge = new Edge(new Vector2Int(vector.x, kV.Key.y), vector);
                    hallways.Add(yEdge);
                    hallways.Add(xEdge);
                }
            }
        }
    }

    /// <summary>
    /// If the rooms can be connected by a straight hallway, without any turns
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    /// <returns></returns>
    private bool RoomsAreAligned(Room room1, Room room2)
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
    private void RemoveDuplicates()
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
    private Edge GetLinkingEdge(Room room1, Room room2)
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
    private void MapToTilemap()
    {
        AddFloorsAndWalls();
        PlaceDoors();
    }

    /// <summary>
    /// Clear any generated game objects, like doors
    /// </summary>
    private void ClearGameObjects()
    {
        GameObject prevGameObject = GameObject.Find("MapGameObjects");
        if (prevGameObject) Destroy(prevGameObject);
        mapGameObjects = new GameObject("MapGameObjects").GetComponent<Transform>();
        mapGameObjects.transform.SetParent(transform);
    }

    /// <summary>
    /// Translate the 2d map array into actual tiles on the tilemap
    /// </summary>
    public void AddFloorsAndWalls()
    {
        for (int y = 0; y < DungeonLevel.Height; y++)
        {
            for (int x = 0; x < DungeonLevel.Width; x++)
            {
                if (map[x, y] == Tiles.floorTile)
                {
                    // Offset to keep the tilemap at the expected position
                    floor.SetTile(new Vector3Int(x, y, 0), DungeonLevel.floorTile.GetTile());
                }

                // Fill in anything else with walls
                else
                {
                    map[x, y] = Tiles.freeStandingWallTile;
                    walls.SetTile(new Vector3Int(x, y, 0), DungeonLevel.freeStandingWallTile.GetTile());
                    if (y > 0 && (IsWallTile(map[x, y - 1])))
                    {
                        map[x, y] = Tiles.wallTile;
                        walls.SetTile(new Vector3Int(x, y, 0), DungeonLevel.wallTile.GetTile());
                    }
                }

                //Remove filler walls
                bool containsFloor = false;
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (x + i > 0 && x + i < DungeonLevel.Width - 1 && y + j > 0 && y + j < DungeonLevel.Height - 1)
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
                    walls.SetTile(new Vector3Int(x, y, 0), DungeonLevel.voidTile.GetTile());
                }
            }
        }
    }

    /// <summary>
    /// Place doors in an already generated dungeons, searching for a certain pattern in space
    /// </summary>
    public void PlaceDoors()
    {
        for (int y = 1; y < DungeonLevel.Height - 1; y++)
        {
            for (int x = 1; x < DungeonLevel.Width - 1; x++)
            {
                if (map[x, y] == Tiles.floorTile)
                {
                    Vector2Int[] up = {Vector2Int.up, Vector2Int.left, Vector2Int.right};
                    Vector2Int[] down = {Vector2Int.down, Vector2Int.right, Vector2Int.left};
                    Vector2Int[] left = {Vector2Int.left, Vector2Int.down, Vector2Int.up};
                    Vector2Int[] right = {Vector2Int.right, Vector2Int.up, Vector2Int.down};
                    Vector2Int[][] directions = {up, down, left, right};
                    foreach (var direction in directions)
                    {
                        /*
                         * Doors are detected in a certain pattern, where 'o' is open space, 'w' is wall, and 'd' is the door:
                         *
                         *      o
                         *    o o o
                         *    w d w
                         *
                         * The other complexity is the need to rotate the door
                         */
                        Vector2Int crossCenter = new Vector2Int(x + direction[0].x, y + direction[0].y);
                        Vector2Int crossUp = new Vector2Int(x + direction[0].x * 2, y + direction[0].y * 2);
                        Vector2Int crossLeft = new Vector2Int(x + direction[0].x + direction[1].x, y + direction[0].y + direction[1].y);
                        Vector2Int crossRight = new Vector2Int(x + direction[0].x + direction[2].x, y + direction[0].y + direction[2].y);
                        Vector2Int crossBottomLeft = new Vector2Int(x + direction[1].x, y + direction[1].y);
                        Vector2Int crossBottomRight = new Vector2Int(x + direction[2].x, y + direction[2].y);

                        if (map[crossCenter.x, crossCenter.y] == Tiles.floorTile &&
                            map[crossUp.x, crossUp.y] == Tiles.floorTile &&
                            (map[crossLeft.x, crossLeft.y] == Tiles.floorTile || map[crossRight.x, crossRight.y] == Tiles.floorTile) &&
                            IsWallTile(map[crossBottomLeft.x, crossBottomLeft.y]) &&
                            IsWallTile(map[crossBottomRight.x, crossBottomRight.y]))
                        {
                            DungeonDoor instance = Instantiate(door, new Vector3(x, y), Quaternion.identity,
                                mapGameObjects);
                            instance.spriteRenderer.sprite = DungeonLevel.door;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Place the entrance and exit.
    /// </summary>
    public void PlaceExits()
    {
        // Place the upstairs
        DungeonUpstairs upstairs = Instantiate(upStairs, dungeonExits.ToVector2()[0], Quaternion.identity, mapGameObjects);
        upstairs.GetComponent<SpriteRenderer>().sprite = DungeonLevel.upStairs;
        Vector2Int claimedSpot = Vector2Int.FloorToInt(dungeonExits.ToVector2()[0]);
        rooms[claimedSpot].ClaimRoomSpot(claimedSpot);

        // Also claim the foot of the stairs
        rooms[claimedSpot].ClaimRoomSpot(claimedSpot + new Vector2Int(-1, 0));
        if (generateDownStairs)
        {
            DungeonDownstairs downstairs = Instantiate(downStairs, dungeonExits.ToVector2()[1], Quaternion.identity, mapGameObjects);
            downstairs.GetComponent<SpriteRenderer>().sprite = DungeonLevel.downStairs;
            claimedSpot = Vector2Int.FloorToInt(dungeonExits.ToVector2()[1]);
            rooms[claimedSpot].ClaimRoomSpot(claimedSpot);
            rooms[claimedSpot].ClaimRoomSpot(claimedSpot + new Vector2Int(1, 0));
        }
    }

    public bool IsWallTile(Tiles tile)
    {
        return tile == Tiles.wallTile || tile == Tiles.freeStandingWallTile;
    }

    private class Edge
    {
        public readonly Vector2Int v0;
        public readonly Vector2Int v1;

        public Edge(Vector2Int v0, Vector2Int v1)
        {
            this.v0 = v0;
            this.v1 = v1;
        }
    }
}

using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class BoardManager : MonoBehaviour
{

    public static BoardManager instance = null;	//Static instance of GameManager which allows it to be accessed by any other script.

    // Using Serializable allows us to embed a class with sub properties in the inspector.
    [Serializable]
    public class Count
    {
        public int minimum;             //Minimum value for our Count class.
        public int maximum;             //Maximum value for our Count class.

        //Assignment constructor.
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 7;                                         //Number of columns in our game board.
    public int rows = 7;                                            //Number of rows in our game board.
    public GameObject[] floorTiles;                                 //Array of floor prefabs.
    public GameObject[] wallTiles;                                  //Array of wall prefabs.
    public GameObject[] foodTiles;                                  //Array of food prefabs.
    public GameObject[] enemyTiles;									//Array of enemy prefabs.
    public GameObject[] bossTiles;                                 //Array of boss prefabs.
    public Biome[] biomes;
    public GameObject[] pregeneratedStructures;

    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject enemyPrefab;
    public GameObject pickup;

    public LayerMask blockingLayer;

    private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
    private Dictionary<int, GameObject> generatedFloorPositions = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> generatedObjectPositions = new Dictionary<int, GameObject>();
    private Dictionary<int, bool> generatedStructurePositions = new Dictionary<int, bool>(); //Stores positions that the generation algorithm has already passed over so that the program doesn't try to keep generating in certain spots.
    private List<GameObject> lootBags = new List<GameObject>();
    private GameManager manager;
    private Player player;
    private TileDatabase tileDatabase;

    void Start()
    {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        biomes = FindObjectsOfType<Biome>();

        player = GameObject.Find("Player").GetComponent<Player>();
        if (player.inField)
        {
            boardHolder = new GameObject("Board").transform;
        }

        manager = GameManager.instance;
        tileDatabase = gameObject.GetComponent<TileDatabase>();

        for(int i = 0; i < pregeneratedStructures.Length; i++)
        {
            RegisterStructureTiles(pregeneratedStructures[i]);
        }

        if (LoadData.instance.dataLoaded)
        {
            Load();
        }

    }

    public void AddPickup(int x, int y, int[] itemId)
    {//Generates an item in the form of a pickup

        if (x < 0) return;
        GameObject instance =
            Instantiate(pickup, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

        instance.GetComponent<Pickup>().itemId = itemId;

        instance.transform.SetParent(boardHolder);

        lootBags.Add(instance);
    }

    public void GenerateEnemy(int x, int y)//Loads an enemy data set by a position, basing it on the biome that position is in.
    {
            int[] availableEnemyTiles = GetRandomBiomeFromPosition(x, y).biomeEnemies;


            if (availableEnemyTiles.Length == 0)
            {
                return;
            }

            EnemyData enemyData = tileDatabase.GetEnemyByID(availableEnemyTiles[Random.Range(0, availableEnemyTiles.Length)]);
            Wall enemyBody = enemyPrefab.GetComponent<Wall>();
            Enemy enemyStat = enemyPrefab.GetComponent<Enemy>();

            enemyBody.id = enemyData.Id;
            enemyBody.hp = enemyData.CurrentHealth;
            enemyBody.defence = enemyData.Defense;
            enemyBody.experienceValue = enemyData.Experience;
            enemyBody.itemDrop = enemyData.ItemDrop;
            enemyBody.rarity = enemyData.DropRarity;
            enemyBody.name = enemyData.Name;

            enemyStat.enemyDesc = enemyData.EnemyDesc;
            enemyStat.enemyLevel = enemyData.EnemyLevel;
            enemyStat.enemySpeed = enemyData.EnemySpeed;
            enemyStat.playerDamage = enemyData.PlayerDamage;

            enemyPrefab.GetComponent<Animator>().runtimeAnimatorController = enemyData.Animation;

            AddEnemy(x, y, enemyPrefab);
    }

    public void AddEnemy(float x, float y, GameObject enemy)//Adds an enemy to the game.
    {

        if (x < 0) return;
        GameObject instance =
        Instantiate(enemy, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

        instance.transform.SetParent(boardHolder);
    }

    public void ActivateObject(int x, int y)
    {
        int xy = generateKey(x, y);
        if (generatedObjectPositions.ContainsKey(xy))
        {
            if (generatedObjectPositions[xy] != null)
            {
                generatedObjectPositions[xy].SetActive(true);
            }
            else
            {
                generatedObjectPositions.Remove(xy);
            }
        }
    }

    public void DeactivateObject(int x, int y)
    {
        int xy = generateKey(x, y);
        if (generatedObjectPositions.ContainsKey(xy))
        {
            if (generatedObjectPositions[xy] != null)
            {
                generatedObjectPositions[xy].SetActive(false);
            }
            else
            {
                generatedObjectPositions.Remove(xy);
            }
        }
    }

    public bool ObjectPositionTaken(int x, int y)
    {
        int xy = generateKey(x, y);
        if (generatedObjectPositions.ContainsKey(xy))
        {
            return true;
        }
        if (manager.enemyAtPosition(x, y))
        {
            return true;
        }
        return false;
    }

    public void ActivateFloor(int x, int y)
    {
        int xy = generateKey(x, y);

        generatedFloorPositions[xy].SetActive(true);
    }

    public void DeactivateFloor(int x, int y)
    {
        int xy = generateKey(x, y);

        generatedFloorPositions[xy].SetActive(false);
    }

    public void GenerateTerrain(int x, int y)//Generates both tiles and their obstacles(walls) enemies are separate, because they come and go.
    {
        if (x < 0) return;
        Biome biome = GetRandomBiomeFromPosition(x, y);

        if (biome == null)
        {
            Debug.Log("No biome found!!");
            return;
        }

        int[] biomeTiles;//Array to hold all possible floors and walls

        /*biomeTiles = biome.biomeFloors;

        FloorData floorData = tileDatabase.GetFloorByID(biomeTiles[Random.Range(0, biomeTiles.Length)]);//Assigns the object to instanciate the appropriate data.
        floorPrefab.GetComponent<SpriteRenderer>().sprite = floorData.Sprite;
        floorPrefab.GetComponent<Floor>().id = floorData.Id;
        floorPrefab.SetActive(true);

        GameObject instance =
            Instantiate(floorPrefab, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

        instance.transform.SetParent(boardHolder);

        int xy = generateKey(x, y);

        generatedFloorPositions.Add(xy, instance);*/

        // Same thing as the code above except it adds the walls, based on a random factor(because we don't want walls everywhere, just scattered.) Except when the biome uses all of its tiles as walls.

        int xy = generateKey(x, y);

        if (biome.biomeWalls.Length == 0 || generatedObjectPositions.ContainsKey(xy)) return;

        int rand = Random.Range(0, biome.wallSparseness);
        if (rand == 0 || biome.useWallsAsFloors)
        {
            biomeTiles = biome.biomeWalls;

            WallData wallData = tileDatabase.GetWallByID(biomeTiles[Random.Range(0, biomeTiles.Length)]);

            LoadDataIntoWallPrefab(wallData);

            GameObject instance =
                 Instantiate(wallPrefab, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

            instance.transform.SetParent(boardHolder);

            xy = generateKey(x, y);

            generatedObjectPositions.Add(xy, instance);

        }
    }

    private void LoadDataIntoWallPrefab(WallData wallData)
    {
        Wall wall = wallPrefab.GetComponent<Wall>();
        wall.defence = wallData.Defense;
        wall.hp = wallData.CurrentHealth;
        wall.maxHp = wallData.MaxHealth;
        wall.id = wallData.Id;

        if (wallData.IsUnbreakable)//Makes a wall unbreakable, for water, or for city walls, ect.
        {
            wall.gameObject.tag = "Barrier";
        }
        else
        {
            wall.gameObject.tag = "Wall";
        }

        wallPrefab.GetComponent<SpriteRenderer>().sprite = wallData.Sprite;
    }

    public List<Biome> GetBiomesFromPosition(int x, int y)//Returns a list of all the biomes that envelop the position given
    {
        float tilePositionX = x;
        float tilePositionY = y;

        List<Biome> b = new List<Biome>();

        for (int i = 0; i < biomes.Length; i++)
        {
            Biome biome = biomes[i];

            if (biome.GetComponent<Collider2D>().OverlapPoint(new Vector2(tilePositionX, tilePositionY)))
            {
                b.Add(biome);
            }
        }
        return b;
    }

    public Biome GetRandomBiomeFromPosition(int x, int y)
    {

        List<Biome> biomesIn = GetBiomesFromPosition(x, y);

        if (biomesIn.Count == 1)//If there is only one biome, simply return that biome
        {
            return biomesIn[0];
        }
        else if (biomesIn.Count == 0)
        {
            return null;
        }
        else//When there are multiple biomes, some more complicated generation happens to make the edges a bit smoother.
        {

            int[] edgeValues = new int[biomesIn.Count];//Determines how clear cut the biome boundaries are; useful for biomes such as oceans and rivers and paths.

            Biome highestPriorityBiome = null;
            int highestPriority = 0;

            for(int i = 0; i < biomesIn.Count; i++)
            {

                Biome current = biomesIn[i];
                edgeValues[i] = current.edgeHardness;

                if(highestPriority < current.biomePriority)
                {
                    if (current.randomBiomeMergeFactor > 0)
                    {
                        if(Random.Range(0, 100) < Mathf.Clamp(100 - current.randomBiomeMergeFactor, 0, 100))
                        {
                            highestPriorityBiome = current;
                            highestPriority = highestPriorityBiome.biomePriority;
                        }
                    }
                    else
                    {
                        highestPriorityBiome = current;
                        highestPriority = highestPriorityBiome.biomePriority;
                    }
                }

                if (current.edgeHardness == 0) {
                    throw new Exception("Edge hardness is 0!");
                }

            }

            if(highestPriorityBiome != null)//Return the biome of the highest priority - for now, the edge hardness is 0, and I don't really know how to change that. 
            {
                return highestPriorityBiome;
            }

            int edgeHardness = Mathf.Clamp(Mathf.Min(edgeValues), 1, 5);

            List<Biome> closestBiomes = new List<Biome>();

            int distance = 1;

            while (closestBiomes.Count == 0)
            {
                for (int j = 0; j < biomesIn.Count; j++)
                {
                    List<Biome> biomesEast = GetBiomesFromPosition(x + distance, y);//Will expand search distance until it finds a position has the closest biome.
                    List<Biome> biomesWest = GetBiomesFromPosition(x - distance, y);
                    List<Biome> biomesNorth = GetBiomesFromPosition(x, y + distance);
                    List<Biome> biomesSouth = GetBiomesFromPosition(x, y - distance);
                    if (biomesEast.Count == 1)
                    {
                        closestBiomes.Add(biomesEast[0]);
                    }
                    if (biomesWest.Count == 1)
                    {
                        closestBiomes.Add(biomesWest[0]);
                    }
                    if (biomesNorth.Count == 1)
                    {
                        closestBiomes.Add(biomesNorth[0]);
                    }
                    if (biomesSouth.Count == 1)
                    {
                        closestBiomes.Add(biomesSouth[0]);
                    }
                }
                distance += edgeHardness;
                if (distance > 30)
                {
                    Debug.Log("Couldn't find closest biome!!");
                    return null;
                }
            }

            int random = Random.Range(0, closestBiomes.Count - 1);//If there are more than one closest biome, it will include some randomness.
            return closestBiomes[random];

        }
    }

    public void AttemptStructureGeneration(int x, int y)//Will attempt to generate a structure in a certain location based on biomes. Not even the creator fully understands!
    {
        generatedStructurePositions.Add(generateKey(x, y), true);

        List<Biome> biomes = GetBiomesFromPosition(x, y);

        if(biomes.Count == 1)
        {

            Biome biome = biomes[0];

            if (Random.Range(0, biome.generationSparseness - 1) != 0) return;//Makes generation more sparse.

            int random = Random.Range(0, biome.weightSum);

            for(int i = 0; i < biome.generatedStructures.Length; i++)
            {

                GeneratedStructure structure = biome.generatedStructures[i];

                if (random >= biome.weightMap[i] && random < biome.weightMap[i] + structure.generationWeight - 1)//A bizarre comparison that compares whether the random number will attempt to generate a certain structure.
                {

                    structure.transform.position = new Vector2(x, y); // Offsets the prefab. Not sure if this will be a problem in the future.

                    bool collision = CheckStructureCollisions(structure.gameObject);

                    if (!collision) {//Makes sure there are no collsions happening.

                        return;

                    }

                    GameObject instance = Instantiate(biome.generatedStructures[i].gameObject, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                    RegisterStructureTiles(instance);

                    return;

                }

            }

        }

    }

    public bool CheckStructureCollisions(GameObject structure)//Makes sure that the generating structure doens't conflict with other objects before placing
    {
        foreach (Transform child in structure.transform)
        {
            if (child.gameObject != structure)
            {
                if (child.gameObject.tag == "Floor")
                {
                    int xy = generateKey((int)child.position.x, (int)child.position.y);
                    if (generatedFloorPositions.ContainsKey(xy))
                        return false;
                }
                else if (child.gameObject.tag == "Barrier" || child.gameObject.tag == "Wall" || child.gameObject.tag == "NPC")
                {
                    int xy = generateKey((int)child.position.x, (int)child.position.y);
                    if (generatedObjectPositions.ContainsKey(xy) || generatedFloorPositions.ContainsKey(xy))
                        return false;
                }
            }
            if (!CheckStructureCollisions(child.gameObject))
                return false;
        }
        return true;
    }

    public void RegisterStructureTiles(GameObject structure) //Adds the structures tiles to the generated floor and wall positions, so it doesn't get overwritten by random generation.
    {
        foreach (Transform child in structure.transform)
        {
            if (child.gameObject != structure)
            {
                if (child.gameObject.tag == "Floor")
                {
                    int xy = generateKey((int)child.position.x, (int)child.position.y);
                    if (!generatedFloorPositions.ContainsKey(xy))
                        generatedFloorPositions.Add(xy, child.gameObject);
                }
                else if (child.gameObject.tag == "Barrier" || child.gameObject.tag == "Wall" || child.gameObject.tag == "NPC")
                {
                    int xy = generateKey((int)child.position.x, (int)child.position.y);
                    generatedObjectPositions.Add(xy, child.gameObject);
                }
            }

            RegisterStructureTiles(child.gameObject);//To reach children nested in children, use recursive functionality
        }

    }

    public bool CanAttemptToGenerateStructure(int x, int y)//Checks to see if a generated structure is off limits
    {
        return !generatedStructurePositions.ContainsKey(generateKey(x, y));
    }

    public bool FloorPositionTaken(int x, int y)
    {
        int xy = generateKey(x, y);

        if (generatedFloorPositions.ContainsKey(xy))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int generateKey(int x, int y)//Creates a unique integer key from a 2D position in space
    {
        int oom = 10;
        while ((double)y / oom > 1)
        {
            oom *= 10;
        }

        x *= oom;

        int xy = y + x;

        return xy;
    }

    private Vector3 IndexToPosition(int index)//Reverse the function above
    {
        int y = 0;
        int x;

        y = index % 1000;
        index -= index % 1000;

        x = index / 1000;

        return new Vector3(x, y, 0f);

    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/board.dat");
        BoardSave bs = new BoardSave();
        foreach (KeyValuePair<int, GameObject> pair in generatedFloorPositions)
        {
            if (pair.Value != null)
            {
                FloorSaveData floorData = new FloorSaveData(pair.Value.GetComponent<Floor>().id, pair.Value.activeSelf, pair.Key);

                bs.floors.Add(floorData);
            }
        }
        foreach (KeyValuePair<int, GameObject> pair in generatedObjectPositions)
        {

            if (pair.Value != null)
            {
                if (pair.Value.tag == "Wall")
                {
                    WallSaveData wallData = new WallSaveData();
                    Wall wall = pair.Value.GetComponent<Wall>();
                    wallData.CurrentHealth = wall.hp;
                    wallData.Id = wall.id;
                    wallData.Defense = wall.defence;
                    wallData.Position = pair.Key;

                    wallData.IsActive = pair.Value.activeSelf;

                    bs.walls.Add(wallData);

                }


            }

        }
        foreach(KeyValuePair<int, bool> pair in generatedStructurePositions)
        {
            bs.generatedStructurePositions.Add(pair.Key);
        }

        for (int i = 0; i < manager.enemies.Count; i++)
        {
            if (manager.enemies[i] != null)
            {
                EnemySaveData enemyData = new EnemySaveData();
                Enemy enemy = manager.enemies[i];
                Wall wall = enemy.GetComponent<Wall>();

                enemyData.CurrentHealth = wall.hp;
                enemyData.Defense = wall.defence;
                enemyData.Id = wall.id;
                enemyData.Position = generateKey((int)enemy.transform.position.x, (int)enemy.transform.position.y);
                enemyData.EnemyAttack = enemy.playerDamage;
                enemyData.EnemySpeed = enemy.enemySpeed;
                enemyData.TurnCounter = enemy.turnCounter;

                bs.enemies.Add(enemyData);
            }
        }

        for (int i = 0; i < lootBags.Count; i++)
        {
            if (lootBags[i] != null)
            {
                Pickup pickup = lootBags[i].GetComponent<Pickup>();
                LootbagSaveData saveData = new LootbagSaveData(generateKey((int)lootBags[i].transform.position.x, (int)lootBags[i].transform.position.y), pickup.itemId);
                bs.bags.Add(saveData);
            }
        }

        bf.Serialize(file, bs);
        file.Close();

    }

    public void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/board.dat", FileMode.Open);
        BoardSave bs = (BoardSave)bf.Deserialize(file);
        file.Close();

        for (int i = 0; i < bs.floors.Count; i++)
        {
            if (bs.floors[i] != null)
            {
                FloorSaveData floorData = bs.floors[i];
                FloorData databaseData = tileDatabase.GetFloorByID(floorData.Id);
                floorPrefab.GetComponent<SpriteRenderer>().sprite = databaseData.Sprite;
                floorPrefab.GetComponent<Floor>().id = floorData.Id;
                floorPrefab.SetActive(floorData.IsActive);

                GameObject instance =
                    Instantiate(floorPrefab, IndexToPosition(floorData.Position), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);

                generatedFloorPositions.Add(floorData.Position, instance);
            }
        }

        for (int i = 0; i < bs.walls.Count; i++)
        {
            if (bs.walls[i] != null)
            {
                WallSaveData wallData = bs.walls[i];
                Wall wall = wallPrefab.GetComponent<Wall>();
                WallData databaseData = tileDatabase.GetWallByID(wallData.Id);

                wallPrefab.GetComponent<SpriteRenderer>().sprite = databaseData.Sprite;
                wallPrefab.SetActive(wallData.IsActive);

                wall.id = wallData.Id;
                wall.hp = wallData.CurrentHealth;
                wall.defence = wallData.Defense;
                wall.maxHp = databaseData.MaxHealth;

                GameObject instance =
                    Instantiate(wallPrefab, IndexToPosition(wallData.Position), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);

                generatedObjectPositions.Add(wallData.Position, instance);

                if (!generatedFloorPositions.ContainsKey(wallData.Position))//Keeps the generator from messing up on biomes that generate only walls
                {
                    generatedFloorPositions.Add(wallData.Position, instance);
                }

            }
        }

        for(int i = 0; i < bs.generatedStructurePositions.Count; i++)
        {
            generatedStructurePositions.Add(bs.generatedStructurePositions[i], true);
        }

        for (int i = 0; i < bs.enemies.Count; i++)
        {
            if (bs.enemies[i] != null)
            {
                EnemySaveData enemyData = bs.enemies[i];
                Wall wall = enemyPrefab.GetComponent<Wall>();
                Enemy enemy = enemyPrefab.GetComponent<Enemy>();
                EnemyData databaseData = tileDatabase.GetEnemyByID(enemyData.Id);

                wall.defence = enemyData.Defense;
                wall.hp = enemyData.CurrentHealth;
                wall.maxHp = databaseData.MaxHealth;
                wall.experienceValue = databaseData.Experience;
                wall.id = enemyData.Id;
                wall.itemDrop = databaseData.ItemDrop;
                wall.rarity = databaseData.DropRarity;

                enemy.enemyDesc = databaseData.EnemyDesc;
                enemy.enemyLevel = databaseData.EnemyLevel;
                enemy.enemySpeed = enemyData.EnemySpeed;
                enemy.playerDamage = enemyData.EnemyAttack;
                enemy.turnCounter = enemyData.TurnCounter;
                enemy.name = databaseData.Name;

                enemyPrefab.GetComponent<Animator>().runtimeAnimatorController = databaseData.Animation;

                Vector3 position = IndexToPosition(enemyData.Position);

                AddEnemy(position.x, position.y, enemyPrefab);

            }
        }

        for (int i = 0; i < bs.bags.Count; i++)
        {

            if (bs.bags[i] != null)
            {

                Vector3 position = IndexToPosition(bs.bags[i].Position);
                AddPickup((int)position.x, (int)position.y, bs.bags[i].Items);

            }

        }

    }


}

[Serializable]
class BoardSave
{
    public List<FloorSaveData> floors = new List<FloorSaveData>();

    public List<WallSaveData> walls = new List<WallSaveData>();

    public List<int> generatedStructurePositions = new List<int>();

    public List<EnemySaveData> enemies = new List<EnemySaveData>();

    public List<LootbagSaveData> bags = new List<LootbagSaveData>();
}

[Serializable]
public class FloorSaveData
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public int Position { get; set; }

    public FloorSaveData(int id, bool isActive, int position)
    {
        this.Id = id;
        this.IsActive = isActive;
        this.Position = position;
    }
}

[Serializable]
public class WallSaveData
{
    public int Id { get; set; }
    public int CurrentHealth { get; set; }
    public bool IsActive { get; set; }
    public int Defense { get; set; }
    public int Position { get; set; }

    public WallSaveData(int id, int currentHealth, bool isActive, int defense, int position)
    {
        this.Id = id;
        this.CurrentHealth = currentHealth;
        this.IsActive = isActive;
        this.Defense = defense;
        this.Position = position;
    }

    public WallSaveData()
    {
        this.Id = -1;
    }
}

[Serializable]
public class EnemySaveData
{
    public int Id { get; set; }
    public int CurrentHealth { get; set; }
    public int Defense { get; set; }
    public int EnemySpeed { get; set; }
    public int EnemyAttack { get; set; }
    public int Position { get; set; }
    public int TurnCounter { get; set; }

    public EnemySaveData()
    {
        this.Id = -1;
    }
}

[Serializable]
public class LootbagSaveData
{
    public int Position { get; set; }
    public int[] Items { get; set; }

    public LootbagSaveData(int position, int[] items)
    {
        Items = items;
        Position = position;
    }

}



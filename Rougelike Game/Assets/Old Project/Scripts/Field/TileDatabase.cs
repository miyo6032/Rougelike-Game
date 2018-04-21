using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.IO;

public class TileDatabase : MonoBehaviour
{
    private Dictionary<int, FloorData> floorDatabase = new Dictionary<int, FloorData>();
    private JsonData floorData;
    private Dictionary<int, WallData> wallDatabase = new Dictionary<int, WallData>();
    private JsonData wallData;
    private Dictionary<int, EnemyData> enemyDatabase = new Dictionary<int, EnemyData>();
    private JsonData enemyData;

    void Awake()
    {

        floorData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Floors.json"));
        ConstructFloorDatabase();
        wallData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Walls.json"));
        ConstructWallDatabase();
        enemyData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Enemies.json"));
        ConstructEnemyDatabase();
    }

    public FloorData GetFloorByID(int id)
    {
        FloorData fd;
            if (floorDatabase.TryGetValue(id, out fd))
                return fd;
        return null;
    }

    void ConstructFloorDatabase()
    {
        for (int i = 0; i < floorData.Count; i++)
        {
            floorDatabase.Add((int)floorData[i]["id"], 
            new FloorData(
            (int)floorData[i]["id"],
            (string)floorData[i]["icon"],
            (int)floorData[i]["iconIndex"],
            true
        ));
        }
    }

    public WallData GetWallByID(int id)
    {
        WallData wd;
            if (wallDatabase.TryGetValue(id, out wd))
                return wd;
        return null;
    }

    void ConstructWallDatabase()
    {
        for (int i = 0; i < wallData.Count; i++)
        {
            wallDatabase.Add((int)wallData[i]["id"], 
                new WallData(
            (int)wallData[i]["id"],
            (string)wallData[i]["icon"],
            (int)wallData[i]["iconIndex"],
            (int)wallData[i]["maxHealth"],
            (int)wallData[i]["health"],
            (int)wallData[i]["defense"],
            (bool)wallData[i]["isUnbreakable"],
            true
        ));
        }
    }

    public EnemyData GetEnemyByID(int id)
    {
        EnemyData ed;
            if (enemyDatabase.TryGetValue(id, out ed))
                return ed;
        return null;
    }

    void ConstructEnemyDatabase()
    {
        for (int i = 0; i < enemyData.Count; i++)
        {
            int[] itemlist = new int[enemyData[i]["itemDrops"].Count];
            for(int j = 0; j < enemyData[i]["itemDrops"].Count; j++)
            {
                itemlist[j] = (int)enemyData[i]["itemDrops"][j];
            }

            enemyDatabase.Add((int)enemyData[i]["id"],
                new EnemyData(
            (int)enemyData[i]["id"],
            enemyData[i]["name"].ToString(),
            (string)enemyData[i]["animation"],
            (int)enemyData[i]["maxHealth"],
            (int)enemyData[i]["defense"],
            itemlist,
            (int)enemyData[i]["experience"],
            (int)enemyData[i]["rarity"],
            (int)enemyData[i]["playerDamage"],
            (int)enemyData[i]["level"],
            (int)enemyData[i]["speed"],
            (string)enemyData[i]["desc"],
            true
        ));
        }
    }

}

public class FloorData
{
    public int Id { get; set; }
    public Sprite Sprite { get; set; }
    public bool IsActive { get; set; }

    public FloorData(int id, string sprite, int spriteIndex, bool isActive)
    {
        this.Id = id;
        object[] spritesheet = Resources.LoadAll<Sprite>("Sprites/Tiles/" + sprite);
        this.Sprite = (Sprite)spritesheet[spriteIndex];
        this.IsActive = isActive;
    }

    public FloorData()
    {
        this.Id = -1;
    }
}

public class WallData
{
    public int Id { get; set; }
    public Sprite Sprite { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int Defense { get; set; }
    public bool IsUnbreakable { get; set; }
    public bool IsActive { get; set; }

    public WallData(int id, string sprite, int spriteIndex, int maxHealth, int currentHealth, int defense, bool isUnbreakable, bool isActive)
    {
        this.Id = id;
        object[] spritesheet = Resources.LoadAll<Sprite>("Sprites/Tiles/" + sprite);
        this.Sprite = (Sprite)spritesheet[spriteIndex];
        this.MaxHealth = maxHealth;
        this.CurrentHealth = currentHealth;
        this.Defense = defense;
        this.IsUnbreakable = isUnbreakable;
        this.IsActive = isActive;
    }

    public WallData()
    {
        this.Id = -1;
    }
}

public class EnemyData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public RuntimeAnimatorController Animation { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int Defense { get; set; }
    public int[] ItemDrop { get; set; }
    public int Experience { get; set; }
    public int DropRarity { get; set; }
    public int PlayerDamage { get; set; }
    public int EnemyLevel { get; set; }
    public int EnemySpeed { get; set; }
    public string EnemyDesc { get; set; }
    public bool IsActive { get; set; }

    public EnemyData(int id, string name, string animation, int maxHealth, int defense, int[] itemdrop, int experience, int dropRarity, int playerDamage, int enemyLevel, int enemySpeed, string enemyDesc, bool isActive)
    {
        this.Id = id;
        this.Name = name;
        this.Animation = Resources.Load("Animations/AnimatorControllers/" + animation) as RuntimeAnimatorController;
        this.MaxHealth = maxHealth;
        this.CurrentHealth = maxHealth;
        this.Defense = defense;
        this.ItemDrop = itemdrop;
        this.Experience = experience;
        this.DropRarity = dropRarity;
        this.PlayerDamage = playerDamage;
        this.EnemyLevel = enemyLevel;
        this.EnemySpeed = enemySpeed;
        this.EnemyDesc = enemyDesc;
        this.IsActive = isActive;

    }

    public EnemyData()
    {
        this.Id = -1;
    }
}

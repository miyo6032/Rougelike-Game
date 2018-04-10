using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;	//Allows us to use UI.
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
    public int generationSpeed = 3;
    public bool isDead = false;
    public bool inField = false;
    public float hitSpeed = 0.2f;
    public float transitionDelay = 1f;        //Delay time in seconds to a new region(such as a dungeon)
    public float moveScaleFactor = 1;
    public static Player instance;

    public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
    public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
    public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
    public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
    public AudioClip hitSound;              //1 of 2 Audio clips to play when player collects a soda object.
    public AudioClip hitSound2;				//2 of 2 Audio clips to play when player collects a soda object.
    public AudioClip hitSound3;
    public AudioClip hitSound4;
    public AudioClip gameOverSound;             //Audio clip to play when player dies.

    //UI variables
    public Slider experienceSlider;
    public Slider healthSlider;
    public Slider focusSlider;
    public Text healthText;
    public Text damageText;
    public Text focusText;
    public Text armorText;
    public Text defenceText;
    public Text experienceText;
    public Text dialogueText;
    public GameObject canvasGroup;
    public int generationSize = 14;

    public Transform playerIcon;

    public Collider2D[] pregeneratedStructures;

    private Animator animator;                  //Used to store a reference to the Player's animator component.

    public BoardManager board;
    private Inventory inventory;
    private int equippedSkill = -1;
    private bool facingRight = true; //Used in animation to face left or right depending on movement
    private string animationDirection = "side"; //Used in animations to tell which way to face after movement

    private PlayerStat playerStat;

    private List<GameObject> AllObjectInRadius;

    private Vector2 playerDirection = new Vector2(0, 0);

    private UnityEngine.EventSystems.EventSystem _eventSystem;

    private int waitCounter;//Used if a player waits a long time, can regenerate health.

    private Queue<TerrainGeneration> generationQueue = new Queue<TerrainGeneration>();

    //Start overrides the Start function of MovingObject
    protected override void Start()
    {

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        inventory = GameObject.Find("ItemDatabase").GetComponent<Inventory>();
        playerStat = GetComponent<PlayerStat>();

        //Get a component reference to the Player's animator component
        animator = GetComponent<Animator>();

        AllObjectInRadius = new List<GameObject>();

        //Call the Start function of the MovingObject base class.
        base.Start();

        if (LoadData.instance.dataLoaded)
        {
            Load();
        }
        else//Delete soon *******************
        {
            if (inField)
            {
                int generationSize = 14;
                for (int x = -generationSize; x < generationSize + 1; x++)
                {
                    for (int y = -generationSize; y < generationSize + 1; y++)
                    {
                       //ManageTerrainGeneration((int)transform.position.x + x, (int)transform.position.y + y);
                    }
                }
            }
        }
    }

    public int currentEquippedSkill()
    {
        return equippedSkill;
    }

    //When the player tries to "equip" a skill using a button or clicking the skill, this function is called, and determines if you are allowed to use that skill.
    public void equipSkill(int id)
    {
        Item skill = inventory.items[id];
        if (equippedSkill != skill.Id)
        {
            if (skill.ItemLevel <= playerStat.level)
            {
                if (playerStat.focus >= skill.Attack)
                {
                    equippedSkill = skill.Id;
                    Text instance = Instantiate(dialogueText);
                    instance.gameObject.GetComponent<FadeOut>().SetColorAndText("Skill Equipped", Color.white);
                    inventory.EquipHotbar(id);
                }
                else
                {
                    Text instance = Instantiate(dialogueText);
                    instance.gameObject.GetComponent<FadeOut>().SetColorAndText("Not Enough Focus!", Color.white);
                }
            }
            else
            {
                Text instance = Instantiate(dialogueText);
                instance.gameObject.GetComponent<FadeOut>().SetColorAndText("You are not skilled enough!", Color.white);
            }
        }
        else
        {
            equippedSkill = -1;
            inventory.EquipHotbar(-1);
        }
    }

    public void unequipSkill()
    {
        equippedSkill = -1;
        inventory.EquipHotbar(-1);
    }

    private void FixedUpdate()
    {
        /*
for (int i = 0; i < generationSpeed; i++)
if (generationQueue.Count > 0)
{
TerrainGeneration posToGenerate = generationQueue.Dequeue();
int xpos = posToGenerate.x;
int ypos = posToGenerate.y;

if (posToGenerate.genType == 0)
{

    if (board.ObjectPositionTaken(xpos, ypos))//Deactivates walls and floors from behind
        board.DeactivateObject(xpos, ypos);

    if (board.FloorPositionTaken(xpos, ypos))
        board.DeactivateFloor(xpos, ypos);
}
else if(posToGenerate.genType == 1)
{
    ManageTerrainGeneration(xpos, ypos);//Generates floors and walls, or activate existing ones
}
else
{
    AddEnemy(xpos, ypos);
}

    }
            else
            {
                break;
            }
            */
        animator.SetFloat("horizontal", Mathf.Abs(playerDirection.x));
        animator.SetFloat("vertical", playerDirection.y);

        //If it's not the player's turn, exit the function.
        if (moving || isDead) return;

        playerDirection = new Vector2(0, 0);

        //Input to ready a skill
        if (Input.GetButtonDown("Skill1"))
        {
            equipSkill(24);
        }
        if (Input.GetButtonDown("Skill2"))
        {
            equipSkill(25);
        }
        if (Input.GetButtonDown("Skill3"))
        {
            equipSkill(26);
        }
        if (Input.GetButtonDown("Skill4"))
        {
            equipSkill(27);
        }
        if (Input.GetButtonDown("Skill5"))
        {
            equipSkill(28);
        }

        int horizontal = 0;     //Used to store the horizontal move direction.
        int vertical = 0;       //Used to store the vertical move direction.

        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (horizontal != 0)
        {

            vertical = 0;//Make sure we don't move diagonally, however cool that would be

            animationDirection = "side";//Rest of this is for animation

            if (horizontal > 0 && !facingRight)
            {
                flip();
            }
            else if (horizontal < 0 && facingRight)
            {
                flip();
            }

        }
        else if (vertical < 0)
        {
            animationDirection = "front";
        }
        else if (vertical > 0)
        {
            animationDirection = "back";
        }

        //Check if we have a non-zero value for horizontal or vertical
        if (horizontal != 0 || vertical != 0)
        {
            //A key piece of code that moves the entire board.
            if (!inventory.inventoryOpen && !inventory.IsMapDrawn() && !inventory.IsQuestDrawn() && !inventory.DialogueDrawn())
            {
                waitCounter = 0;
                moving = true;//Used to keep the player from input while gliding to a space

                Vector2 vec = new Vector2(transform.position.x + horizontal, transform.position.y + vertical);
                GameManager.instance.claimedSpots.Add(vec);//Make sure an enemy doesn't move into the spot you are going to move into!
                GameManager.instance.SetupEnemies();//All enemies will claim their movement spots
                AttemptMove<Wall>(horizontal, vertical);//The player moves
                GameManager.instance.MoveEnemies();//The enemies move
                inventory.EquipHotbar(-1);//Reset skills at the ready (because you "used up" the skill)
            }
        }

    }

    void flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void OnMouseDown()//If the player clicks on the player sprite, wait/skip a turn.
    {

        UpdateFocus(2);

        if (playerStat.health < playerStat.maxHealth)
        {
            playerStat.health += 1;
            if (waitCounter > 3)
            {
                playerStat.health += waitCounter;
            }
        }

        UpdateHealth();

        SkipTurn();

    }

    public void SkipTurn()//Same thing at when you move, except this time the player isn't moving, so claim the spot you are already in.
    {
        if (!inventory.inventoryOpen && !inventory.IsMapDrawn() && !inventory.IsQuestDrawn())
        {
            waitCounter++;
            moving = true;
            Vector2 vec = new Vector2(transform.position.x, transform.position.y);
            GameManager.instance.claimedSpots.Add(vec);
            GameManager.instance.SetupEnemies();
            GameManager.instance.MoveEnemies();
            moving = false;
        }
    }

    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override void AttemptMove<T>(int xDir, int yDir)
    {

        //Hit allows us to reference the result of the Linecast done in Move.
        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {

            GenerateTerrain(xDir, yDir);

            playerDirection = new Vector2(xDir, yDir);

            playerIcon.position = playerIcon.position + new Vector3(xDir, yDir, 0) * moveScaleFactor;

            //SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);

            if (equippedSkill == -2 || equippedSkill == -3)//Skills that require an enemy be the target
            {
                equippedSkill = -1;
                Text instance = Instantiate(dialogueText);
                instance.gameObject.GetComponent<FadeOut>().SetColorAndText("No enemy is attacked.", Color.white);
            }
            else if (equippedSkill == -5)//Healing
            {
                equippedSkill = -1;
                Text instance = Instantiate(dialogueText);
                instance.gameObject.GetComponent<FadeOut>().SetColorAndText("You patch your wounds.", Color.white);
                int heal = playerStat.maxHealth / 4;
                playerStat.health = Mathf.Clamp((playerStat.health + heal), 0, playerStat.maxHealth);
                playerStat.focus -= 5;

                DamageCounter damageCounter = (Instantiate(canvasGroup, gameObject.transform.position, Quaternion.identity) as GameObject).GetComponent<DamageCounter>();
                damageCounter.setDamage(heal);
                damageCounter.GetComponentInChildren<Image>().color = Color.green;
            }

            if (playerStat.health < playerStat.maxHealth)//Naturally heal 1 health during a move
            {
                playerStat.health += 1;
                if (waitCounter > 3)
                {
                    playerStat.health += waitCounter;
                }
            }
            UpdateFocus(1);

        }
        else
        {
            T hitComponent = hit.transform.GetComponent<T>();
            OnCantMove(hitComponent);
        }

        UpdateHealth();//Update health GUI

        CheckIfGameOver();//The player could have potentially died in the last turn.

        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        GameManager.instance.playersTurn = false;
    }

    public void GenerateTerrain(int xDir, int yDir)
    {//Will handle all procedures that involve the board manager

        GameObject[] enemyPosition2 = GameObject.FindGameObjectsWithTag("Enemy") as GameObject[];

        if (xDir != 0)
        {
            int y;
            int x = generationSize * xDir;
            int xpos = (int)transform.position.x;
            int ypos = (int)transform.position.y;

            for (y = -generationSize; y <= generationSize; y++)
            {

                TerrainGeneration toQueue = new TerrainGeneration(xpos + x, ypos + y, 1);
                generationQueue.Enqueue(toQueue);
                toQueue = new TerrainGeneration(xpos - x, ypos + y, 0);
                generationQueue.Enqueue(toQueue);

            }

            y = UnityEngine.Random.Range(-generationSize + 1, generationSize);

            if (enemyPosition2.Length < 10)//If there are less than 10 enemies, then generate some more - keeps enemy spawning from going out of control
            {
                TerrainGeneration toQueue = new TerrainGeneration(x + (int)transform.position.x, y + (int)transform.position.y, 2);
                generationQueue.Enqueue(toQueue);
            }

        }
        else//Same thing except for the y direction
        {
            int x;
            int y = generationSize * yDir;
            int xpos = (int)transform.position.x;
            int ypos = (int)transform.position.y;

            for (x = -generationSize; x <= generationSize; x++)
            {
                TerrainGeneration toQueue = new TerrainGeneration(xpos + x, ypos + y, 1);
                generationQueue.Enqueue(toQueue);
                toQueue = new TerrainGeneration(xpos + x, ypos - y, 0);
                generationQueue.Enqueue(toQueue);

            }

            x = UnityEngine.Random.Range(-generationSize + 1, generationSize);

            if (enemyPosition2.Length < 10)//If there are less than 10 enemies, then generate some more - keeps enemy spawning from going out of control
            {
                TerrainGeneration toQueue = new TerrainGeneration(x + (int)transform.position.x, y + (int)transform.position.y, 2);
                generationQueue.Enqueue(toQueue);
            }

        }
    }

    bool GeneratedStructureExists(int x, int y)//Returns true if a position is enclosed by a pregeneratedstructure
    {
        for (int i = 0; i < pregeneratedStructures.Length; i++)
        {
            if (pregeneratedStructures[i].bounds.Contains(new Vector3(x, y, 0)))
            {
                return true;
            }
        }
        return false;
    }

    void AddEnemy(int x, int y)
    {
        if (!board.ObjectPositionTaken(x, y) && !GeneratedStructureExists((int)transform.position.x, (int)transform.position.y) && !GeneratedStructureExists(x, y))
        {
            board.GenerateEnemy(x, y);
        }
    }

    void ManageTerrainGeneration(int x, int y)//Manages which terrain get activated... basically either adds floors and walls or activate them. 
    {
        if (!board.FloorPositionTaken(x, y))
        {
            board.GenerateTerrain(x, y);
        }
        else
        {
            board.ActivateFloor(x, y);
        }

        if (board.ObjectPositionTaken(x, y))
        {
            board.ActivateObject(x, y);
        }
    }

    //When the player collides with something, anything
    protected override void OnCantMove<T>(T component)
    {
        Invoke("MovingFalse", hitSpeed);//A delay keeps the player from hitting whatever is in front of them too rapidly, aka. 1 hit per frame.

        //Set hitWall to equal the component passed in as a parameter.
        Wall hitWall = component as Wall;

        //If the object collided with is a Barrier, just do nothing, basically. The inField is not implement right now could be used for dungeons ***********************************
        if (!inField && hitWall.gameObject.tag == "Wall" || hitWall.gameObject.tag == "Barrier")
        {
            return;
        }

        //If you collide into an NPC, activate whatever that NPC is supposed to do.
        if (hitWall.gameObject.tag == "NPC")
        {
            hitWall.gameObject.GetComponent<NPC>().OnMouseDown();
            return;
        }

        if (animationDirection == "side") animator.SetTrigger("attackSide");
        if (animationDirection == "front") animator.SetTrigger("attackFront");
        if (animationDirection == "back") animator.SetTrigger("attackBack");

        //Combat section of CantMove()

        int baseDamage = UnityEngine.Random.Range(playerStat.attack, playerStat.maxAttack + 1);

        //Call the DamageWall function of the Wall we are hitting.
        if (equippedSkill == -2)
        {//Critical Hit Skill
            baseDamage = baseDamage * 2;
            UpdateFocus(-10);
            equippedSkill = -1;
            SoundManager.instance.PlaySingle(hitSound4);
        }
        else if (equippedSkill == -3)
        {//Circle Slash Skill

            float range = 1;
            //baseDamage = baseDamage;
            UpdateFocus(-10);
            equippedSkill = -1;
            GameObject[] enemyPosition1 = GameObject.FindGameObjectsWithTag("Enemy") as GameObject[];
            GameObject[] enemyPosition2 = GameObject.FindGameObjectsWithTag("Wall") as GameObject[];

            foreach (GameObject enemy in enemyPosition2)
            {
                if (enemy.GetComponent<Wall>() != null)
                {
                    if (Mathf.Abs(enemy.transform.position.x - transform.position.x) <= range && Mathf.Abs(enemy.transform.position.y - transform.position.y) <= range)
                    {
                        enemy.GetComponent<Wall>().DamageWall(baseDamage);
                    }
                }
            }
            foreach (GameObject enemy in enemyPosition1)
            {
                if (enemy.GetComponent<Wall>() != null)
                {
                    if (Mathf.Abs(enemy.transform.position.x - transform.position.x) <= range && Mathf.Abs(enemy.transform.position.y - transform.position.y) <= range)
                    {
                        enemy.GetComponent<Wall>().DamageWall(baseDamage);
                    }
                }
            }

            SoundManager.instance.PlaySingle(hitSound4);
        }
        else if (equippedSkill == -5)//Heal Skill
        {
            equippedSkill = -1;
            Text instance = Instantiate(dialogueText);
            instance.gameObject.GetComponent<FadeOut>().SetColorAndText("You can't heal and attack at the same time!", Color.white);
            SoundManager.instance.RandomizeSfx(hitSound, hitSound2, hitSound3);
        }
        else
        {
            SoundManager.instance.RandomizeSfx(hitSound, hitSound2, hitSound3);
        }

        hitWall.DamageWall(baseDamage);

    }

    public void MovingFalse()
    {
        moving = false;
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Exit")
        {
            //Disable the player object since level is over.
            enabled = false;
        }
    }

    //LoseFood is called when an enemy attacks the player.
    //It takes a parameter loss which specifies how many points to lose.
    public void LoseFood(int loss)
    {

        waitCounter = 0;

        SoundManager.instance.RandomizeSfx(hitSound, hitSound2, hitSound3);

        //Subtract lost food points from the players total.
        loss = loss - playerStat.defence;
        if (loss < 0)
        {
            loss = 0;
        }
        playerStat.health -= loss;
        UpdateFocus(-1);

        //Add a damage counter to show damage taken
        DamageCounter damageCounter = (Instantiate(canvasGroup, gameObject.transform.position, Quaternion.identity) as GameObject).GetComponent<DamageCounter>();
        damageCounter.setDamage(Mathf.Clamp(loss, 0, loss));
        damageCounter.GetComponentInChildren<Image>().color = Color.red;
        if ((double)loss / (double)playerStat.maxHealth > 0.5)
        {
            damageCounter.GetComponentInChildren<Image>().color = Color.HSVToRGB(0.08f, 1, 1);
            SoundManager.instance.PlaySingle(hitSound4);
        }
        else
        {
            SoundManager.instance.RandomizeSfx(hitSound, hitSound2, hitSound3);
        }

        //Updates health
        UpdateHealth();

        //Check to see if game has ended.
        CheckIfGameOver();
    }

    public void AddExperience(int experience)
    {
        playerStat.experience += experience;
        if (playerStat.experience > playerStat.maxExperience)
        {
            playerStat.experience = -playerStat.maxExperience;
            playerStat.level++;
            experienceText.text = "Level " + playerStat.level;
            playerStat.maxExperience = (int)(playerStat.maxExperience * 1.4f);
            playerStat.experience = 0;
            playerStat.maxFocus += 2;
            playerStat.maxHealth = (int)(playerStat.maxHealth * 1.2f);
            armorText.text = "Max Health: " + playerStat.maxHealth;
            Text instance = Instantiate(dialogueText);
            instance.gameObject.GetComponent<FadeOut>().SetColorAndText("You Leveled Up!", Color.green);
        }
        experienceSlider.value = (float)(playerStat.experience) / (float)(playerStat.maxExperience) * 100;
    }

    public void UpdateExperience()
    {
        experienceText.text = "Level " + playerStat.level;
        experienceSlider.value = (float)(playerStat.experience) / (float)(playerStat.maxExperience) * 100;
    }

    private void CheckIfGameOver()
    {
        if (playerStat.health <= 0)
        {
            //Bring up the canvas to either reload or quit game
            inventory.ActivateDeathCanvas();
            isDead = true;
        }
    }

    private void UpdateFocus(int focus)
    {
        playerStat.UpdateFocus(focus);
    }

    private void UpdateHealth()//Updates Health GUI
    {
        playerStat.UpdateHealth();
    }

    private List<GameObject> FindAllInRadius(Vector3 center, float radius)
    {
        Vector2 vec = new Vector2(center.x, center.y);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(vec, radius);
        List<GameObject> objects = new List<GameObject>();

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Debug.Log(hitColliders.Length);
            objects.Add(hitColliders[i].gameObject);
        }

        return objects;
    }

    public List<GameObject> GetObjectsInRadius()
    {
        return AllObjectInRadius;
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/player.dat");

        PlayerSave pss = new PlayerSave();
        float[] iconposition = { playerIcon.position.x, playerIcon.position.y, playerIcon.position.z };
        float[] playerPosition = { transform.position.x, transform.position.y, transform.position.z };
        pss.playerIconPosition = iconposition;
        pss.equippedSkill = equippedSkill;
        pss.playerPosition = playerPosition;

        bf.Serialize(file, pss);
        file.Close();

    }

    public void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
        PlayerSave pss = (PlayerSave)bf.Deserialize(file);
        file.Close();
        playerIcon.position = new Vector3(pss.playerIconPosition[0], pss.playerIconPosition[1], pss.playerIconPosition[2]);
        equippedSkill = pss.equippedSkill;
        transform.position = new Vector3(pss.playerPosition[0], pss.playerPosition[1], pss.playerPosition[2]);
    }


}

struct TerrainGeneration
{
    public int x;
    public int y;
    public int genType;

    public TerrainGeneration(int X, int Y, int GenType)
    {
        x = X;
        y = Y;
        genType = GenType;
    }

}

[Serializable]
class PlayerSave
{
    public float[] playerIconPosition;

    public int equippedSkill = -1;

    public float[] playerPosition;
}



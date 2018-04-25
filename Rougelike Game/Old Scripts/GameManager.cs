using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
	public int playerFoodPoints = 100;						//Starting value for Player food points.
	public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
	[HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.
    public List<Vector2> claimedSpots;
	
	private Text levelText;									//Text to display current level number.
	private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
	//private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.									//Current level number, expressed in game as "Day 1".
	public List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
	private Transform player;

    public bool enemyAtPosition(int x, int y)
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null)
            {
                if (enemies[i].transform.position.x == x && enemies[i].transform.position.y == y)
                {
                    return true;
                }
            }
        }
        if(player.position.x == x && player.position.y == y)
        {
            return true;
        }
        return false;
    }

	//Awake is always called before any Start functions
	void Awake()
	{

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Assign enemies to a new List of Enemy objects.
        enemies = new List<Enemy>();

		player = GameObject.FindGameObjectWithTag ("Player").transform;
		
		InitGame();
	}
	
	//Initializes the game for each level.
	void InitGame()
	{
		
		//Change this. Maybe don't need it for the city? 
		levelImage = GameObject.Find("LevelImage");
		
		//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		
		//Set the text of levelText to the string "Day" and append the current level number.
		levelText.text = "Will you survive?";
		
		//Set levelImage to active blocking player's view of the game board during setup.
		levelImage.SetActive(true);
		
		//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
		Invoke("HideLevelImage", levelStartDelay);
		
		//Clear any Enemy objects in our List to prepare for next level.
		enemies.Clear();
		
	}
	
	//Hides black image used between levels
	void HideLevelImage()
	{
		//Disable the levelImage gameObject.
		levelImage.SetActive(false);
	}

    //Call this to add the passed in Enemy to the List of Enemy objects.
    public void AddEnemyToList(Enemy script)
    {
        //Add Enemy to List enemies.
        enemies.Add(script);
    }

    public void ReformPlayer()
    {

        player.position = Vector3.zero;

        player.gameObject.SetActive(true);

        enabled = true;

        levelImage.SetActive(false);

    }

    public bool SpotClaimed(Vector2 spot)
    {
        for(int i = 0; i < claimedSpots.Count; i++)
        {
            if(claimedSpots[i] == spot)
            {
                return true;
            }
        }
        return false;
    }

    public void MoveEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {

            if (enemies[i] != null)
            {

                if ((enemies[i].transform.position - player.position).sqrMagnitude < 25)
                {
                    enemies[i].MoveEnemy();
                }
            }
        }

        //Once Enemies are done moving, set playersTurn to true so player can move.
		playersTurn = true;

        claimedSpots.Clear();
    }
	
	//Coroutine to move enemies in sequence.
	public void SetupEnemies()
	{

		//Loop through List of Enemy objects.
		for (int i = 0; i < enemies.Count; i++)
			{

			if(enemies[i] != null){

				if((enemies[i].transform.position - player.position).sqrMagnitude < 15){

                    int speed = enemies[i].enemySpeed;

                    if (speed > 0)
                    {

                        for (int i2 = 0; i2 < speed; i2++)
                        {

                            //Call the MoveEnemy function of Enemy at index i in the enemies List.
                            enemies[i].SetupNextMove();

                        }

                    }

                    if (speed < 0) {

                        if (enemies[i].turnCounter == 0) {

                            //Call the MoveEnemy function of Enemy at index i in the enemies List.
                            enemies[i].SetupNextMove();
                            enemies[i].turnCounter = speed;
                        }
                        else
                        {
                            enemies[i].turnCounter ++;
                        }

                    }

				}
                else if((enemies[i].transform.position - player.position).sqrMagnitude > 200)
                {
                    enemies[i].gameObject.GetComponent<Wall>().Delete();
                    enemies.Remove(enemies[i]);
                }

			}

		}
	}
}

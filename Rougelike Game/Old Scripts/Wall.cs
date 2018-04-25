using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Wall : MonoBehaviour {
	
	public int hp = 4;
	public int maxHp;
	public int defence;
	public int experienceValue;
	public int[] itemDrop;
	public int rarity;
    public int id;
	public GameObject canvasGroup;
	public GameObject canvasGroupToInstanciate;

	//private EnemyHealth enemyHealth;
    //public ParticleSystem system;

    // Use this for initialization
    void Awake () {
		maxHp = hp;
		/*if (this.tag != "Wall" && this.tag != "NPC" && this.tag != "Barrier") {
			enemyHealth = (Instantiate (canvasGroupToInstanciate, gameObject.transform.position, Quaternion.identity) as GameObject).GetComponent<EnemyHealth> ();
			UpdateHealthBar();
		}*/
	}

	public void Update(){
		/*if (this.tag != "Wall" && this.tag != "NPC" && this.tag != "Barrier") {
			UpdateHealthBar();
		}*/
	}

	void UpdateHealthBar(){
		/*if (this.tag != "Wall" && this.tag != "NPC" && this.tag != "Barrier") {
			enemyHealth.setPosition (gameObject.transform.position + new Vector3(0, 0.5f, 0));
			enemyHealth.updateHealth (hp * 100 / maxHp);
		}*/
	}

    public void Delete()
    {
        /*if (gameObject.tag != "Wall")
        {
            Destroy(enemyHealth.gameObject);
        }*/
        Destroy(gameObject);
    }
	
	public void DamageWall(int loss){
		loss = loss - defence;
		if (loss < 0) {
			loss = 0;
		}
		hp -= loss;

		//Add a damage counter to show damage taken
		DamageCounter damageCounter = (Instantiate (canvasGroup, gameObject.transform.position, Quaternion.identity) as GameObject).GetComponent<DamageCounter>();
		damageCounter.setPosition (gameObject.transform.position);
		damageCounter.setDamage(Mathf.Clamp(loss, 0, loss));
        damageCounter.GetComponentInChildren<Image>().color = Color.red;
        if ((double)loss / (double)maxHp > 0.5)
        {
            damageCounter.GetComponentInChildren<Image>().color = Color.HSVToRGB(0.08f, 1, 1);
        }

        if (hp <= 0) {
			if (this.tag != "Wall") {
                //system.transform.position = this.transform.position;
                //system.Emit(10);
				Player.instance.AddExperience(experienceValue);

                int[] itemsDropped = new int[4];

                for(int i = 0; i < 4; i++)
                {
                    if (Random.Range(0, rarity) == 0 && itemDrop.Length > i)
                    {
                        itemsDropped[i] = itemDrop[i];
                    }
                    else
                    {
                        itemsDropped[i] = -1;
                    }
                }
                if (!(itemsDropped[0] == -1 && itemsDropped[1] == -1 && itemsDropped[2] == -1 && itemsDropped[3] == -1))
                {

                    //Handles the case when an enemy drops something on an exisiting bag
                    GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
                    bool bagExists = false;
                    for (int i = 0; i < pickups.Length; i++)
                    {
                        if (pickups[i].transform.position == transform.position)
                        {
                            bagExists = true;
                            int pickupsIndex = 0;
                            Pickup matchingPickup = pickups[i].GetComponent<Pickup>();
                            for (int j = 0; j < 4; j++)
                            {
                                if(matchingPickup.itemId[i] == -1)
                                {
                                    matchingPickup.itemId[i] = itemsDropped[pickupsIndex];
                                    pickupsIndex++;
                                }
                            }
                        }
                    }

                    if (!bagExists)
                    {
                        BoardManager.instance.AddPickup((int)transform.position.x, (int)transform.position.y, itemsDropped);
                    }
                }
			}
            Delete();
			UpdateHealthBar();
		}
	}
}

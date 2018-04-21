using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour {

    public int duration = 0;
    public string type;
    public int potency;
    Player player = null;
    Enemy enemy = null;
    bool effectApplied = false;
    int tmp = 0;

	// Use this for initialization
	void Start () {
        if (gameObject.transform.parent.gameObject.tag == "Player")
        {
            player = gameObject.GetComponentInParent<Player>();
        }
        else
        {
            enemy = gameObject.GetComponentInParent<Enemy>();
        }

        ParticleSystem.MainModule main = gameObject.GetComponent<ParticleSystem>().main;

        if(type == "poison")
        {
            main.startColor = Color.green;
        }
        else if(type == "fire")
        {
            main.startColor = Color.red;
        }
        else
        {
            main.startColor = Color.blue;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(duration <= 0)
        {
            Destroy(gameObject);
        }

		if(player != null)
        {
            if (GameManager.instance.playersTurn && !effectApplied)
            {
                duration--;
                if(type == "poison" || type == "fire")
                {
                    player.LoseFood(potency);
                }
                else if(type == "freeze")
                {
                    GameManager.instance.playersTurn = false;
                }
                effectApplied = true;

            }
            else if(!GameManager.instance.playersTurn && effectApplied)
            {
                effectApplied = false;
            }
        }
        else
        {
            if(!GameManager.instance.playersTurn && !effectApplied)
            {
                duration--;
                if (type == "poison" || type == "fire")
                {
                    enemy.gameObject.GetComponent<Wall>().DamageWall(potency);
                }
                else if (type == "freeze")
                {
                    tmp = enemy.enemySpeed;
                    enemy.enemySpeed = 0;
                }
                effectApplied = true;
            }
            else if(GameManager.instance.playersTurn && effectApplied)
            {
                effectApplied = false;
                if (type == "freeze")
                {
                    enemy.enemySpeed = tmp;
                }
            }
        }
	}
}

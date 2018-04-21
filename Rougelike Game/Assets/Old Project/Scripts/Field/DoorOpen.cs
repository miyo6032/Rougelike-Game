using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorOpen : MonoBehaviour {

    private SpriteRenderer sp;
    private GameObject player;

    public Sprite change;

	// Use this for initialization
	void Start () {
        sp = gameObject.GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if(player.transform.position.x == transform.position.x && player.transform.position.y == transform.position.y)
        {
            sp.sprite = change;
        }
	}
}

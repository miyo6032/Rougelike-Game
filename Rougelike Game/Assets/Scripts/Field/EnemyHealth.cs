using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour {

	private Slider healthSlider;

	// Use this for initialization
	void Awake () {
		healthSlider = GetComponentInChildren <Slider>();
	}

	public void updateHealth(int healthpercentage){
		healthSlider.value = healthpercentage;
		if (healthpercentage <= 0) {
			Destroy(gameObject);
		}
	}

	public void setPosition(Vector3 position){
		healthSlider.transform.position = position;
	}

}

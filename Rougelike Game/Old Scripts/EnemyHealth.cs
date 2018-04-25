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
	}

}

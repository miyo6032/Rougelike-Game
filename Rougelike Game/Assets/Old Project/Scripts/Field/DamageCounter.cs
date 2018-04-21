using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DamageCounter : MonoBehaviour {

	private Image damageImage;
	private Text damageCounterText;
	public int delayTime = 1;

	public void setDamage(int damage){
		damageCounterText.text = "-" + damage;
	}

	public void setPosition(Vector3 position){
		damageImage.rectTransform.position = position;
		damageCounterText.rectTransform.position = position;
	}

	// Use this for initialization
	void Awake () {
		damageImage = GetComponentInChildren<Image>();
		damageCounterText = GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		damageImage.color = Color.Lerp (damageImage.color, Color.clear, delayTime * Time.deltaTime);
		damageCounterText.color = Color.Lerp (damageCounterText.color, Color.clear, Time.deltaTime * delayTime);
		if (damageImage.color.a < 0.5f) {
			Destroy(gameObject);
		}
	}
}

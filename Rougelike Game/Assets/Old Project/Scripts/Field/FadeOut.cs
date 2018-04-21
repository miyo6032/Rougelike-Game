using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour {

    public void SetColorAndText(string t, Color color)
    {
        gameObject.GetComponent<Text>().color = color;
        gameObject.GetComponent<Text>().text = t;
    }

	// Use this for initialization
	void Start () {
        GameObject gameCanvas = GameObject.Find("GameCanvas");

        gameObject.transform.SetParent(gameCanvas.transform);
    }
	
	// Update is called once per frame
	void Update () {

        gameObject.GetComponent<Text>().color = Color.Lerp(gameObject.GetComponent<Text>().color, Color.clear, Time.deltaTime);

        if (gameObject.GetComponent<Text>().color.a < 0.5f)
        {
            GameObject.Destroy(gameObject);
        }
	}
}

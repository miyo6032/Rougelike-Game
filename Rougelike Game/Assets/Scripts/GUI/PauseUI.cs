using UnityEngine;

public class PauseUI : MonoBehaviour {

    public GameObject gameUI;
	
	void Start () {

	}
	
    public void Resume()
    {
        Time.timeScale = 1;
        gameUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Quit()
    {

    }

    public void Options()
    {

    }
	
}

using UnityEngine;

/// <summary>
/// Pausing the game, options, and quitting
/// </summary>
public class PauseUI : MonoBehaviour {

    public GameObject gameUI;
	
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

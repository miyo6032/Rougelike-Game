using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {
    
    public Slider healthSlider; //The player's in-game health bar
    public GameObject pauseUI;
    public GameObject invUI;
    public GameObject menuPopup;

	void Start () {
	}

    //Update the player's in game health bar
    public void UpdateHealth(float healthPercent)
    {
        healthSlider.value = healthPercent; 
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OpenInventory()
    {
        invUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void TogglePopupMenu()
    {
        menuPopup.SetActive(!menuPopup.activeSelf);
    }

}

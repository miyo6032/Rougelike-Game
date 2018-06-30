using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the ui for active game (like health and hot bars)
/// </summary>
public class InGameUI : MonoBehaviour
{
    public Slider healthSlider; // The player's in-game health bar
    public Slider focusSlider;
    public GameObject pauseUI;
    public GameObject invUI;
    public GameObject menuPopup;
    public GameObject lootPopupPanel;

    /// <summary>
    /// Update the player's in game health bar
    /// </summary>
    /// <param name="healthPercent"></param>
    public void UpdateHealth(float healthPercent)
    {
        healthSlider.value = healthPercent;
    }

    public void UpdateFocus(float focusPercent)
    {
        focusSlider.value = focusPercent;
    }

    /// <summary>
    /// Pauses by setting the timescale to 0
    /// </summary>
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

    public void ToggleLootPanel()
    {
        lootPopupPanel.SetActive(!lootPopupPanel.activeSelf);
    }

    public void OpenLootPanel()
    {
        lootPopupPanel.SetActive(true);
    }

    public void CloseLootPanel()
    {
        lootPopupPanel.SetActive(false);
    }
}

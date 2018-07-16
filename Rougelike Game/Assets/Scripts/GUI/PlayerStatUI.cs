using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Displays the statistics of the player
/// </summary>
public class PlayerStatUI : MonoBehaviour
{
    public static PlayerStatUI instance;
    public Text level;
    public Text experience;
    public Text maxHealth;
    public Text maxFocus;
    public Text defense;
    public Text attack;
    public Slider experienceSlider;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Duplicate " + this.GetType().Name);
            Destroy(gameObject);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the UI by passing int all of the player's statistics
    /// </summary>
    public void UpdateStatUI(int level, int experience, int maxExperience, int health, int maxHealth, int focus, int maxFocus, int defence, int minAtack,
        int maxAttack)
    {
        this.level.text = "Level: " + level;
        this.experience.text = "XP: " + experience + "/" + maxExperience;
        this.maxHealth.text = "HP: " + health + "/" + maxHealth;
        this.maxFocus.text = "Focus " + focus + "/" + maxFocus;
        defense.text = "Defense: " + defence;
        attack.text = "Damage per Hit:\n" + minAtack + " to " + maxAttack;
        experienceSlider.value = (float) experience / maxExperience;
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        PanelManagement.instance.SetRightPanel(gameObject.activeSelf ? gameObject : null);
    }
}

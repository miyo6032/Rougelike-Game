using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Displays the statistics of the player
/// </summary>
public class PlayerStatUI : MonoBehaviour
{
    public Text level;
    public Text experience;
    public Text maxHealth;
    public Text maxFocus;
    public Text defense;
    public Text attack;

    /// <summary>
    /// Updates the UI by passing int all of the player's statistics
    /// </summary>
    public void UpdateStatUI(int level, int experience, int maxHealth, int maxFocus, int defence, int minAtack,
        int maxAttack)
    {
        this.level.text = "Level: " + level;
        this.experience.text = experience + " to next level";
        this.maxHealth.text = "Max Health: " + maxHealth;
        this.maxFocus.text = "Max Focus: " + maxFocus;
        this.defense.text = "Total Defense: " + defence;
        this.attack.text = "Total Damage per Hit:\n" + minAtack + " to " + maxAttack;
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        if (gameObject.activeSelf)
        {
            StaticCanvasList.instance.panelManagement.SetRightPanel(gameObject);
        }
        else
        {
            StaticCanvasList.instance.panelManagement.SetRightPanel(null);
        }
    }
}

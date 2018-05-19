using UnityEngine.UI;
using UnityEngine;

public class PlayerStatUI : MonoBehaviour {

    public Text level;
    public Text experience;
    public Text maxHealth;
    public Text maxFocus;
    public Text defense;
    public Text attack;

	public void UpdateStatUI(int level, int experience, int maxHealth, int maxFocus, int defence, int minAtack, int maxAttack)
    {
        this.level.text = "Level: " + level;
        this.experience.text = experience + " to next level";
        this.maxHealth.text = "Max Health: " + maxHealth;
        this.maxFocus.text = "Max Focus: " + maxFocus;
        this.defense.text = "Total Defense: " + defence;
        this.attack.text = "Total Damage per Hit: " + minAtack + " to " + maxAttack;
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

}

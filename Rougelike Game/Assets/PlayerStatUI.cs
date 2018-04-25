using UnityEngine.UI;
using UnityEngine;

public class PlayerStatUI : MonoBehaviour {

    public Text level;
    public Text experience;
    public Text maxHealth;
    public Text maxFocus;
    public Text defense;
    public Text attack;

	public void UpdateStatUI(int l, int e, int h, int f, int d, Vector2 a)
    {
        level.text = "Level: " + l;
        experience.text = e + " to next level";
        maxHealth.text = "Max Health: " + h;
        maxFocus.text = "Max Focus: " + f;
        defense.text = "Total Defense: " + d;
        attack.text = "Total Damage per Hit: " + a.x + " to " + a.y;
    }

}

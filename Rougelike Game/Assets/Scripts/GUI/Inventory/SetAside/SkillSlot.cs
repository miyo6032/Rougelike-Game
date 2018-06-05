using UnityEngine;
using UnityEngine.UI;

//NOT IN USE
//Responsible for containing the skill information, as well as a tooltip
public class SkillSlot : MonoBehaviour {

    [HideInInspector]
    public Skill skill;
    public Image sprite;

    public bool active = false;

    //Also implicity unequipps the last skill is any because everything is overriden
    public void AddSkill(Skill s)
    {
        if (s != null)
        {
            skill = s;
            sprite.sprite = skill.Sprite;
            active = false;
            sprite.gameObject.SetActive(true);
        }
    }

    //Remove a skill
    public void RemoveSkill()
    {
        skill = null;
        sprite.gameObject.SetActive(false);
        active = false;
    }

}

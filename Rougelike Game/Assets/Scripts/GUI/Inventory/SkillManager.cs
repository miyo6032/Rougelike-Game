using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the input for skills, the active and passive skills.
public class SkillManager : MonoBehaviour {

    SkillSlot[] skillSlots = new SkillSlot[8];

    KeyCode[] skillKeys = { KeyCode.Q, KeyCode.E, KeyCode.R, KeyCode.F, KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4 };

    SkillDatabase database;

    void Start()
    {
        database = GetComponent<SkillDatabase>();
        database.ConstructSkillDatabase();
    }

    void Update()
    {
        foreach (SkillSlot skill in skillSlots)
        {
            if (Input.GetKeyDown(KeyCode.Q) && skill.skill != null)
            {
                DoTheSkill(skill);
                //the player shall not activate more than one skill at once
                break;
            }
        }
    }

    //Figures out what to do for a skill passed in, and execute the skill
    void DoTheSkill(SkillSlot s)
    {
        if(s.skill.Title == "Critical Hit")
        {

        }
    }

}

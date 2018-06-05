using UnityEngine;
using System.Collections.Generic;
using LitJson;
using System.IO;

//NOT IN USE
/// <summary>
/// Loads all of the Skill data from the json file and holds it for other scripts to use
/// </summary>
public class SkillDatabase : MonoBehaviour
{
    public Dictionary<int, Skill> Skills = new Dictionary<int, Skill>();

    //Finds the 
    public Skill GetSkillByID(int id)
    {

        Skill Skill;

        if (Skills.TryGetValue(id, out Skill))
        {
            return Skill;
        }

        return null;
    }

    //Populates the database for use - called from the InventoryManager to populate before its start
    public void ConstructSkillDatabase()
    {
        JsonData SkillData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Skills.json"));
        for (int i = 0; i < SkillData.Count; i++)
        {
            Skills.Add(
                 (int)SkillData[i]["id"], new Skill(
            (int)SkillData[i]["id"],
            SkillData[i]["title"].ToString(),
            SkillData[i]["description"].ToString(),
            (string)SkillData[i]["sprite"]
        ));
        }
    }
}

public class Skill
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Sprite Sprite { get; set; }

    public Skill(int id, string title, string description, string sprite)
    {
        Id = id;
        Title = title;
        Description = description;
        Sprite = Resources.Load<Sprite>("Skills/" + sprite);
    }

    public Skill()
    {
        Id = -1;
    }
}

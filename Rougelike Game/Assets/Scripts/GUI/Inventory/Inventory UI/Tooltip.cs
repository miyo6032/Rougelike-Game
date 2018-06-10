using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows information for items when they are hovered over
/// </summary>
public class Tooltip : MonoBehaviour
{
    public Text title;
    public Text description;

    public void ShowUpgradeTooltip(UpgradeInstance instance)
    {
        transform.position = Input.mousePosition;
        PositionRelativeToScreen();
        gameObject.SetActive(true);
        title.text = instance.upgrade.name;
        description.text = GetLockedString(instance) + GetStatsString(instance.upgrade);
    }

    string GetLockedString(UpgradeInstance instance)
    {
        return instance.isUnlocked ? "Already Unlocked\n\n" : "";
    }

    string GetStatsString(Upgrade upgrade)
    {
        string str = "";
        foreach (var stat in upgrade.statsAffected)
        {
            str += GetStatString(stat);
        }
        return str;
    }

    private string GetStatString(Stat stat)
    {
        switch (stat.stat)
        {
            case Stats.maxHealth:
                return "+" + stat.effect + " Max health\n\n";
            case Stats.baseAttack:
                return "+" + stat.effect + " Attack\n\n";
            case Stats.baseDefense:
                return "+" + stat.effect + " Defense\n\n";
            case Stats.hitSpeed:
                return "+" + stat.effect + " Hit Speed\n\n";
            case Stats.maxFocus:
                return "+" + stat.effect + " Max Focus\n\n";
            case Stats.damage:
                return "+" + stat.effect + " Damage Per Second\n\n";
            case Stats.healing:
                return "+" + stat.effect + " HP Per Second\n\n";
            default:
                return "";
        }
    }

    /// <summary>
    /// Shows the tooltip with the item data
    /// </summary>
    /// <param name="item"></param>
    public void ShowItemTooltip(Item item)
    {
        transform.position = Input.mousePosition;
        PositionRelativeToScreen();
        gameObject.SetActive(true);
        title.text = item.Title;
        description.text = GetLevelString(item) + GetAttackString(item) + GetDefenseString(item) +
                           GetValueString(item) + item.Description;
    }

    string GetLevelString(Item item)
    {
        return item.ItemLevel == 0 ? "" : "Required Level: " + item.ItemLevel + "\n\n";
    }

    string GetValueString(Item item)
    {
        return item.Value == 0 ? "" : "Value: " + item.Value + "\n\n";
    }

    string GetAttackString(Item item)
    {
        if (item.Attack == 0)
        {
            return "";
        }

        if (item.Attack == item.MaxAttack)
        {
            return item.Attack + " Damage\n\n";
        }

        return item.Attack + " - " + item.MaxAttack + " Damage\n\n";

    }

    string GetDefenseString(Item item)
    {
        return item.Defence == 0? "" : "+" + item.Defence + " Defense\n\n";
    }

    /// <summary>
    /// Depending on where the tooltip is, position it relative to the mouse so it doesn't go off the screen
    /// </summary>
    void PositionRelativeToScreen()
    {
        Vector2 pivotPosition = new Vector2
        {
            y = Input.mousePosition.y > Screen.height / 2f ? 1 : 0,
            x = Input.mousePosition.x > Screen.width / 2f ? 1 : 0
        };
        GetComponent<RectTransform>().pivot = pivotPosition;
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }
}

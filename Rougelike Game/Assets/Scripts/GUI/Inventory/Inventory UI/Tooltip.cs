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
        string str = upgrade.maxHealthMultiplier == 0 ? "" : "+" + upgrade.maxHealthMultiplier + " Max health\n\n";
        str += upgrade.attackMultiplier == 0 ? "" : "+" + upgrade.attackMultiplier + " Attack\n\n";
        str += upgrade.defenseMultiplier == 0 ? "" : "+" + upgrade.defenseMultiplier + " Defense\n\n";
        str += upgrade.hitSpeedMultiplier == 0 ? "" : "+" + upgrade.hitSpeedMultiplier + " Hit Speed\n\n";
        str += upgrade.maxFocusMultiplier == 0 ? "" : "+" + upgrade.maxFocusMultiplier + " Max Focus\n\n";
        return str;
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

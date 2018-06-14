using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows information for items when they are hovered over
/// </summary>
public class Tooltip : MonoBehaviour
{
    public Text title;
    public Text description;

    public void ShowEffectTooltip(Effect effect)
    {
        PositionTooltip();
        title.text = effect.name;
        string str = "";
        description.text = GetStatsString(effect.ModifiersAffected);
    }

    /// <summary>
    /// When hovering over an upgrade in the upgrade tree, show its stats
    /// </summary>
    /// <param name="instance"></param>
    public void ShowUpgradeTooltip(UpgradeInstance instance)
    {
        PositionTooltip();
        title.text = instance.upgrade.name;
        string lockedString = instance.isUnlocked ? "Already Unlocked\n\n" : "";
        description.text = lockedString + GetStatsString(instance.upgrade.ModifiersAffected);
    }

    string GetStatsString(Modifier[] modifiers)
    {
        string str = "";
        foreach (var stat in modifiers)
        {
            str += stat.value + GetStatString(stat) + "\n\n";
        }
        return str;
    }

    private string GetStatString(Modifier modifier)
    {
        switch (modifier.ModifierType)
        {
            case ModifierType.maxHealth:
                return " Max health";
            case ModifierType.baseAttack:
                return " Attack";
            case ModifierType.baseDefense:
                return " Defense";
            case ModifierType.hitSpeed:
                return " Hit Speed";
            case ModifierType.maxFocus:
                return " Max Focus";
            case ModifierType.damage:
                return " Damage Per Second";
            case ModifierType.healing:
                return " HP Per Second";
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
        PositionTooltip();
        title.text = item.Title;
        string str = item.ItemLevel == 0 ? "" : "Required Level: " + item.ItemLevel + "\n\n";
        str += item.Value == 0 ? "" : "Value: " + item.Value + "\n\n";
        str += GetAttackString(item);
        str += item.Defence == 0 ? "" : "+" + item.Defence + " Defense\n\n";
        description.text = str;

    }

    void PositionTooltip()
    {
        transform.position = Input.mousePosition;
        PositionRelativeToScreen();
        gameObject.SetActive(true);
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

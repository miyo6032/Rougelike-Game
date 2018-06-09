using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows information for items when they are hovered over
/// </summary>
public class Tooltip : MonoBehaviour
{
    public Text title;
    public Text description;

    /// <summary>
    /// Shows the tooltip with the item data
    /// </summary>
    /// <param name="item"></param>
    public void ShowTooltip(Item item)
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
        if (item.ItemLevel == 0)
        {
            return "";
        }

        return "Required Level: " + item.ItemLevel + "\n\n";
    }

    string GetValueString(Item item)
    {
        if (item.Value == 0)
        {
            return "";
        }

        return "Value: " + item.Value + "\n\n";
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
        if (item.Defence == 0)
        {
            return "";
        }

        return "+" + item.Defence + " Defense\n\n";
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

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows information for items when they are hovered over
/// </summary>
public class Tooltip : MonoBehaviour
{
    public static Tooltip instance;
    public Text title;
    public Text description;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Duplicate " + this.GetType().Name);
            Destroy(gameObject);
        }
        gameObject.SetActive(false);
    }

    public void ShowEffectTooltip(Effect effect)
    {
        PositionTooltip();
        title.text = effect.name;
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

    /// <summary>
    /// Iterates through all of the modifiers and puts them all into a string
    /// </summary>
    /// <param name="modifiers"></param>
    /// <returns></returns>
    private string GetStatsString(Modifier[] modifiers)
    {
        var str = "";
        var builder = new System.Text.StringBuilder();
        builder.Append(str);
        foreach (var stat in modifiers)
        {
            if (stat.modifierType == StatModifierType.linear)
            {
                builder.Append(stat.value + GetStatString(stat) + "\n\n");
            }
            else if (stat.modifierType == StatModifierType.basePercent)
            {
                builder.Append(stat.value + "% base" + GetStatString(stat) + "\n\n");
            }
            if (stat.modifierType == StatModifierType.afterPercent)
            {
                builder.Append(stat.value + "%" + GetStatString(stat) + "\n\n");
            }
        }
        str = builder.ToString();
        return str;
    }

    /// <summary>
    /// Map a stat to its string name
    /// </summary>
    /// <param name="modifier"></param>
    /// <returns></returns>
    private string GetStatString(Modifier modifier)
    {
        switch (modifier.statToModify)
        {
            case PlayerStatModifier.maxHealth:
                return " Max health";

            case PlayerStatModifier.minAttack:
                return " Attack";

            case PlayerStatModifier.maxAttack:
                return " MaxAttack";

            case PlayerStatModifier.defense:
                return " Defense";

            case PlayerStatModifier.hitSpeed:
                return " Hit Speed";

            case PlayerStatModifier.maxFocus:
                return " Max Focus";

            case PlayerStatModifier.damage:
                return " Damage Per Second";

            case PlayerStatModifier.healing:
                return " HP Per Second";

            case PlayerStatModifier.movementDelay:
                return " Movement Delay";

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
        Modifier defenseMod = GetModifier(item, PlayerStatModifier.defense);

        PositionTooltip();
        title.text = item.Title;
        string str = item.ItemLevel == 0 ? "" : "Required Level: " + item.ItemLevel + "\n\n";
        str += GetAttackString(item);
        str += defenseMod.value == 0 ? "" : "+" + defenseMod.value + " Defense\n\n";
        str += item.focusConsumption == 0 ? "" : "Required focus: " + item.focusConsumption + "\n\n";
        str += item.Value == 0 ? "" : "Value: " + item.Value + "\n\n";
        str += item.Description;
        description.text = str;
    }

    /// <summary>
    /// Position the tooltip in relation to the mouse screen position
    /// </summary>
    private void PositionTooltip()
    {
        transform.position = Input.mousePosition;
        PositionRelativeToScreen();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Do some processing to return an attack string that describes the item's attack
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private string GetAttackString(Item item)
    {
        Modifier minAttack = GetModifier(item, PlayerStatModifier.minAttack);
        Modifier maxAttack = GetModifier(item, PlayerStatModifier.maxAttack);
        if (maxAttack.value == 0)
        {
            return "";
        }

        if (minAttack.value == maxAttack.value)
        {
            return minAttack.value + " Damage\n\n";
        }

        return minAttack.value + " - " + maxAttack.value + " Damage\n\n";
    }

    /// <summary>
    /// Returns a modifier based on its type, or a useless modifier is there is none
    /// </summary>
    private Modifier GetModifier(Item item, PlayerStatModifier modifierType)
    {
        foreach (var modifier in item.equipmentModifiers)
        {
            if (modifier.statToModify == modifierType)
            {
                return modifier;
            }
        }

        return new Modifier(modifierType, StatModifierType.linear, 0);
    }

    /// <summary>
    /// Depending on where the tooltip is, position it relative to the mouse so it doesn't go off the screen
    /// </summary>
    private void PositionRelativeToScreen()
    {
        Vector2 pivotPosition = new Vector2
        {
            y = Input.mousePosition.y > Screen.height / 2f ? 1 : 0,
            x = Input.mousePosition.x > Screen.width / 2f ? 1 : 0
        };
        GetComponent<RectTransform>().pivot = pivotPosition;
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }
}
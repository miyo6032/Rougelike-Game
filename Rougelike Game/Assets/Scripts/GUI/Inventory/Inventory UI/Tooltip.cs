using UnityEngine;
using UnityEngine.UI;

//Shows information for items when hovered over
public class Tooltip : MonoBehaviour {

    public Text title;
    public Text description;

    public void ShowTooltip(Item item)
    {
        transform.position = Input.mousePosition;
        PositionRelativeToScreen();
        gameObject.SetActive(true);
        title.text = item.Title;
        description.text = GetLevelString(item) + GetAttackString(item) + GetDefenseString(item) + GetValueString(item) + item.Description;
    }

    string GetLevelString(Item item)
    {
        if(item.ItemLevel == 0)
        {
            return "";
        }
        return "Required Level: " + item.ItemLevel + "\n\n";
    }

    string GetValueString(Item item)
    {
        if(item.Value == 0)
        {
            return "";
        }
        return "Value: " + item.Value + "\n\n";
    }

    string GetAttackString(Item item)
    {
        if(item.Attack == 0 && item.MaxAttack == 0)
        {
            return "";
        }
        if(item.Attack != item.MaxAttack)
        {
            return "Attack: " + item.Attack + " - " + item.MaxAttack + "\n\n";
        }
        return "Attack: " + item.Attack + "\n\n";
    }

    string GetDefenseString(Item item)
    {
        if(item.Defence == 0)
        {
            return "";
        }
        return "Defense: " + item.Defence + "\n\n";
    }

    //Depending on where the tooltip is, position it relative to the mouse so it doesn't go off the screen
    void PositionRelativeToScreen()
    {
        Vector2 pivotPosition = new Vector2(0, 0);
        if (Input.mousePosition.y > Screen.height/2)
        {
            pivotPosition.y = 1;
        }
        else
        {
            pivotPosition.y = 0;
        }
        if(Input.mousePosition.x > Screen.width / 2)
        {
            pivotPosition.x = 1;
        }
        else
        {
            pivotPosition.x = 0;
        }
        GetComponent<RectTransform>().pivot = pivotPosition;
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

}

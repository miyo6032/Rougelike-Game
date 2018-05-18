using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    public Text title;
    public Text description;
    public CanvasGroup canvas;

    public void ShowTooltip(Item item)
    {
        transform.position = Input.mousePosition;
        gameObject.SetActive(true);
        title.text = item.Title;
        description.text = "Required Level: " + item.ItemLevel + "\n" +
            GetAttackString(item) +
            "Defense: " + item.Defence + "\n" + 
            "Value: " + item.Value +
            item.Description;
    }

    string GetAttackString(Item item)
    {
        if(item.Attack != item.MaxAttack)
        {
            return "Attack: " + item.Attack + " - " + item.MaxAttack + "\n";
        }
        return "Attack: " + item.Attack + "\n";
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

}

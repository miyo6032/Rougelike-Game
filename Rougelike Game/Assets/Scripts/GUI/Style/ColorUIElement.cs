using UnityEngine;
using UnityEngine.UI;

//Automatically colors the element it is attached to, so that makes is really easy to change color schemes
public class ColorUIElement : MonoBehaviour {

	public enum UIElements
    {
        darkButton,
        lightButton,
        mainPanel,
        secondaryPanel
    };

    public UIElements UIElement = UIElements.darkButton;

    void Start()
    {
        switch (UIElement)
        {
            case UIElements.darkButton:
                ColorButton(StaticCanvasList.instance.uiColoring.DARK_BUTTON_NORMAL, StaticCanvasList.instance.uiColoring.DARK_BUTTON_PRESSED);
                break;
            case UIElements.lightButton:
                ColorButton(StaticCanvasList.instance.uiColoring.LIGHT_BUTTON_NORMAL, StaticCanvasList.instance.uiColoring.LIGHT_BUTTON_PRESSED);
                break;
            case UIElements.mainPanel:
                ColorImage(StaticCanvasList.instance.uiColoring.MAIN_PANEL);
                break;
            case UIElements.secondaryPanel:
                ColorImage(StaticCanvasList.instance.uiColoring.SECONDARY_PANEL);
                break;
        }
    }

    void ColorButton(Color colorNormal, Color colorPressed)
    {
        ColorBlock button = GetComponent<Button>().colors;
        button.normalColor = colorNormal;
        button.pressedColor = colorPressed;
        button.highlightedColor = colorNormal;
        GetComponent<Button>().colors = button;
    }

    void ColorImage(Color color)
    {
        GetComponent<Image>().color = color;
    }

}

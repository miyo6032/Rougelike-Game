﻿using UnityEngine;

/// <summary>
/// Holds the coloring for most ui elements to easily change color schemes for ui
/// </summary>
public class UIColoring : MonoBehaviour
{
    public Color MAIN_PANEL = Color.black;
    public Color SECONDARY_PANEL = Color.black;
    public Color DARK_BUTTON_PRESSED = Color.black;
    public Color DARK_BUTTON_NORMAL = Color.black;
    public Color LIGHT_BUTTON_PRESSED = Color.black;
    public Color LIGHT_BUTTON_NORMAL = Color.black;
    public static UIColoring instance;

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
    }
}

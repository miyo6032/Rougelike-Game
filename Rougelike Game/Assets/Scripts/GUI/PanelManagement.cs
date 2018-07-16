using UnityEngine;

/// <summary>
/// When inventory panels conflict i.e. chest inventory and player stats, will resolve those conflicts
/// </summary>
public class PanelManagement : MonoBehaviour
{
    GameObject rightPanel;
    public static PanelManagement instance;

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

    /// <summary>
    /// Keeps track of the right panel to keep only one panel open at a time
    /// </summary>
    /// <param name="newPanel"></param>
    public void SetRightPanel(GameObject newPanel)
    {
        if (rightPanel != null && rightPanel != newPanel)
        {
            rightPanel.SetActive(false);
        }

        rightPanel = newPanel;
    }
}

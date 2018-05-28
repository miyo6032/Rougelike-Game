using UnityEngine;

//When inventory panels conflict i.e. chest inventory and player stats, will resolve those conflicts
public class PanelManagement : MonoBehaviour {

    GameObject rightPanel;

    public void SetRightPanel(GameObject newPanel)
    {
        if(rightPanel != null && rightPanel != newPanel)
        {
            rightPanel.SetActive(false);
        }
        rightPanel = newPanel;
    }

}

using UnityEngine;

public class StaticCanvasList : MonoBehaviour {

    public static StaticCanvasList instance;

    public InventoryManager inventoryManager;
    public InGameUI gameUI;
    public PauseUI pauseUI;

	void Start () {
		if(instance == null)
        {
            instance = this;
        }
        else
        {
            if(instance != this)
            {
                Destroy(gameObject);
            }
        }
	}
}

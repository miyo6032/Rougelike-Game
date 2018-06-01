using UnityEngine;

//Used for scripts to reference instances of ui canvases in the scene
public class StaticCanvasList : MonoBehaviour {

    public static StaticCanvasList instance;

    public InventoryManager inventoryManager;
    public InGameUI gameUI;
    public PauseUI pauseUI;
    public PlayerStatUI statUI;
    public Tooltip inventoryTooltip;
    public UIColoring uiColoring;
    public LootInventory lootInventory;
    public ChestInventory chestInventory;
    public ItemGenerator itemGenerator;
    public ItemModuleDatabase itemModuleDatabase;
    public ItemDatabase itemDatabase;
    public TextureDatabase textureDatabase;
    public PanelManagement panelManagement;
    public ItemDropGenerator itemDropGenerator;

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

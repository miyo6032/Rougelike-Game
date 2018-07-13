using UnityEngine;

/// <summary>
/// Used by scripts to reference instances of ui canvases in the scene
/// </summary>
public class StaticCanvasList : MonoBehaviour {

    public static StaticCanvasList instance;

    public InventoryManager inventoryManager;
    public InGameUI gameUI;
    public PlayerStatUI statUI;
    public Tooltip inventoryTooltip;
    public UIColoring uiColoring;
    public LootInventory lootInventory;
    public ChestInventory chestInventory;
    public ModuledItemGenerator moduledItemGenerator;
    public PresetItemDatabase presetItemDatabase;
    public ItemModuleDatabase itemModuleDatabase;
    public ItemDatabase itemDatabase;
    public TextureDatabase textureDatabase;
    public PanelManagement panelManagement;
    public ItemDropGenerator itemDropGenerator;
    public SkillTree skillTree;
    public DialoguePanel dialoguePanel;

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

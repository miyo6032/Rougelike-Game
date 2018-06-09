using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The button that represents a node in the upgrade tree
/// </summary>
public class UpgradeInstance : MonoBehaviour
{
    public Upgrade upgrade;
    private Image buttonImage;
    public bool isUnlocked;
    public bool canBeUnlocked;
    public bool isEmpty;
    public Image downLink;
    public Image rightLink;
    public int id;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        if (!isEmpty)
        {
            buttonImage.sprite = upgrade.upgradeSprite;
        }
    }

    /// <summary>
    /// Called when the button is clicked and the player can upgrade
    /// </summary>
    public void ApplyUpgrade()
    {
        if (isUnlocked || !canBeUnlocked) return;

        if(StaticCanvasList.instance.skillTree.ApplyUpgrade(this))
            isUnlocked = true;
    }

    public void SetCanBeApplied(bool canBeApplied)
    {
        if (!canBeUnlocked)
        {
            canBeUnlocked = canBeApplied;
        }
    }
}

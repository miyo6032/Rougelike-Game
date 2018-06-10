using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generates and handles the skill tree ui
/// </summary>
public class SkillTree : MonoBehaviour
{
    private List<UpgradeInstance> upgrades;
    private List<Image> rims;
    public PlayerStats playerStats;
    public Transform content;
    public Transform rimTranform;
    public Transform backgroundTransform;
    public Image image;
    public Text upgradePointsText;
    public int treeRowWidth = 7;
    [Header("Sprites")] public Sprite sideLink;
    public Sprite downLink;
    public Sprite unlockedSideLink;
    public Sprite unlockedDownLink;
    public Sprite background;

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        StaticCanvasList.instance.inventoryTooltip.gameObject.SetActive(false);
    }

    /// <summary>
    /// Calculate which upgrades can be unlocked next and update them
    /// </summary>
    /// <param name="instance"></param>
    public bool ApplyUpgrade(UpgradeInstance instance)
    {
        if (playerStats.GetUpgradePoints() > 0)
        {
            playerStats.AddUpgrade(instance.upgrade);
            rims[instance.id].sprite = instance.upgrade.unlockedRimSprite;
            SetNeighborsUnlockable(instance.id);
            playerStats.UseUpgradePoint();
            return true;
        }
        return false;
    }

    private void Start()
    {
        upgrades = content.GetComponentsInChildren<UpgradeInstance>().ToList();
        rims = new List<Image>();
        InitializeUpgrades();
        upgrades[1].SetCanBeApplied(true);
        upgrades[3].SetCanBeApplied(true);
        upgrades[5].SetCanBeApplied(true);
    }

    /// <summary>
    /// Render upgrade images and handle first unlockable upgrades
    /// </summary>
    private void InitializeUpgrades()
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            Image rimInstance = Instantiate(image, rimTranform);
            rims.Add(rimInstance);
            Image backgroundInstance = Instantiate(image, backgroundTransform);
            if (!upgrades[i].isEmpty)
            {
                upgrades[i].id = i;
                SetupLinks(i);
                if (upgrades[i].isUnlocked)
                {
                    SetNeighborsUnlockable(i);
                }

                rimInstance.sprite = upgrades[i].upgrade.rimSprite;
                backgroundInstance.sprite = background;
            }
        }
    }

    /// <summary>
    /// Set up an upgrade's two links: down and right
    /// </summary>
    /// <param name="i"></param>
    private void SetupLinks(int i)
    {
        if (RightExists(i))
        {
            upgrades[i].rightLink.sprite = sideLink;
        }

        if (DownExists(i))
        {
            upgrades[i].downLink.sprite = downLink;
        }
    }

    private bool LeftExists(int i)
    {
        int left = i - 1;
        return left >= 0 && i % treeRowWidth != 0 && !upgrades[left].isEmpty;
    }

    private bool RightExists(int i)
    {
        int right = i + 1;
        return right < upgrades.Count && right % treeRowWidth != 0 && !upgrades[right].isEmpty;
    }

    private bool UpExists(int i)
    {
        int up = i - treeRowWidth;
        return up >= 0 && !upgrades[up].isEmpty;
    }

    private bool DownExists(int i)
    {
        int down = i + treeRowWidth;
        return down < upgrades.Count && !upgrades[down].isEmpty;
    }

    /// <summary>
    /// Set the neighbors (up left down right) of the unlocked skill to unlockable
    /// </summary>
    /// <param name="i"></param>
    private void SetNeighborsUnlockable(int i)
    {
        int left = i - 1;
        int right = i + 1;
        int up = i - treeRowWidth;
        int down = i + treeRowWidth;
        if (LeftExists(i))
        {
            if (upgrades[left].isUnlocked)
            {
                upgrades[left].rightLink.sprite = unlockedSideLink;
            }
            else
            {
                upgrades[left].SetCanBeApplied(true);
            }
        }

        if (RightExists(i))
        {
            if (upgrades[right].isUnlocked)
            {
                upgrades[i].rightLink.sprite = unlockedSideLink;
            }
            else
            {
                upgrades[right].SetCanBeApplied(true);
            }
        }

        if (UpExists(i))
        {
            if (upgrades[up].isUnlocked)
            {
                upgrades[up].downLink.sprite = unlockedDownLink;
            }
            else
            {
                upgrades[up].SetCanBeApplied(true);
            }
        }

        if (DownExists(i))
        {
            if (upgrades[down].isUnlocked)
            {
                upgrades[i].downLink.sprite = unlockedDownLink;
            }
            else
            {
                upgrades[down].SetCanBeApplied(true);
            }
        }
    }
}

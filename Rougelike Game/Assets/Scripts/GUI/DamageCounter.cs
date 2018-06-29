using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// For the damage counter prefab - the rest is done in animation
/// </summary>
public class DamageCounter : MonoBehaviour
{
    public Text damageText;

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    public void SetText(string text)
    {
        damageText.text = text;
    }
}

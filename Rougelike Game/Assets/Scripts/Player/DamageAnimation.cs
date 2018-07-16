using UnityEngine;

/// <summary>
/// Used to animate the player's damage animation
/// </summary>
public class DamageAnimation : MonoBehaviour
{
    //Time to animate the redness
    public float animationTime;

    //Have to animate each individual sprite renderer
    public SpriteRenderer[] spriteRenderers;

    //Because the armors are also done by colors, we have to remember what the original colors were after turning the player red
    Color[] originalColors;
    bool animating;
    float lerpAccumulator;

    public void Update()
    {
        if (animating)
        {
            lerpAccumulator += Time.deltaTime;
            for (int i = 0; i < originalColors.Length; i++)
            {
                spriteRenderers[i].color = Color.Lerp(spriteRenderers[i].color, originalColors[i],
                    lerpAccumulator / animationTime);
                if (spriteRenderers[i].color == originalColors[i])
                {
                    animating = false;
                }
            }
        }
    }

    /// <summary>
    /// Called by the player's animation in editor to start the animation
    /// </summary>
    public void StartAnimation()
    {
        SetOriginalColors();
        if (!animating)
        {
            lerpAccumulator = 0f;
            animating = true;
            for (int i = 0; i < originalColors.Length; i++)
            {
                originalColors[i] = spriteRenderers[i].color;
                spriteRenderers[i].color = Color.red;
            }
        }
    }

    /// <summary>
    /// Remember the original colors to return from being turned red
    /// </summary>
    public void SetOriginalColors()
    {
        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < originalColors.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }
    }
}

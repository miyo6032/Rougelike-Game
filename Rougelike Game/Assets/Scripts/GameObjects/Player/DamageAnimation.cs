using UnityEngine;

public class DamageAnimation : MonoBehaviour {

    public float animationTime;

    public SpriteRenderer[] spriteRenderers;

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
                spriteRenderers[i].color = Color.Lerp(spriteRenderers[i].color, originalColors[i], lerpAccumulator / animationTime);
                if (spriteRenderers[i].color == originalColors[i])
                {
                    animating = false;
                }
            }
        }
    }

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

    public void SetOriginalColors()
    {
        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < originalColors.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }
    }

}

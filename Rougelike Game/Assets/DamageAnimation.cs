using UnityEngine;

public class DamageAnimation : MonoBehaviour {

    public SpriteRenderer spriteRenderer;

    Color originalColor;

    bool animating;

    public void Update()
    {
        if (animating)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, Time.deltaTime);
            if(spriteRenderer.color == originalColor)
            {
                animating = false;
            }
        }
    }

	public void StartAnimation()
    {
        if (!animating)
        {
            animating = true;
            originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
        }
    }

}

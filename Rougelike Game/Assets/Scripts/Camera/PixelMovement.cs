using UnityEngine;

/// <summary>
/// In charge of rounding the camera movement to maintain crisp pixels
/// </summary>
public class PixelMovement : MonoBehaviour
{
    private Vector3 position;

    private void Awake()
    {
        position = transform.position;
        transform.position = position + new Vector3(-0.1f, -0.1f);
    }

    private void LateUpdate()
    {
        transform.position = position + new Vector3(-0.1f, -0.1f);
    }

    public void UpdatePosition(Vector3 pos)
    {
        position = pos;
    }
}

using UnityEngine;

//In charge of rounding the camera movement so there is no pixel weirdness
public class PixelMovement : MonoBehaviour {

    Vector3 position;

	void Awake()
    {
        position = transform.position;
        transform.position = position + new Vector3(-0.1f, -0.1f);
    }

    void LateUpdate()
    {
        transform.position = position + new Vector3(-0.1f, -0.1f);
    }

    public void UpdatePosition(Vector3 pos)
    {
        position = pos;
    }

}

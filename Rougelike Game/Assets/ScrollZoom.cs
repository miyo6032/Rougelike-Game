using UnityEngine;

/// <summary>
/// Zoom in and out for the upgrade tree
/// </summary>
public class ScrollZoom : MonoBehaviour
{
    public float maxScale = 2;
    public float minScale = 0.3f;
    public float scrollSensitivity = 1;

    public float scale = 1;

	void Update () {
	    //Scrolling zooms in or out
	    if (Input.GetAxis("Mouse ScrollWheel") != 0)
	    {
	        scale = Mathf.Clamp(scale + Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity, minScale, maxScale);
	        transform.localScale = new Vector3(scale, scale);
	    }
    }
}

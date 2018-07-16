using UnityEngine;

/// <summary>
/// Used by scripts to reference instances of ui canvases in the scene
/// </summary>
public class StaticCanvasList : MonoBehaviour {

    public static StaticCanvasList instance;

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

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangeColorOnHover : MonoBehaviour {

    public Text text;



    public void changeColorToYellow() {

        text.color = new Color(1, 0.8f, 0.35f);

    }

    public void changeColorToWhite()
    {

        text.color = Color.white;

    }

}

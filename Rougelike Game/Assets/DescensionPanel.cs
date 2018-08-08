using UnityEngine;
using UnityEngine.UI;

public class DescensionPanel : MonoBehaviour
{
    public static DescensionPanel instance;
    public Text transitionText;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Duplicate DescensionPanels in Scene!");
        }
        gameObject.SetActive(false);
    }

    public void StartAnimation(string text)
    {
        gameObject.SetActive(true);
        GetComponent<Animator>().SetTrigger("Descend");
        transitionText.text = text;
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
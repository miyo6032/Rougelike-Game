using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartNewGame : MonoBehaviour {

	public void LoadByIndex(int sceneIndex){

        SceneManager.LoadScene(sceneIndex);

	}

}

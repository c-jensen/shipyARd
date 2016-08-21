using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class StartGameScript : MonoBehaviour {

	// Update is called once per frame
	public void ChangeToScene(int sceneToLoad) {
            Application.LoadLevel(sceneToLoad);
    }
}

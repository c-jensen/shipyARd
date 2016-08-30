using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class StartGameScript : MonoBehaviour
{

    //This function is a conveniance function to load a different scene
    public void ChangeToScene(int sceneToLoad)
    {
        Application.LoadLevel(sceneToLoad);
    }
}

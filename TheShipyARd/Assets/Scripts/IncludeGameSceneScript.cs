using UnityEngine;
using System.Collections;

public class IncludeGameSceneScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Application.LoadLevelAdditive("GameScene");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
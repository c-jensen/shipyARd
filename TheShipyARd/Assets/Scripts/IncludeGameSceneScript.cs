using UnityEngine;
using System.Collections;

public class IncludeGameSceneScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Application.LoadLevelAdditive("GameScene");
    }
}
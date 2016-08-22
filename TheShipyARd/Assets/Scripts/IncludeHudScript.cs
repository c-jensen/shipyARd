using UnityEngine;
using System.Collections;

public class IncludeHudScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Application.LoadLevelAdditive("HudScene");
    }
}
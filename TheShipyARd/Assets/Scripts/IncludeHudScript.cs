using UnityEngine;
using System.Collections;

public class IncludeHudScript : MonoBehaviour
{
    void Start()
    {
        //Additive loading of the hud scene into the rendering scene
        //this allows the use of multiple scenes in Unity for better overview
        //Rendering, Gamestate and HUD can stay isolated and are added together for the final scene
        Application.LoadLevelAdditive("HudScene");
    }
}
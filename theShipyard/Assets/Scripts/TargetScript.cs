using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Target script is responsible for displaying the right target icon on the top right hand corner in the HUD

public class TargetScript : MonoBehaviour
{

    public Image targetComponent;

    //different image types are set within Unity to the variables
    public Sprite targetImageUnknown;

    void Awake()
    {
        //executed when starting up
        setImageUnknown();
    }

    //executed when starting and no target is known yet
    public void setImageUnknown()
    {
        targetComponent.sprite = targetImageUnknown;
    }

    //function to set the right image 
    public void setImage(string path)
    {
        targetComponent.sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
    }
}

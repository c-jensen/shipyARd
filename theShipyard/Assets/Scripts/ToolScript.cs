using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//this script is responsible for setting the right image to the left hand lower corner depending on the active tool

public class ToolScript : MonoBehaviour
{

    public Image toolComponent;

    //different sprites for the tool types as set inside of Unity
    public Sprite toolImageUnknown;
    public Sprite toolImageHandcuffs;
    public Sprite toolImageInjection;
    public Sprite toolImageRope;

    void Awake()
    {
        //on startup set tool unknown image
        setImageUnknown();
    }

    public void setImageUnknown()
    {
        //set image to unknown
        toolComponent.sprite = toolImageUnknown;
    }

    //depending on toolID, set the image of the HUD to the right tool
    public void setImage(int toolID)
    {
        if (toolID == (int)Tool.HANDCUFFS)
            toolComponent.sprite = toolImageHandcuffs;
        else if (toolID == (int)Tool.INJECTION)
            toolComponent.sprite = toolImageInjection;
        else if (toolID == (int)Tool.ROPE)
            toolComponent.sprite = toolImageRope;
        else if (toolID == (int)Tool.NONE)
            toolComponent.sprite = toolImageUnknown;
    }
}

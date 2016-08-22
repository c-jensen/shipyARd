using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToolScript : MonoBehaviour {

    public Image toolComponent;

    public Sprite toolImageUnknown;
    public Sprite toolImageHandcuffs;
    public Sprite toolImageInjection;
    public Sprite toolImageRope;

    // Use this for initialization
    void Awake ()
    {
        setImageUnknown();
    }
	
    public void setImageUnknown()
    {
        toolComponent.sprite = toolImageUnknown;
    }

    public void setImage(int toolID) //method to set our image
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

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
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void setImageUnknown()
    {
        Debug.LogError("testerino");
        toolComponent.sprite = toolImageUnknown;
    }

    public void setImage(int toolID) //method to set our image
    {
        Debug.LogError("ToolDebug: Image gets set to " + toolID);
        if (toolID == (int)Tool.HANDCUFF)
            toolComponent.sprite = toolImageHandcuffs;
        else if (toolID == (int)Tool.INJECTION)
            toolComponent.sprite = toolImageInjection;
        else if (toolID == (int)Tool.ROPE)
            toolComponent.sprite = toolImageRope;
        else if (toolID == (int)Tool.NONE)
            toolComponent.sprite = toolImageUnknown;
    }
}

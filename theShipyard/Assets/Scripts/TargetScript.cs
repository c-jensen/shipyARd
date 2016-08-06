using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TargetScript : MonoBehaviour {

    Image targetComponent;

    public Sprite targetImageUnknown;
    public Sprite targetImage1;
    public Sprite targetImage2;
    public Sprite targetImage3;
    public Sprite targetImage4;
    public Sprite targetImage5;

    // Use this for initialization
    void Start () {
        targetComponent = GetComponent<Image>(); //Our image component is the one attached to this gameObject
        setImageUnknown();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setImageUnknown() //method to set our first image
    {
        targetComponent.sprite = targetImageUnknown;
    }

    public void setImage1() //method to set our first image
    {
        targetComponent.sprite = targetImage1;
    }

    public void setImage2() //method to set our second image
    {
        targetComponent.sprite = targetImage2;
    }

    public void setImage3() //method to set our third image
    {
        targetComponent.sprite = targetImage3;
    }

    public void setImage4() //method to set our fourth image
    {
        targetComponent.sprite = targetImage4;
    }

    public void setImage5() //method to set our fifth image
    {
        targetComponent.sprite = targetImage5;
    }
}

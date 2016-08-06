using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToolScript : MonoBehaviour {

    Image toolComponent;

    public Sprite toolUnknownImage;
    public Sprite toolImage1;
    public Sprite toolImage2;
    public Sprite toolImage3;

    // Use this for initialization
    void Start () {
        toolComponent = GetComponent<Image>(); //Our image component is the one attached to this gameObject
        setImageUnknown();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setImageUnknown() //method to set our first image
    {
        toolComponent.sprite = toolUnknownImage;
    }

    public void setImage1() //method to set our first image
    {
        toolComponent.sprite = toolImage1;
    }

    public void setImage2() //method to set our second image
    {
        toolComponent.sprite = toolImage2;
    }

    public void setImage3() //method to set our third image
    {
        toolComponent.sprite = toolImage3;
    }
}

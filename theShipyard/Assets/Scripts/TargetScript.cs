using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TargetScript : MonoBehaviour {

    public Image targetComponent;

    public Sprite targetImageUnknown;
    public Sprite targetImage0;
    public Sprite targetImage1;
    public Sprite targetImage2;
    public Sprite targetImage3;
    public Sprite targetImage4;
    public Sprite targetImage5;
    public Sprite targetImage6;
    public Sprite targetImage7;
    public Sprite targetImage8;
    public Sprite targetImage9;

    public Sprite targetImage0_Successful;
    public Sprite targetImage1_Successful;
    public Sprite targetImage2_Successful;
    public Sprite targetImage3_Successful;
    public Sprite targetImage4_Successful;
    public Sprite targetImage5_Successful;
    public Sprite targetImage6_Successful;
    public Sprite targetImage7_Successful;
    public Sprite targetImage8_Successful;
    public Sprite targetImage9_Successful;

    public Sprite targetImage0_Failed;
    public Sprite targetImage1_Failed;
    public Sprite targetImage2_Failed;
    public Sprite targetImage3_Failed;
    public Sprite targetImage4_Failed;
    public Sprite targetImage5_Failed;
    public Sprite targetImage6_Failed;
    public Sprite targetImage7_Failed;
    public Sprite targetImage8_Failed;
    public Sprite targetImage9_Failed;

    // Use this for initialization
    void Awake () {
        setImageUnknown();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setImageUnknown() //method to set our first image
    {
        targetComponent.sprite = targetImageUnknown;
    }

    public void setImage(string path) //method to set our first image
    {
        targetComponent.sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
    }
}

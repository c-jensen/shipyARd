using UnityEngine;
using System.Collections;

public class Tmp_cubeRotationOnGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    float angle = 10;
    Vector3 axis = new Vector3(1, 1, 1);
    float anglePerUpdate = 2f;

    void OnGUI()
    {
        GUIStyle localStyle = new GUIStyle(GUI.skin.button);
        localStyle.fontSize = 30;


        //create another button below "Rotate Once".
        //this is a  RepeatButton that will continue to perform its action every update
        if (GUI.RepeatButton(new Rect(0, 100, 500, 200), "Rotate Continuous", localStyle))
        {
            //if button is pressed, perform the following
            //rotate the object at a specified speed, around the specified axis
            //take the existing rotation and add a little bit to it
            this.transform.rotation = this.transform.rotation * Quaternion.AngleAxis(anglePerUpdate, axis);
        }
    }
}

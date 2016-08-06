using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreScript : MonoBehaviour {

    public Text scoreReference; //Reference to our scorevalue
    public Text scoreText; //The number that shall be shown in the score textblock

    // Use this for initialization
    void Start () {
        scoreReference = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HighscoreScript : MonoBehaviour {

    public bool firstRound;
    public List<int> highscores;

	// Use this for initialization
	void Start () {
        highscores = new List<int>();
        firstRound = true;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

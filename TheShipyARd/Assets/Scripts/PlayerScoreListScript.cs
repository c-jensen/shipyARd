using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerScoreListScript : MonoBehaviour {

    public GameObject playerScoreEntryPrefab;
    public HighscoreScript highscoreScript;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //creates the leaderboard
    public void initScoreList()
    {
        highscoreScript = GameObject.Find("HighscoreScriptObject").GetComponent<HighscoreScript>();

        //loop over sorted highscore list
        for (int i = 0; i < highscoreScript.sortedHighscores.Count; i++)
        {
            //create one line in the table for each player
            GameObject playerScoreEntry_go = Instantiate(playerScoreEntryPrefab);
            playerScoreEntry_go.transform.SetParent(this.transform);
            int cast_markerID = highscoreScript.sortedMarkerIDs[i] + 1;
            playerScoreEntry_go.transform.Find("PlayerIDText").GetComponent<Text>().text = "Player " + cast_markerID.ToString();
            playerScoreEntry_go.transform.Find("PlayerScoreText").GetComponent<Text>().text = highscoreScript.sortedHighscores[i].ToString();
        }
    }
}
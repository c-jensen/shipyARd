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

    public void initScoreList()
    {
        highscoreScript = GameObject.Find("GameSceneScripts").GetComponent<HighscoreScript>();
        for(int i = 0; i < highscoreScript.sortedHighscores.Count; i++)
        {
            GameObject go = Instantiate(playerScoreEntryPrefab);
            go.transform.SetParent(this.transform);
            go.transform.Find("PlayerIDText").GetComponent<Text>().text = "Player " + highscoreScript.sortedPlayerIDs[i].ToString();
            go.transform.Find("PlayerScoreText").GetComponent<Text>().text = highscoreScript.sortedHighscores[i].ToString();
        }
    }
}
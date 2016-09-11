using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HighscoreScript : MonoBehaviour {

    //check if list is initialized
    public bool firstRound;
    
    //lists sorted by player id
    public List<int> playerIDs;
    public List<int> highscores;

    //list sorted by highest score
    public List<int> sortedPlayerIDs;
    public List<int> sortedHighscores;

    // This method prevents the Game Object from beeing destroyed, if the scene is changed
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        highscores          = new List<int>();
        sortedHighscores    = new List<int>();
        playerIDs           = new List<int>();
        sortedPlayerIDs     = new List<int>();

        firstRound = true;
    }

    //This method sorts the lists by highest score values
    public void sort()
    {
        //init sorted lists with current player and scores first sorted by player ID
        sortedHighscores.Clear();
        sortedPlayerIDs.Clear();

        for (int i = 0; i < highscores.Count; i++)
        {
            sortedHighscores.Add(highscores[i]);
            sortedPlayerIDs.Add(playerIDs[i]);
        }

        int tmpScore;
        int tmpPlayerID;
        int highestIndex;

        for (int i = 0; i < sortedHighscores.Count; i++)
        {
            highestIndex = i;

            //get current highstest score in the list
            for (int j = i + 1; j < sortedHighscores.Count; j++)
            {
                if (sortedHighscores[highestIndex] < sortedHighscores[j])
                {
                    highestIndex = j;
                }
            }

            //switch first with highest index
            tmpScore = sortedHighscores[i];
            tmpPlayerID = sortedPlayerIDs[i];
            sortedHighscores[i] = sortedHighscores[highestIndex];
            sortedHighscores[highestIndex] = tmpScore;
            sortedPlayerIDs[i] = sortedPlayerIDs[highestIndex];
            sortedPlayerIDs[highestIndex] = tmpPlayerID;
        }

        PlayerScoreListScript playerScoreListScript = GameObject.Find("PlayerScoreTable").GetComponent<PlayerScoreListScript>();
        playerScoreListScript.initScoreList();
    }   
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HighscoreScript : MonoBehaviour {

    public bool firstRound;
    public List<int> highscores;
    public List<int> sortedHighscores;
    public List<int> playerIDs;
    public List<int> sortedPlayerIDs;

    // Use this for initialization
    void Start () {
        highscores = new List<int>();
        sortedHighscores = new List<int>();
        playerIDs = new List<int>();
        sortedPlayerIDs = new List<int>();
        firstRound = true;
    }


    public void sort()
    {
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

            for (int j = i + 1; j < sortedHighscores.Count; j++)
            {
                if (sortedHighscores[highestIndex] < sortedHighscores[j])
                {
                    highestIndex = j;
                }
            }

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

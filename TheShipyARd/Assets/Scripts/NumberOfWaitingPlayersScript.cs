﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class NumberOfWaitingPlayersScript : MonoBehaviour {

    public Text numberOfPlayersText;

    private Text playerText;

    // Use this for initialization
    void Start()
    {
        numberOfPlayersText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!NetworkManager.isConnected)
        {
            numberOfPlayersText.text = "Connecting to server...";
        }
        else
        {
            int missingNoPlayers = NetworkManager.expectedNumberOfPlayers - NetworkManager.currentNumberOfPlayers;

            string playerString = "players";

            if (missingNoPlayers == 1)
                playerString = "player";

            numberOfPlayersText.text = "Connection established" + "\n \n \n" +
                    "Waiting for " + missingNoPlayers.ToString() + " additional " + playerString + "\n \n" +
                    "Current number of players: " + NetworkManager.currentNumberOfPlayers.ToString();
        }
    }
}
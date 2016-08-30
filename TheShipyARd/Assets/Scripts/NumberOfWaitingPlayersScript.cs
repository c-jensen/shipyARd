using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//This Scripts represents the lobby screen. It shows the current number of waiting players and informs 
//a user about how many people still need to join for the game to begin

public class NumberOfWaitingPlayersScript : MonoBehaviour
{

    public Text numberOfPlayersText;
    private Text playerText;

    void Start()
    {
        numberOfPlayersText = GetComponent<Text>();
    }

    void Update()
    {
        //while not connected, write Connecting to server text
        if (!NetworkManager.isConnected)
        {
            numberOfPlayersText.text = "Connecting to server...";
        }
        else
        {
            //if already connected, write current number of players and how many people still are missing
            int missingNoPlayers = NetworkManager.expectedNumberOfPlayers - NetworkManager.currentNumberOfPlayers;

            //fancy singular / plural hack
            string playerString = "players";

            if (missingNoPlayers == 1)
                playerString = "player";

            numberOfPlayersText.text = "Connection established" + "\n \n \n" +
                    "Waiting for " + missingNoPlayers.ToString() + " additional " + playerString + "\n \n" +
                    "Current number of players: " + NetworkManager.currentNumberOfPlayers.ToString();
        }
    }
}

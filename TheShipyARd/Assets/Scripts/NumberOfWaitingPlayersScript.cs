using UnityEngine;
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

        if (!RandomMatchMaker.isConnected)
        {
            numberOfPlayersText.text = "Connecting to server...";
        }
        else
        {
            int missingNoPlayers = RandomMatchMaker.MINIMUM_NUMBER_OF_PLAYERS - RandomMatchMaker.currentNumberOfPlayers;

            string playerString = "players";

            if (missingNoPlayers == 1)
                playerString = "player";

            numberOfPlayersText.text = "Connection established" + "\n \n \n" +
                    "Waiting for " + missingNoPlayers.ToString() + " additional " + playerString + "\n \n" +
                    "Current number of players: " + RandomMatchMaker.currentNumberOfPlayers.ToString();
        }
    }
}

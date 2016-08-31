using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This script updates the Slider which displays the expected number of players in the start menu
public class NumOfPlayersScript : MonoBehaviour
{
    //This script updates the number of players script in the start menu

    public Slider slider;
    public Text textBox;

    void Update()
    {
        NetworkManager.expectedNumberOfPlayers = (int)slider.value;
        textBox.text = NetworkManager.expectedNumberOfPlayers.ToString();
    }
}

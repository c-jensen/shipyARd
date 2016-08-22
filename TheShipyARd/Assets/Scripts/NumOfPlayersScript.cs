using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumOfPlayersScript : MonoBehaviour {

    public Slider slider;
    public Text textBox;

    // Update is called once per frame
    void Update () {
        NetworkManager.expectedNumberOfPlayers = (int) slider.value;
        textBox.text = NetworkManager.expectedNumberOfPlayers.ToString();
    }
}

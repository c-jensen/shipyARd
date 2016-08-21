using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumOfPlayersScript : MonoBehaviour {

    public Slider slider;
    public Text textBox;

    // Use this for initialization
    void Start ()
    {
        
    }

    // Update is called once per frame
    void Update () {
        NetworkManager.expectedNumberOfPlayers = (int) slider.value;
        textBox.text = NetworkManager.expectedNumberOfPlayers.ToString();
    }
}

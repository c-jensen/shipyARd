using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NumOfPlayersScript : MonoBehaviour {

    private Slider slider;
    private Text textBox;

    // Use this for initialization
    void Start () {
        

        GameObject sliderGO = GameObject.Find("NumOfPlayersSlider");
        GameObject textBoxGO = GameObject.Find("NumOfPlayersIntDisplay");


        if (sliderGO != null)
        {
            slider = sliderGO.GetComponent<Slider>();
            slider.value = 2;
        }

        if (textBoxGO != null)
        {
            textBox = textBoxGO.GetComponent<Text>();
            textBox.text = "2";
        }
        
    }

    // Update is called once per frame
    void Update () {
        NetworkManager.expectedNumberOfPlayers = (int) slider.value;
        textBox.text = NetworkManager.expectedNumberOfPlayers.ToString();
    }
}

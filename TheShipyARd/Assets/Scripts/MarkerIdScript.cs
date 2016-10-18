using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MarkerIdScript : MonoBehaviour
{
    //This Script changes the NEW GAME button at the start menu to green and enables it, 
    //as soon as the player choose his marker id 

    public Text newGameButtonText;
    public Button newGameButton;
    public Slider slider;
    public Text textBox;

    void Update()
    {
        //slider gets initialized with -1 as value
        //once slider has been moved, the value is not -1 anymore
        if (slider.value != -1)
        {

            //change min value of slider to 0, so that player cant take any value below 0 anymore
            slider.minValue = 1;

            //Change marker ID in text box next to slider
            int finalMarkerID = (int)slider.value;
            PlayerScript.markerID = finalMarkerID - 1;
            textBox.text = finalMarkerID.ToString();

            //change new game button to be interactable and to have the green new game font
            newGameButtonText.text = "New Game";
            newGameButton.interactable = true;
            newGameButtonText.color = Color.green;
        }
    }
}

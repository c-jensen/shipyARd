using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MarkerIdScript : MonoBehaviour
{
    public Text newGameButtonText;
    public Button newGameButton;
    public Slider slider;
    public Text textBox;

    // Update is called once per frame
    void Update()
    {
        if (slider.value != -1)
        {
            slider.minValue = 0;

            PlayerScript.markerID = (int)slider.value;
            textBox.text = PlayerScript.markerID.ToString();

            newGameButtonText.text = "New Game";
            newGameButton.interactable = true;
            newGameButtonText.color = Color.green;
        }
    }
}

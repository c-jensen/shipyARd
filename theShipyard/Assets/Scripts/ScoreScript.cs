using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//This script sets the color of the score text in the HUD depending on if its positive, negative or neutral
public class ScoreScript : MonoBehaviour
{

    public Text scoreText; //Reference to our scorevalue

    public void updateScoreValue(int score)
    {
        if (score == 0)
            scoreText.color = Color.white;
        else if (score < 0)
            scoreText.color = Color.red;
        else if (score > 0)
            scoreText.color = Color.green;

        scoreText.text = score.ToString();
    }

}

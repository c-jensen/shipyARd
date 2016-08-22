using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreScript : MonoBehaviour {

    public Text scoreText; //Reference to our scorevalue

    public void updateScoreValue(int score)
    {
        if (score == 0)
            scoreText.color = Color.white;
        else if(score < 0)
            scoreText.color = Color.red;
        else if (score > 0)
            scoreText.color = Color.green;

        scoreText.text = score.ToString();
    }

}

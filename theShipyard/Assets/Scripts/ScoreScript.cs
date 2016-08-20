using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreScript : MonoBehaviour {

    public Text scoreText; //Reference to our scorevalue

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    public void updateScoreValue(int score)
    {
        Debug.LogError("ScoreText set to " + score);
        if (score == 0)
            scoreText.color = Color.white;
        else if(score < 0)
            scoreText.color = Color.red;
        else if (score > 0)
            scoreText.color = Color.green;

        scoreText.text = score.ToString();
    }

}

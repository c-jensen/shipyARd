using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using UnityEngine.EventSystems;


public class HudScript : MonoBehaviour {

    PlayerScript playerScript = null;

    public Button attackButton;
    public Sprite buttonDown;
    public Sprite buttonUp;

    // Use this for initialization
    void Start ()
    {
        GameObject player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerScript>();
    }

    public void attackButtonClicked()
    {
        attackButton.image.sprite = buttonDown;

        if (playerScript == null)
        {
            GameObject player = GameObject.Find("Player");
            playerScript = player.GetComponent<PlayerScript>();
        }

        playerScript.playerFunctions.attack();
        
    }

    public void attackButtonReleased()
    {
        attackButton.image.sprite = buttonUp;
    }
}

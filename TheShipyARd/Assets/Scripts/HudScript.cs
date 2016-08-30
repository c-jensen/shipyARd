﻿using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using UnityEngine.EventSystems;


public class HudScript : MonoBehaviour
{

    PlayerScript playerScript = null;

    public Button attackButton;
    public Sprite buttonDown;
    public Sprite buttonUp;

    void Start()
    {
        //save the player script to access the player functions
        GameObject player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerScript>();
    }

    public void attackButtonClicked()
    {

        //change button image to the button down version
        attackButton.image.sprite = buttonDown;

        //just a safety call, in case start was not executed yet
        if (playerScript == null)
        {
            GameObject player = GameObject.Find("Player");
            playerScript = player.GetComponent<PlayerScript>();
        }

        //call the interaction method in player functions
        playerScript.playerFunctions.attack();

    }

    public void attackButtonReleased()
    {
        //if button is released, change image again to the button up version
        attackButton.image.sprite = buttonUp;
    }
}

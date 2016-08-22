using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class HudScript : MonoBehaviour {

    PlayerScript playerScript = null;

    // Use this for initialization
    void Start ()
    {
        GameObject player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerScript>();
    }

    public void attackButtonClicked()
    {        
        if (playerScript == null)
        {
            GameObject player = GameObject.Find("Player");
            playerScript = player.GetComponent<PlayerScript>();
        }

        playerScript.playerFunctions.attack();
        
    }
}

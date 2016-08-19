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
        Debug.LogError("BUTTON STARTED");

    }

    // Update is called once per frame
    void Update ()
    {
    }

    public void attackButtonClicked()
    {
        Debug.LogError("BUTTON CLICKED");
        
        if (playerScript == null)
        {
            GameObject player = GameObject.Find("Player");
            playerScript = player.GetComponent<PlayerScript>();
        }

        playerScript.attack();
        
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerIDScript : MonoBehaviour {

    public Text idText; //Reference to our playerID value

    // Use this for initialization
    public void postPlayerID (int id)
    {
        idText.text = id.ToString();
    }

}

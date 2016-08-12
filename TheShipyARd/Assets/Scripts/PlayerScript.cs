using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Target
{
    TARGET_A,
    TARGET_B,
    TARGET_C,
    TARGET_D,
    TARGET_E,
    TARGET_F,
    TARGET_G,
    TARGET_H,
    TARGET_I,
    TARGET_J,
    MAX_NUM_OF_TARGETS,
    UNKNOWN

}

public enum Tool
{
    HANDCUFF,
    CABLE_TIE,
    ROPE,
    MAX_NUM_OF_TOOLS,
    NONE
}

public class PlayerScript : MonoBehaviour {

    //List containing all unused targets
    public static List<Target> availableTargets = new List<Target>();
    //List containing all unused player IDs
    public static List<Target> availablePlayers = new List<Target>();

    private GameObject player;

    public float health = 100f;
    public int score = 0;

    public bool playerDead = false;
    public Target targetPlayer = Target.UNKNOWN;
    public Target playerID = Target.UNKNOWN;
    public Tool activeTool = Tool.NONE;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");

        if (PhotonNetwork.isMasterClient == true)
        {
            Debug.LogError("We now generate the list");
            generatePlayerAndTargetList();
        }

        Debug.LogError("We now run requestTargetAndPlayer");
        requestTargetAndPlayer();        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f)
        {
            // Player currently dying
            if (!playerDead)
            {
                playerDying();
            }
            // Player is already dead
            else
            {
                playerIsDead();
            }
        }

    }

    void playerDying()
    {
        playerDead = true;
    }

    void playerIsDead()
    {

    }

    public void takeDamage(float amount)
    {
        health -= amount;
    }

    public void generatePlayerAndTargetList()
    {
        //Add all target to a list
        for (int i = 0; i < (int)Target.MAX_NUM_OF_TARGETS; i++)
        {
            availableTargets.Add((Target)i);
        }
        //Shuffle list
        for (int i = 0; i < availableTargets.Count; i++)
        {
            Target temp = availableTargets[i];
            int randomIndex = Random.Range(i, availableTargets.Count);
            availableTargets[i] = availableTargets[randomIndex];
            availableTargets[randomIndex] = temp;
        }
        //Remove enough elements so that list size matchtes number of players
        for (int i = 0; i < availableTargets.Count - NetworkManager.expectedNumberOfPlayers; i++)
        {
            availableTargets.RemoveAt(i);
        }

        availablePlayers = availableTargets;
        availablePlayers.Reverse();
        //In case of an odd number of players the middle element gets switched
        if (availablePlayers.Count % 2 == 1)
        {
            int switchIndex = (int)Mathf.Floor(availablePlayers.Count / 2.0f);
            Target tmp = availablePlayers[0];
            availablePlayers[0] = availablePlayers[switchIndex];
            availablePlayers[switchIndex] = tmp;
        }
    }

    public void requestTargetAndPlayer()
    {
        Debug.LogError("requestTargetAndPlayer");

        player.GetComponent<PhotonView>().RPC("rpc_sendTargetAndPlayerToClient", PhotonTargets.MasterClient, PhotonNetwork.player.ID);

        //player.GetComponent<PhotonView>().RPC("rpc_sendTargetAndPlayerToClient", PhotonTargets.MasterClient);
    }

    [PunRPC]
    public void rpc_sendTargetAndPlayerToClient(int id)
    {
        Debug.LogError("rpc_sendTargetAndPlayerToClient");

        if (PhotonNetwork.isMasterClient && availableTargets.Count >= 1 && availablePlayers.Count >= 1)
        {
            Debug.LogError(id);

            //send the target to the client
            int clientTarget = (int) availableTargets[0];
            availableTargets.RemoveAt(0);
            Debug.LogError(string.Format("I am here a"));
            player.GetComponent<PhotonView>().RPC("rpc_receiveTarget", PhotonPlayer.Find(id), clientTarget);

            //send the player ID to the client
            int clientPlayer = (int) availablePlayers[0];
            availablePlayers.RemoveAt(0);
            Debug.LogError(string.Format("I am here b"));

            player.GetComponent<PhotonView>().RPC("rpc_receivePlayer", PhotonPlayer.Find(id), clientPlayer);
        }
    }

    [PunRPC]
    public void rpc_receiveTarget(int target, PhotonMessageInfo info)
    {
        //Debug.LogError(string.Format("Info rpc_receiveTarget: {0} {1} {2}", info.sender, info.photonView, info.timestamp));

        Debug.LogError("receiveTarget");
        targetPlayer = (Target) target;
    }

    [PunRPC]
    public void rpc_receivePlayer(int player, PhotonMessageInfo info)
    {
        //Debug.LogError(string.Format("Info rpc_receivePlayer: {0} {1} {2}", info.sender, info.photonView, info.timestamp));

        Debug.LogError("receivePlayer");
        playerID = (Target) player;

        GameObject go = new GameObject();
        go.AddComponent<GUIText>();

        GUIText guiText = go.GetComponent<GUIText>();
        go.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
        guiText.text = "Target NR: " + (int)targetPlayer + " Player ID: " + (int)playerID;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}

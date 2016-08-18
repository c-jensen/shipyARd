using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public enum Target
{
    TARGET_A,
    TARGET_B,
    TARGET_C,
    /*
    TARGET_D,
    TARGET_E,
    TARGET_F,
    TARGET_G,
    TARGET_H,
    TARGET_I,
    TARGET_J,
    */
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
    private TargetScript targetImage;

    public float health = 100f;
    public int score = 0;

    public bool playerDead = false;
    public Target targetPlayer = Target.UNKNOWN;
    public Target playerID = Target.UNKNOWN;
    public Tool activeTool = Tool.NONE;

    IEnumerator wait()
    {
        yield return new WaitForSeconds(15.0f);
    }

    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("TargetImageGUI");
        Image targetComponent = go.GetComponent<Image>();
        targetImage = targetComponent.GetComponent<TargetScript>();
        player = GameObject.Find("Player");

        if (PhotonNetwork.isMasterClient == false)
            StartCoroutine(wait());

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
        for (int i = 0; i < NetworkManager.expectedNumberOfPlayers; i++)
        {
            availableTargets.Add((Target)i);
            availablePlayers.Add((Target)i);
        }
        
        //Shuffle lists
        for (int i = 0; i < availableTargets.Count; i++)
        {
            Target temp = availableTargets[i];
            int randomIndex = Random.Range(i, availableTargets.Count-1);
            availableTargets[i] = availableTargets[randomIndex];
            availableTargets[randomIndex] = temp;
        }

        //copy list
        for (int i = 0; i < availableTargets.Count; i++)
        {
            availablePlayers[i] = availableTargets[i];
        }

        availablePlayers.Reverse();

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
            //send the target and player to the client
            int clientTarget = (int)availableTargets[0];
            int clientPlayer = (int)availablePlayers[0];

            if (clientTarget == clientPlayer)
            {
                if (availablePlayers.Count >= 2)
                {
                    clientPlayer = (int)availablePlayers[1];
                    availableTargets.RemoveAt(0);
                    availablePlayers.RemoveAt(1);
                }
            }
            else
            {
                availableTargets.RemoveAt(0);
                availablePlayers.RemoveAt(0);
            }


            player.GetComponent<PhotonView>().RPC("rpc_receiveTarget", PhotonPlayer.Find(id), clientTarget);
            player.GetComponent<PhotonView>().RPC("rpc_receivePlayer", PhotonPlayer.Find(id), clientPlayer);
        }
    }

    [PunRPC]
    public void rpc_receiveTarget(int target, PhotonMessageInfo info)
    {
        //Debug.LogError(string.Format("Info rpc_receiveTarget: {0} {1} {2}", info.sender, info.photonView, info.timestamp));

        Debug.LogError("receiveTarget");
        targetPlayer = (Target) target;

        if (target == 0)
            targetImage.setImage0();
        else if (target == 1)
            targetImage.setImage1();
        else if (target == 2)
            targetImage.setImage2();
        else if (target == 3)
            targetImage.setImage3();
        else if (target == 4)
            targetImage.setImage4();
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

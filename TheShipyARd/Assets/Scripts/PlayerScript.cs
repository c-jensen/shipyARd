using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public enum Target
{
    TARGET_0,
    TARGET_1,
    TARGET_2,
    /*
    TARGET_3,
    TARGET_4,
    TARGET_5,
    TARGET_6,
    TARGET_7,
    TARGET_8,
    TARGET_9,
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

    public Sprite playerImage0;
    public Sprite playerImage1;
    public Sprite playerImage2;
    public Sprite playerImage3;
    public Sprite playerImage4;
    public Sprite playerImage5;
    public Sprite playerImage6;
    public Sprite playerImage7;
    public Sprite playerImage8;
    public Sprite playerImage9;
    public Sprite playerDeadImage;

    private Hashtable markerToPlayer;
    private Hashtable markerToPhotonID;

    private HealthSliderScript GUIHealthSlider;
    private ScoreScript GUIScoreText;

    GameObject defeatedHUD;

    private int markerID;
    private GameObject plane;

    private Target trackedTarget;
   
    private int markerPlayerCounter = 0;

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
        GameObject scoreText = GameObject.Find("ScoreNumberGUI");
        GUIScoreText = scoreText.GetComponent<ScoreScript>();

        GameObject healthSlider = GameObject.Find("HealthSlider");
        GUIHealthSlider = healthSlider.GetComponent<HealthSliderScript>();

        defeatedHUD = GameObject.Find("HUDCanvasDefeatedGUI");
        defeatedHUD.SetActive(false);

        GUIScoreText.updateScoreValue(score);

        markerToPlayer = new Hashtable();
        markerToPhotonID = new Hashtable();
        markerID = PhotonNetwork.player.ID - 1;
        GameObject go = GameObject.Find("TargetImageGUI");
        Image targetComponent = go.GetComponent<Image>();
        targetImage = targetComponent.GetComponent<TargetScript>();
        player = GameObject.Find("Player");

        //trackedTarget = Target.UNKNOWN;

        if (PhotonNetwork.isMasterClient == false)
            StartCoroutine(wait());

        if (PhotonNetwork.isMasterClient == true)
        {
            generatePlayerAndTargetList();
        }

        requestTargetAndPlayer();

        
    }

    public void attack()
    {
        if (!playerDead)
        {
            int cast_trackedTarget = (int)trackedTarget;

            if (cast_trackedTarget != markerID)
            {
                int photonID = (int)markerToPhotonID[cast_trackedTarget];

                Debug.LogError("targetID ist " + photonID);
                player.GetComponent<PhotonView>().RPC("rpc_takeDamage", PhotonPlayer.Find(photonID), (int)playerID, 25.0f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void playerDying(int attackerID)
    {
        playerDead = true;
        player.GetComponent<PhotonView>().RPC("rpc_playerDied", PhotonTargets.Others, markerID, attackerID, (int)targetPlayer);
        Debug.LogError("Ich sterbe und der Mörder ist " + attackerID);
        GameObject go0 = GameObject.Find("HUDCanvasGUI");
        go0.SetActive(false);
        defeatedHUD.SetActive(true);
        plane = GameObject.Find("player_" + markerID);
        plane.GetComponent<Renderer>().material.mainTexture = playerDeadImage.texture;
    }

    [PunRPC]
    public void rpc_playerDied(int markerID, int attackerID, int hisTargetID)
    {
        Debug.LogError("Ich habe gehört, dass " + markerToPlayer[markerID] + " stirbt und sein angreifer war " + attackerID + " und meine targetID ist " + (int)targetPlayer);

        if ((int)markerToPlayer[markerID] == (int)targetPlayer)
        {
            if ((int)playerID == attackerID)
            {
                targetImage.setImageSuccessful();
                score++;
                GUIScoreText.updateScoreValue(score);
            }
            else
            {
                targetImage.setImageUnsuccessful();
            }
        }

        else if ((int)playerID == attackerID)
        {
            if (hisTargetID == (int)playerID)
                score += 2;
            else
                score--;
            GUIScoreText.updateScoreValue(score);
        }

        plane = GameObject.Find("player_" + markerID);
        plane.GetComponent<Renderer>().material.mainTexture = playerDeadImage.texture;
    }

    [PunRPC]
    public void rpc_takeDamage(int attackerID, float amount)
    {
        if (!playerDead)
        {
            health -= amount;
            GUIHealthSlider.updateValue(health);
            Debug.LogError("ICH VERLIERE LEBEN und habe noch: " + health);

            if (health <= 0)
            {
                playerDying(attackerID);
            }
        }
    }

    public void setTrackedTarget(int trackedEnemy)
    {
        trackedTarget = (Target)trackedEnemy;
        Debug.LogError("Tracked Target set on " + trackedEnemy + " " + trackedTarget);
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
        player.GetComponent<PhotonView>().RPC("rpc_sendTargetAndPlayerToClient", PhotonTargets.MasterClient, PhotonNetwork.player.ID, markerID);

        //player.GetComponent<PhotonView>().RPC("rpc_sendTargetAndPlayerToClient", PhotonTargets.MasterClient);
    }

    [PunRPC]
    public void rpc_sendTargetAndPlayerToClient(int id, int marker)
    {
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

            markerToPlayer[marker] = clientPlayer;
            markerToPhotonID[marker] = id;

            player.GetComponent<PhotonView>().RPC("rpc_receiveTarget", PhotonPlayer.Find(id), clientTarget);
            player.GetComponent<PhotonView>().RPC("rpc_receivePlayer", PhotonPlayer.Find(id), clientPlayer);

            markerPlayerCounter++;
        }

        if (markerPlayerCounter >= NetworkManager.expectedNumberOfPlayers)
        {
            for (int i = 0; i < markerToPlayer.Count; i++)
            {
                player.GetComponent<PhotonView>().RPC("rpc_receiveMarkerPlayerRelation", PhotonTargets.All, i, markerToPlayer[i], markerToPhotonID[i]);
            }

            //player.GetComponent<PhotonView>().RPC("printHash", PhotonTargets.All);

        }
    }

    /*
    [PunRPC]
    public void printHash()
    {
        for (int i = 0; i < markerToPlayer.Count; i++)
        {
            Debug.LogError("Marker: " + i);
            Debug.LogError("MarkerToPlayer: " + markerToPlayer[i]);
            Debug.LogError("MarkertoPhotonID " + markerToPhotonID[i]);
        }
    }
    */

    [PunRPC]
    public void rpc_receiveMarkerPlayerRelation(int markerID, int playersID, int photonID)
    {
        //Debug.LogError("RECEIVE MarkerID: " + markerID + " playerID: " + playersID);

        if(PhotonNetwork.isMasterClient == false)
        {
            markerToPlayer[markerID] = playersID;
            markerToPhotonID[markerID] = photonID;
        }

        plane = GameObject.Find("player_" + markerID);

        if(playersID == 0) 
            plane.GetComponent<Renderer>().material.mainTexture = playerImage0.texture;
        else if(playersID == 1)
            plane.GetComponent<Renderer>().material.mainTexture = playerImage1.texture;
        else if (playersID == 2)
            plane.GetComponent<Renderer>().material.mainTexture = playerImage2.texture;
        else if (playersID == 3)
            plane.GetComponent<Renderer>().material.mainTexture = playerImage3.texture;
        else if (playersID == 4)
            plane.GetComponent<Renderer>().material.mainTexture = playerImage4.texture;
        else if (playersID == 5)
            plane.GetComponent<Renderer>().material.mainTexture = playerImage5.texture;
        else if (playersID == 6)
            plane.GetComponent<Renderer>().material.mainTexture = playerImage6.texture;
        else if (playersID == 7)
            plane.GetComponent<Renderer>().material.mainTexture = playerImage7.texture;
        else if (playersID == 8)
            plane.GetComponent<Renderer>().material.mainTexture = playerImage8.texture;
        else if (playersID == 9)
            plane.GetComponent<Renderer>().material.mainTexture = playerImage9.texture;
    }

    [PunRPC]
    public void rpc_receiveTarget(int target, PhotonMessageInfo info)
    {
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

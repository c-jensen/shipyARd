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
    INJECTION,
    ROPE,
    MAX_NUM_OF_TOOLS,
    NONE
}

public class PlayerScript : MonoBehaviour {

    private const float HANDCUFF_DAMAGE = 20.0f;
    private const float INJECTION_DAMAGE = 50.0f;
    private const float ROPE_DAMAGE = 10.0f;

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
    public Sprite thumbsUp;
    public Sprite thumbsDown;

    public Sprite toolImageHandcuffs;
    public Sprite toolImageInjection;
    public Sprite toolImageRope;
    public Sprite toolImageLooted;

    private Hashtable markerToPlayer;
    private Hashtable markerToPhotonID;
    private Hashtable markerToTool;

    private HealthSliderScript GUIHealthSlider;
    private ScoreScript GUIScoreText;

    GameObject defeatedHUD;
    Image onHitHUD;
    Text infoTextHUD;

    Color onHitColor;
    Color infoTextColor;

    public static int markerID;
    private GameObject planePlayer;
    private GameObject planeTool;

    private Target trackedTarget;

    private int trackedToolMarker;
   
    private int markerPlayerCounter = 0;

    private GameObject player;
    private TargetScript targetImage;
    private ToolScript toolImage;

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
        trackedTarget = Target.UNKNOWN;
        trackedToolMarker = -1;

        GameObject scoreText = GameObject.Find("ScoreNumberGUI");
        GUIScoreText = scoreText.GetComponent<ScoreScript>();

        GameObject healthSlider = GameObject.Find("HealthSlider");
        GUIHealthSlider = healthSlider.GetComponent<HealthSliderScript>();

        defeatedHUD = GameObject.Find("HUDCanvasDefeatedGUI");
        defeatedHUD.SetActive(false);

        onHitHUD = GameObject.Find("OnHitEffectGUI").GetComponent<Image>();
        onHitColor = onHitHUD.color;
        onHitColor.a = 0.0f;
        onHitHUD.color = onHitColor;

        infoTextHUD = GameObject.Find("InfoTextBoxGUI").GetComponent<Text>();
        infoTextColor = infoTextHUD.color;
        infoTextColor.a = 0.0f;
        infoTextHUD.color = infoTextColor;

        GUIScoreText.updateScoreValue(score);

        markerToPlayer = new Hashtable();
        markerToPhotonID = new Hashtable();
        markerToTool = new Hashtable();

        //markerID = PhotonNetwork.player.ID - 1;

        GameObject go = GameObject.Find("TargetImageGUI");
        Image targetComponent = go.GetComponent<Image>();
        targetImage = targetComponent.GetComponent<TargetScript>();

        GameObject go1 = GameObject.Find("ToolImageGUI");
        Image toolComponent = go1.GetComponent<Image>();
        toolImage = toolComponent.GetComponent<ToolScript>();

        player = GameObject.Find("Player");

        if (PhotonNetwork.isMasterClient == false)
            StartCoroutine(wait());

        if (PhotonNetwork.isMasterClient == true)
        {
            generateToolDistribution();
            generatePlayerAndTargetList();
        }

        requestTargetAndPlayer();

        activeTool = Tool.NONE;
    }

    public void attack()
    {
        if (!playerDead)
        {
            Debug.LogError("ToolDebug: trackedToolMarker is: " + trackedToolMarker);

            int cast_trackedTarget = (int)trackedTarget;                       

            if (cast_trackedTarget != markerID && (cast_trackedTarget != (int)Target.UNKNOWN) && (activeTool != Tool.NONE))
            {
                int photonID = (int)markerToPhotonID[cast_trackedTarget];

                Debug.LogError("targetID ist " + photonID);
                float finalDamage = 0.0f;
                if (activeTool == Tool.HANDCUFF)
                    finalDamage = HANDCUFF_DAMAGE;
                else if (activeTool == Tool.INJECTION)
                    finalDamage = INJECTION_DAMAGE;
                else if (activeTool == Tool.ROPE)
                    finalDamage = ROPE_DAMAGE;
                player.GetComponent<PhotonView>().RPC("rpc_takeDamage", PhotonPlayer.Find(photonID), (int)playerID, finalDamage);
            }
            else if (trackedToolMarker != -1 && (Tool)markerToTool[trackedToolMarker] != Tool.NONE)
            {
                Debug.LogError("ToolDebug: Trackedtool marker: " + trackedToolMarker);
                Debug.LogError("ToolDebug: active tool before pickup is: " + activeTool);

                Debug.LogError("I am picking up the tool: " + (Tool)markerToTool[trackedToolMarker]);
                player.GetComponent<PhotonView>().RPC("rpc_changeToolMarker", PhotonTargets.Others, (int)activeTool, trackedToolMarker);
                Tool tmp = (Tool)markerToTool[trackedToolMarker];
                changeToolMarker((int)activeTool, trackedToolMarker);
                activeTool = tmp;
                toolImage.setImage((int)activeTool);
                Debug.LogError("ToolDebug: active tool now is: " + activeTool);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (onHitColor.a > 0.0f)
        {
            onHitColor.a -= 0.02f;
            onHitHUD.color = onHitColor;
        }
        if (infoTextColor.a > 0.0f)
        {
            infoTextColor.a -= 0.0075f;
            infoTextHUD.color = infoTextColor;
        }
    }

    public void generateToolDistribution()
    {
        for (int i = 0; i < (int)Target.MAX_NUM_OF_TARGETS; i++)
        {
            markerToTool[i] = (Tool)(i%(int)Tool.MAX_NUM_OF_TOOLS);
        }
    }

    void playerDying(int attackerID)
    {
        playerDead = true;
        player.GetComponent<PhotonView>().RPC("rpc_playerDied", PhotonTargets.Others, markerID, attackerID, (int)targetPlayer);
        Debug.LogError("Ich sterbe und der Mörder ist " + attackerID);
        GameObject go0 = GameObject.Find("HUDCanvasGUI");
        go0.SetActive(false);
        defeatedHUD.SetActive(true);
        planePlayer = GameObject.Find("player_" + markerID);
        planePlayer.GetComponent<Renderer>().material.mainTexture = playerDeadImage.texture;
    }

    [PunRPC]
    public void rpc_playerDied(int markerID, int attackerID, int hisTargetID)
    {
        Debug.LogError("Ich habe gehört, dass " + markerToPlayer[markerID] + " stirbt und sein angreifer war " + attackerID + " und meine targetID ist " + (int)targetPlayer);

        planePlayer = GameObject.Find("player_" + markerID);

        if ((int)markerToPlayer[markerID] == (int)targetPlayer)
        {
            if ((int)playerID == attackerID)
            {
                targetImage.setImageSuccessful();
                score++;
                GUIScoreText.updateScoreValue(score);
                planePlayer.GetComponent<Renderer>().material.mainTexture = thumbsUp.texture;
                infoTextHUD.text = "You arrested your target!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }
            else
            {
                targetImage.setImageUnsuccessful();
                planePlayer.GetComponent<Renderer>().material.mainTexture = playerDeadImage.texture;
                infoTextHUD.text = "Someone else arrested your target!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }
        }

        else if ((int)playerID == attackerID)
        {
            if (hisTargetID == (int)playerID)
            {
                score += 2;
                planePlayer.GetComponent<Renderer>().material.mainTexture = thumbsUp.texture;
                infoTextHUD.text = "You arrested your pursuer!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }
            else
            {
                score--;
                planePlayer.GetComponent<Renderer>().material.mainTexture = thumbsDown.texture;
                infoTextHUD.text = "You arrested the wrong target!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }
            GUIScoreText.updateScoreValue(score);
        }
        else
        {
            planePlayer.GetComponent<Renderer>().material.mainTexture = playerDeadImage.texture;
            infoTextHUD.text = "Someone in the game was arrested!";
            infoTextColor.a = 1.0f;
            infoTextHUD.color = infoTextColor;
        }
    }

    [PunRPC]
    public void rpc_takeDamage(int attackerID, float amount)
    {
        if (!playerDead)
        {
            onHitColor.a = 1.0f;
            onHitHUD.color = onHitColor;

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

    public void setTrackedToolMarker(int trackedTool)
    {
        trackedToolMarker = trackedTool;
        Debug.LogError("Tracked Tool set on " + trackedTool + " " + trackedToolMarker);
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
                player.GetComponent<PhotonView>().RPC("rpc_receiveMarkerPlayerRelation", PhotonTargets.All, i, (int)markerToPlayer[i], (int)markerToPhotonID[i]);
            }
            for (int i = 0; i < markerToTool.Count; i++)
            {
                player.GetComponent<PhotonView>().RPC("rpc_receiveMarkerToolRelation", PhotonTargets.All, i, (int)markerToTool[i]);
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
    public void rpc_changeToolMarker(int toolID, int marker)
    {
        changeToolMarker(toolID, marker);
    }

    public void changeToolMarker(int toolID, int marker)
    {
        planeTool = GameObject.Find("tool_" + marker);

        markerToTool[marker] = (Tool)toolID;
        if (toolID == (int)Tool.HANDCUFF)
            planeTool.GetComponent<Renderer>().material.mainTexture = toolImageHandcuffs.texture;
        else if (toolID == (int)Tool.INJECTION)
            planeTool.GetComponent<Renderer>().material.mainTexture = toolImageInjection.texture;
        else if (toolID == (int)Tool.ROPE)
            planeTool.GetComponent<Renderer>().material.mainTexture = toolImageRope.texture;
        else if (toolID == (int)Tool.NONE)
            planeTool.GetComponent<Renderer>().material.mainTexture = toolImageLooted.texture;
    }

    [PunRPC]
    public void rpc_receiveMarkerPlayerRelation(int markerID, int playersID, int photonID)
    {
        //Debug.LogError("RECEIVE MarkerID: " + markerID + " playerID: " + playersID);

        if(PhotonNetwork.isMasterClient == false)
        {
            markerToPlayer[markerID] = playersID;
            markerToPhotonID[markerID] = photonID;
        }

        planePlayer = GameObject.Find("player_" + markerID);

        if(playersID == 0)
            planePlayer.GetComponent<Renderer>().material.mainTexture = playerImage0.texture;
        else if(playersID == 1)
            planePlayer.GetComponent<Renderer>().material.mainTexture = playerImage1.texture;
        else if (playersID == 2)
            planePlayer.GetComponent<Renderer>().material.mainTexture = playerImage2.texture;
        else if (playersID == 3)
            planePlayer.GetComponent<Renderer>().material.mainTexture = playerImage3.texture;
        else if (playersID == 4)
            planePlayer.GetComponent<Renderer>().material.mainTexture = playerImage4.texture;
        else if (playersID == 5)
            planePlayer.GetComponent<Renderer>().material.mainTexture = playerImage5.texture;
        else if (playersID == 6)
            planePlayer.GetComponent<Renderer>().material.mainTexture = playerImage6.texture;
        else if (playersID == 7)
            planePlayer.GetComponent<Renderer>().material.mainTexture = playerImage7.texture;
        else if (playersID == 8)
            planePlayer.GetComponent<Renderer>().material.mainTexture = playerImage8.texture;
        else if (playersID == 9)
            planePlayer.GetComponent<Renderer>().material.mainTexture = playerImage9.texture;
    }

    [PunRPC]
    public void rpc_receiveMarkerToolRelation(int markerID, int toolID)
    {
        //Debug.LogError("RECEIVE MarkerID: " + markerID + " playerID: " + playersID);

        if (PhotonNetwork.isMasterClient == false)
        {
            markerToTool[markerID] = toolID;
        }

        planeTool = GameObject.Find("tool_" + markerID);

        if (toolID == 0)
            planeTool.GetComponent<Renderer>().material.mainTexture = toolImageHandcuffs.texture;
        else if (toolID == 1)
            planeTool.GetComponent<Renderer>().material.mainTexture = toolImageInjection.texture;
        else if (toolID == 2)
            planeTool.GetComponent<Renderer>().material.mainTexture = toolImageRope.texture;
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

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public enum Target
{
    TARGET_0,
    TARGET_1,
    TARGET_2,
    TARGET_3,
    TARGET_4,
    TARGET_5,
    TARGET_6,
    TARGET_7,
    TARGET_8,
    TARGET_9,
    MAX_NUM_OF_TARGETS,
    UNKNOWN

}

public class PlayerScript : MonoBehaviour {

    //List containing all unused targets
    public static List<Target> availableTargets = new List<Target>();
    //List containing all unused player IDs
    public static List<Target> availablePlayers = new List<Target>();
    //List containing all players not arrested
    public static List<Target> activePlayers = new List<Target>();
    //List containing all players targets not arrested
    public static List<Target> activePlayersTargets = new List<Target>();

    public MarkerDistributionScript markerDistribution;
    public PlayerFunctionsScript playerFunctions;

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

    public Sprite toolImageHandcuffs;
    public Sprite toolImageInjection;
    public Sprite toolImageRope;
    public Sprite toolImageLooted;

    public PlayerToolScript playerTool;

    public HealthSliderScript GUIHealthSlider;
    public ScoreScript GUIScoreText;

    public GameObject defeatedHUD;
    public GameObject gameFinishedHUD;
    public GameObject highscoreHUD;

    Text gameFinishedText;
    Image gameFinishedBackground;

    public Image onHitHUD;
    public Text infoTextHUD;

    public Color onHitColor;
    public Color infoTextColor;

    public Color gameFinishedBackgroundColor;
    public Color gameFinishedTextColor;

    public static int markerID;

    public GameObject planePlayer;
    public GameObject planeTool;

    public Target trackedTarget;

    public int trackedToolMarker;
   
    public int markerPlayerCounter = 0;

    public GameObject player;
    public TargetScript targetImage;
    public ToolScript toolImage;

    public float health = 100f;
    public int score = 0;
    private bool gameStopped = false;

    public bool playerDead = false;
    public Target targetPlayer = Target.UNKNOWN;
    public Target playerID = Target.UNKNOWN;

    public int allReady = 0;
    public bool masterReceived = false;
    public bool incrementDone = false;
    public bool targetsDistributed = false;

    void Awake()
    {
        playerFunctions = new PlayerFunctionsScript(this);

        markerDistribution = new MarkerDistributionScript();

        trackedTarget = Target.UNKNOWN;
        trackedToolMarker = -1;

        playerTool = new PlayerToolScript(Tool.NONE);
    }

    // Use this for initialization
    void Start()
    {
        GameObject ready = GameObject.Find("RenderingSceneScripts");
        PlayerReadyScript playerReady = ready.GetComponent<PlayerReadyScript>();
        playerReady.ready = true;

        GameObject scoreText = GameObject.Find("ScoreNumberGUI");
        GUIScoreText = scoreText.GetComponent<ScoreScript>();

        GameObject healthSlider = GameObject.Find("HealthSlider");
        GUIHealthSlider = healthSlider.GetComponent<HealthSliderScript>();

        defeatedHUD = GameObject.Find("HUDCanvasDefeatedGUI");
        defeatedHUD.SetActive(false);

        gameFinishedHUD = GameObject.Find("HUDGameIsFinished");
        gameFinishedBackground = GameObject.Find("GameFinishedBackground").GetComponent<Image>();
        gameFinishedBackgroundColor = gameFinishedBackground.color;
        gameFinishedText = GameObject.Find("GameFinishedText").GetComponent<Text>();
        gameFinishedTextColor = gameFinishedText.color;
        gameFinishedHUD.SetActive(false);

        highscoreHUD = GameObject.Find("HUDHighscore");      
        highscoreHUD.SetActive(false);

        onHitHUD = GameObject.Find("OnHitEffectGUI").GetComponent<Image>();
        onHitColor = onHitHUD.color;
        onHitColor.a = 0.0f;
        onHitHUD.color = onHitColor;

        infoTextHUD = GameObject.Find("InfoTextBoxGUI").GetComponent<Text>();
        infoTextColor = infoTextHUD.color;
        infoTextColor.a = 0.0f;
        infoTextHUD.color = infoTextColor;

        GUIScoreText.updateScoreValue(score);

        GameObject go = GameObject.Find("TargetImageGUI");
        Image targetComponent = go.GetComponent<Image>();
        targetImage = targetComponent.GetComponent<TargetScript>();

        GameObject go1 = GameObject.Find("ToolImageGUI");
        Image toolComponent = go1.GetComponent<Image>();
        toolImage = toolComponent.GetComponent<ToolScript>();

        player = GameObject.Find("Player");

        if (PhotonNetwork.isMasterClient == true)
        {
            markerDistribution.generateToolDistribution();
            playerFunctions.generatePlayerAndTargetList();
            allReady++;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.isMasterClient == true)
        {
            if (allReady >= NetworkManager.expectedNumberOfPlayers && !targetsDistributed)
            {
                targetsDistributed = true;
                player.GetComponent<PhotonView>().RPC("rpc_masterIsReady", PhotonTargets.All);
            }
        }
        if (PhotonNetwork.isMasterClient == false)
        {
            if (!masterReceived)
                player.GetComponent<PhotonView>().RPC("rpc_playerIsReady", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
            else if(masterReceived && !incrementDone)
                {
                    incrementDone = true;
                    player.GetComponent<PhotonView>().RPC("rpc_incrementCounter", PhotonTargets.MasterClient);
                }
            }
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
        if (gameFinishedHUD.GetActive())
        {
            if (gameFinishedTextColor.a < 1.0f)
            {
                gameFinishedTextColor.a += 0.003f;
                gameFinishedText.color = gameFinishedTextColor;
            }
            if (gameFinishedBackgroundColor.a < 1.0f)
            {
                gameFinishedBackgroundColor.a += 0.003f;
                gameFinishedBackground.color = gameFinishedBackgroundColor;
            }
            if(gameFinishedBackgroundColor.a >= 1.0f)
            {
                gameFinishedHUD.SetActive(false);
                highscoreHUD.SetActive(true);
            }
        }
    }  

    public void setTrackedTarget(int trackedEnemy)
    {
        trackedTarget = (Target)trackedEnemy;
    }

    public void setTrackedToolMarker(int trackedTool)
    {
        trackedToolMarker = trackedTool;
    }

    public List<Target> getAvailableTargets()
    {
        return availableTargets;
    }

    public List<Target> getAvailablePlayers()
    {
        return availablePlayers;
    }

    public int getMarkerID()
    {
        return markerID;
    }

    public void checkIfGameFinished(Target hisPlayerID)
    {
        for (int i = 0; i < activePlayers.Count; i++)
        {
            if (activePlayers[i] == hisPlayerID)
            {
                activePlayers.RemoveAt(i);
                activePlayersTargets.RemoveAt(i);
                break;
            }
        }
        for (int i = 0; i < activePlayersTargets.Count; i++)
        {
            if (activePlayersTargets[i] == hisPlayerID)
            {
                activePlayersTargets[i] = Target.UNKNOWN;
            }
        }

        bool gameFinished = true;

        for (int i = 0; i < activePlayersTargets.Count; i++)
        {
            gameFinished &= (activePlayersTargets[i] == Target.UNKNOWN);
        }

        if (gameFinished && !gameStopped)
        {
            gameStopped = true;
            player.GetComponent<PhotonView>().RPC("rpc_gameIsOver", PhotonTargets.All);
        }
    }

    //This function is requiered though empty
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){}


    [PunRPC]
    public void rpc_gameIsOver()
    {
        if (!playerDead)
        {
            GameObject HUDCanvas = GameObject.Find("HUDCanvasGUI");
            HUDCanvas.SetActive(false);
        }
        else
        {
            defeatedHUD.SetActive(false);
        }

        gameFinishedHUD.SetActive(true);
    }

    //PUNRPC Section
    [PunRPC]
    public void rpc_incrementCounter()
    {
        allReady += 1;
    }

    [PunRPC]
    public void rpc_playerIsReady(int photonID)
    {
        player.GetComponent<PhotonView>().RPC("rpc_masterReceived", PhotonPlayer.Find(photonID));
    }

    [PunRPC]
    public void rpc_masterReceived()
    {
        masterReceived = true;
    }

    [PunRPC]
    public void rpc_masterIsReady()
    {
        playerFunctions.requestTargetAndPlayer();
    }

    [PunRPC]
    public void rpc_receiveMarkerPlayerRelation(int markerID, int playersID, int photonID)
    {
        if (PhotonNetwork.isMasterClient == false)
        {
            markerDistribution.setMarkerToPlayer(markerID, playersID);
            markerDistribution.setMarkerToPhotonID(markerID, photonID);
        }

        planePlayer = GameObject.Find("player_" + markerID);

        planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/player_" + playersID.ToString(), typeof(Texture2D)) as Texture2D;
    }

    [PunRPC]
    public void rpc_receiveMarkerToolRelation(int markerID, int toolID)
    {
        if (PhotonNetwork.isMasterClient == false)
        {
            markerDistribution.setMarkerToTool(markerID, (Tool)toolID);
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
        targetPlayer = (Target)target;

        targetImage.setImage("Players/player_" + target.ToString());
    }

    [PunRPC]
    public void rpc_receivePlayer(int player, PhotonMessageInfo info)
    {
        playerID = (Target)player;
    }

    [PunRPC]
    public void rpc_changeToolMarker(int toolID, int marker)
    {
        playerFunctions.changeToolMarker(toolID, marker);
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

            markerDistribution.setMarkerToPlayer(marker, clientPlayer);
            markerDistribution.setMarkerToPhotonID(marker, id);

            activePlayers.Add((Target)clientPlayer);
            activePlayersTargets.Add((Target)clientTarget);

            player.GetComponent<PhotonView>().RPC("rpc_receiveTarget", PhotonPlayer.Find(id), clientTarget);
            player.GetComponent<PhotonView>().RPC("rpc_receivePlayer", PhotonPlayer.Find(id), clientPlayer);

            markerPlayerCounter++;
        }

        if (markerPlayerCounter >= NetworkManager.expectedNumberOfPlayers)
        {
            for (int i = 0; i < markerDistribution.getMarkerToPlayerCount(); i++)
            {
                player.GetComponent<PhotonView>().RPC("rpc_receiveMarkerPlayerRelation", PhotonTargets.All, i, markerDistribution.getMarkerToPlayer(i), markerDistribution.getMarkerToPhotonID(i));
            }
            for (int i = 0; i < markerDistribution.getMarkerToToolCount(); i++)
            {
                player.GetComponent<PhotonView>().RPC("rpc_receiveMarkerToolRelation", PhotonTargets.All, i, (int)markerDistribution.getMarkerToTool(i));
            }
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

            if (health <= 0)
            {
                playerFunctions.playerDying(attackerID);
            }
        }
    }

    [PunRPC]
    public void rpc_playerDied(int markerID, int attackerID, int hisTargetID)
    {
        planePlayer = GameObject.Find("player_" + markerID);
        int hisPlayerID = markerDistribution.getMarkerToPlayer(markerID);

        //If arrested player was our target
        if (markerDistribution.getMarkerToPlayer(markerID) == (int)targetPlayer)
        {
            //If arrested player was our own target and arrested by us
            if ((int)playerID == attackerID)
            {
                targetImage.setImage("Players/success_arrested_player_" + hisPlayerID.ToString());
                score++;
                GUIScoreText.updateScoreValue(score);
                planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/success_arrested_player_" + hisPlayerID.ToString(), typeof(Texture2D)) as Texture2D;
                infoTextHUD.text = "You arrested your target!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }
            //If arrested player was our own target and was not arrested by us
            else
            {
                targetImage.setImage("Players/failed_arrested_player_" + hisPlayerID.ToString());
                planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/failed_arrested_player_" + hisPlayerID.ToString(), typeof(Texture2D)) as Texture2D;
                infoTextHUD.text = "Someone else arrested your target!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }
        }

        //If we were the attacker
        else if ((int)playerID == attackerID)
        {
            //If arrested player was our pursuer
            if (hisTargetID == (int)playerID)
            {
                score += 2;
                planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/success_arrested_player_" + hisPlayerID.ToString(), typeof(Texture2D)) as Texture2D;
                infoTextHUD.text = "You arrested your pursuer!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }
            //If the arrested player was either our target nor our pursuer
            else
            {
                score--;
                planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/failed_arrested_player_" + hisPlayerID.ToString(), typeof(Texture2D)) as Texture2D;
                infoTextHUD.text = "You arrested the wrong target!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }
            GUIScoreText.updateScoreValue(score);
        }
        //If we were not the attacker
        else
        {
            planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/arrested_player_" + hisPlayerID.ToString(), typeof(Texture2D)) as Texture2D;
            infoTextHUD.text = "Someone in the game was arrested!";
            infoTextColor.a = 1.0f;
            infoTextHUD.color = infoTextColor;
        }

        if (PhotonNetwork.isMasterClient == true)
        {
            checkIfGameFinished((Target)hisPlayerID);
        }
    }
}

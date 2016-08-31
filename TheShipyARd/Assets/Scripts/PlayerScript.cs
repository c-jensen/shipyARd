using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//Enum with player ids
public enum Player_ID
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
    public static List<Player_ID> availableTargets = new List<Player_ID>();
    
    //List containing all unused player IDs
    public static List<Player_ID> availablePlayers = new List<Player_ID>();
    
    //List containing all players not arrested
    public static List<Player_ID> activePlayers = new List<Player_ID>();
    
    //List containing all players targets not arrested
    public static List<Player_ID> activePlayersTargets = new List<Player_ID>();

    //List for the scores of the players sorted by player id
    private List<int> playerScores;

    //Variables for different Scripts that need to be accessed from here
    public MarkerDistributionScript markerDistribution;
    public PlayerFunctionsScript playerFunctions;
    public HighscoreScript highscoreScript;
    public PlayerToolScript playerTool;
    public JailSliderScript GUIJailSlider;
    public ScoreScript GUIScoreText;
    public TargetScript targetImage;
    public ToolScript toolImage;

    //Sprites for the differnt player faces
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

    //Sprites for different tool types
    public Sprite toolImageHandcuffs;
    public Sprite toolImageInjection;
    public Sprite toolImageRope;
    public Sprite toolImageLooted;

    //GameObjects for different HUD types
    public GameObject defeatedHUD;
    public GameObject gameFinishedHUD;
    public GameObject highscoreHUD;

    public Image onHitHUD;
    public Text infoTextHUD;
    public Color onHitColor;
    public Color infoTextColor;

    //Text and image which are displayed, when the game is finished
    Text gameFinishedText;
    Image gameFinishedBackground;

    public Color gameFinishedTextColor;
    public Color gameFinishedBackgroundColor;

    //MarkerID of current player
    public static int markerID;

    //Health and score of current player
    public float jailSliderValue = 100f;
    public int score = 0;
    public bool playerArrested = false;

    //Target of the current player
    public Player_ID targetPlayer = Player_ID.UNKNOWN;

    //id of current player
    public Player_ID playerID = Player_ID.UNKNOWN;

    //Target currently tracked (set to Target.UNKNOWN if nothing is tracked)
    public Player_ID trackedTarget;
    public int trackedToolMarker;

    public GameObject planePlayer;
    public GameObject planeTool;
    public GameObject player;

    public int markerPlayerCounter = 0;
    public int scoreRefreshed = 0;

    //Variables only used by master client
    public int allReady = 0;
    public bool masterReceived = false;
    public bool incrementDone = false;
    public bool targetsDistributed = false;
    private bool gameStopped = false;
    private bool terminateGame = false;

    void Awake()
    {
        //Create scrripts
        playerFunctions = new PlayerFunctionsScript(this);
        markerDistribution = new MarkerDistributionScript();
        playerTool = new PlayerToolScript(Tool.NONE);

        //set TrackedTarget and Tracked Tool to unknown and -1
        trackedTarget = Player_ID.UNKNOWN;
        trackedToolMarker = -1;

    }

    // Use this for initialization
    void Start()
    {
        //init player scores
        playerScores = new List<int>();

        for (int i = 0; i < NetworkManager.expectedNumberOfPlayers; i++)
        {
            playerScores.Add(0);
        }

        //Init / Get needed Game objects
        GameObject ready = GameObject.Find("RenderingSceneScripts");
        PlayerReadyScript playerReady = ready.GetComponent<PlayerReadyScript>();

        //save in player script, that current player is ready yet
        playerReady.ready = true;

        //create score text
        GameObject scoreText = GameObject.Find("ScoreNumberGUI");
        GUIScoreText = scoreText.GetComponent<ScoreScript>();

        //create health slider
        GameObject jailSlider = GameObject.Find("JailSlider");
        GUIJailSlider = jailSlider.GetComponent<JailSliderScript>();

        //Create defeated HUD
        defeatedHUD = GameObject.Find("HUDCanvasDefeatedGUI");
        defeatedHUD.SetActive(false);
        
        //create game finished hud
        gameFinishedHUD = GameObject.Find("HUDGameIsFinished");
        gameFinishedBackground = GameObject.Find("GameFinishedBackground").GetComponent<Image>();
        gameFinishedBackgroundColor = gameFinishedBackground.color;
        gameFinishedText = GameObject.Find("GameFinishedText").GetComponent<Text>();
        gameFinishedTextColor = gameFinishedText.color;
        gameFinishedHUD.SetActive(false);
        
        //Create highscore HUD
        highscoreHUD = GameObject.Find("HUDHighscore");      
        highscoreHUD.SetActive(false);

        //Create onHit Hud (red effect)
        onHitHUD = GameObject.Find("OnHitEffectGUI").GetComponent<Image>();
        onHitColor = onHitHUD.color;
        onHitColor.a = 0.0f;
        onHitHUD.color = onHitColor;

        //Create info text HUD 
        infoTextHUD = GameObject.Find("InfoTextBoxGUI").GetComponent<Text>();
        infoTextColor = infoTextHUD.color;
        infoTextColor.a = 0.0f;
        infoTextHUD.color = infoTextColor;

        GUIScoreText.updateScoreValue(score);

        //Create Target Image (right hand top corner)
        GameObject go = GameObject.Find("TargetImageGUI");
        Image targetComponent = go.GetComponent<Image>();
        targetImage = targetComponent.GetComponent<TargetScript>();

        //Create Tool image (left hand lower corner)
        GameObject go1 = GameObject.Find("ToolImageGUI");
        Image toolComponent = go1.GetComponent<Image>();
        toolImage = toolComponent.GetComponent<ToolScript>();

        player = GameObject.Find("Player");

        //MASTER ONLY
        if (PhotonNetwork.isMasterClient == true)
        {
            //distribute tools randomly to the markers
            markerDistribution.generateToolDistribution();

            //generate Player List and target list
            playerFunctions.generatePlayerAndTargetList();

            //Master is ready yet! Increment counter
            allReady++;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        //MASTER ONLY
        if(PhotonNetwork.isMasterClient == true)
        {
            //If all players are ready and targets are not distributed yet
            if (allReady >= NetworkManager.expectedNumberOfPlayers && !targetsDistributed)
            {
                targetsDistributed = true;
                //tell all clients that master is done, so they will request a target and player ID
                player.GetComponent<PhotonView>().RPC("rpc_masterIsReady", PhotonTargets.All);
            }

            //If all scores are set and game is not terminated yet and the end game screen is already fully visible
            if(scoreRefreshed >= NetworkManager.expectedNumberOfPlayers && !terminateGame && gameFinishedBackgroundColor.a >= 1.0f)
            {
                terminateGame = true;
                //tell all clients to generate the scoreboard
                player.GetComponent<PhotonView>().RPC("rpc_generateScoreboard", PhotonTargets.All);
            }
        }
        else if (PhotonNetwork.isMasterClient == false)
        {
            //if master has not yet received the client is ready call, send it again
            if (!masterReceived)
                player.GetComponent<PhotonView>().RPC("rpc_playerIsReady", PhotonTargets.MasterClient, PhotonNetwork.player.ID);
            //otherwise we can increase the allReady counter, since the player is ready
            else if(masterReceived && !incrementDone)
            {
                incrementDone = true;
                //increment ready counter
                player.GetComponent<PhotonView>().RPC("rpc_incrementCounter", PhotonTargets.MasterClient);
            }
        }

        //Fade out hit color background (Red effect)
        //If hit effect occurs, the alpha value will be set to 1 again
        if (onHitColor.a > 0.0f)
        {
            onHitColor.a -= 0.02f;
            onHitHUD.color = onHitColor;
        }

        //Face out info text at the top of the hud
        //If info text occurs, the alpha value will be set to 1 again
        if (infoTextColor.a > 0.0f)
        {
            infoTextColor.a -= 0.0075f;
            infoTextHUD.color = infoTextColor;
        }

        
        if (gameFinishedHUD != null)
        {
            //If game finished HUD is active (= game is finished)
            //Fade in background and game finished text
            if (gameFinishedHUD.GetActive())
            {
                //Fade in game finished text
                if (gameFinishedTextColor.a < 1.0f)
                {
                    gameFinishedTextColor.a += 0.003f;
                    gameFinishedText.color = gameFinishedTextColor;
                }

                //Fade in background color 
                if (gameFinishedBackgroundColor.a < 1.0f)
                {
                    gameFinishedBackgroundColor.a += 0.003f;
                    gameFinishedBackground.color = gameFinishedBackgroundColor;
                }
            }
        }
    }  

    //Set tracked target to active marker
    public void setTrackedTarget(int trackedEnemy)
    {
        trackedTarget = (Player_ID)trackedEnemy;
    }

    //Set tracked tool to active marker
    public void setTrackedToolMarker(int trackedTool)
    {
        trackedToolMarker = trackedTool;
    }

    //return the remaining target IDs list (master)
    public List<Player_ID> getAvailableTargets()
    {
        return availableTargets;
    }

    //return the remaining player IDs list (master)
    public List<Player_ID> getAvailablePlayers()
    {
        return availablePlayers;
    }

    //Return marker ID of this player
    public int getMarkerID()
    {
        return markerID;
    }

    //Master checks if the game is finished
    //His player ID is the playerID of the last player that has been arrested
    public void checkIfGameFinished(Player_ID hisPlayerID)
    {
        //loop over all active players
        //Remove lastly arrested player from active player list
        for (int i = 0; i < activePlayers.Count; i++)
        {
            //if active player id matches player id
            if (activePlayers[i] == hisPlayerID)
            {
                //remove whole player and his target from the list
                activePlayers.RemoveAt(i);
                activePlayersTargets.RemoveAt(i);
                break;
            }
        }

        //loop over all active player targets and remove the
        //lastly arrested player as a target from the player
        for (int i = 0; i < activePlayersTargets.Count; i++)
        {
            //if active player target id matches player id
            if (activePlayersTargets[i] == hisPlayerID)
            {
                //set this players target to unknown
                activePlayersTargets[i] = Player_ID.UNKNOWN;
            }
        }

        //init test variable
        bool gameFinished = true;

        //loop over all player targets
        for (int i = 0; i < activePlayersTargets.Count; i++)
        {
            //check if at least one Target is not unknown yet
            //in this case the variable gameFinished will get false and can never be true again
            gameFinished &= (activePlayersTargets[i] == Player_ID.UNKNOWN);
        }

        //if game finished and not stopped yet
        if (gameFinished && !gameStopped)
        {
            //inform all clients that the game is over
            gameStopped = true;
            player.GetComponent<PhotonView>().RPC("rpc_gameIsOver", PhotonTargets.All);
        }
    }

    //This function is requiered though empty
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }


    //PUNRPC Section================================================================

        //Explanation: PunRPC is a remote procedure call - calls a function on some other client / master or on all clients, etc.

    //RPC called if game is finished, so each player will generate the scoreboard
    [PunRPC]
    public void rpc_generateScoreboard()
    {
        //first round of the game - list is generated only with current score
        if (highscoreScript.firstRound)
        {
            highscoreScript.firstRound = false;

            for (int i = 0; i < playerScores.Count; i++)
            {
                highscoreScript.highscores.Add(playerScores[i]);
                highscoreScript.playerIDs.Add(i);
            }
        }

        //otherwise the score of the last round will be incremetned with the current score
        else
        {
            for (int i = 0; i < playerScores.Count; i++)
            {
                highscoreScript.highscores[i] += (playerScores[i]);
            }
        }

        //activate game finished hud and deactivate highscore HUD yet
        gameFinishedHUD.SetActive(false);
        highscoreHUD.SetActive(true);

        //sort score list
        highscoreScript.sort();
    }

    //RPC received by all players to update the score of the sending player
    //Send everytime the score of a player gets updated
    [PunRPC]
    public void rpc_updatePlayerScore(int playerID, int score)
    {
        playerScores[playerID] = score;
    }

    //RPC received by all players, if the master detects, that the game is finished
    [PunRPC]
    public void rpc_gameIsOver()
    {
        //Deactivate HUD canvas if not arrested
        if (!playerArrested)
        {
            GameObject HUDCanvas = GameObject.Find("HUDCanvasGUI");
            HUDCanvas.SetActive(false);
        }

        //otherwise deactivate defeated HUD
        else
        {
            defeatedHUD.SetActive(false);
        }

        //activate game finished HUD
        gameFinishedHUD.SetActive(true);

        //RPCs to tell everyone the final score since game is finished now
        player.GetComponent<PhotonView>().RPC("rpc_updatePlayerScore", PhotonTargets.All, (int)playerID, score);
        
        //Also tell the master that the player score is updated now
        player.GetComponent<PhotonView>().RPC("rpc_incrementScoreRefreshed", PhotonTargets.MasterClient);
    }

    //This RPC is received by master and increments the counter each time a player is connected and readys
    [PunRPC]
    public void rpc_incrementCounter()
    {
        allReady += 1;
    }

    //This RPC is received by master each time, after the game has finished, a player updated his final score 
    [PunRPC]
    public void rpc_incrementScoreRefreshed()
    {
        scoreRefreshed++;
    }

    //This RPC is a check, if master received the message (which means he has connected and is ready yet)
    [PunRPC]
    public void rpc_playerIsReady(int photonID)
    {
        //master sends rpc back to the sender of the prev. call
        player.GetComponent<PhotonView>().RPC("rpc_masterReceived", PhotonPlayer.Find(photonID));
    }

    //Sets the corresponding variable, if the RPC call of the master reaches the client again
    //So now we know, that client and player are both ready and can exchange the playerID and targetID information
    [PunRPC]
    public void rpc_masterReceived()
    {
        masterReceived = true;
    }

    //Once master is ready, the player can request Target and Player ID
    [PunRPC]
    public void rpc_masterIsReady()
    {
        playerFunctions.requestTargetAndPlayer();
    }

    //Receive marker, target and player ID from master
    //This is received by every player because every player saves this relation as everyone needs to know:
        //PlayerID belonging to each marker to set the tracked player correctly
        //and also which photon id belongs to the marker, so that he can set up an rpc call (e.g. when trying to arrest him)
    [PunRPC]
    public void rpc_receiveMarkerPlayerRelation(int markerID, int playersID, int photonID)
    {

        //Save marker ID and player id, photonID relations for each player
        if (PhotonNetwork.isMasterClient == false)
        {
            markerDistribution.setMarkerToPlayer(markerID, playersID);
            markerDistribution.setMarkerToPhotonID(markerID, photonID);
        }

        planePlayer = GameObject.Find("player_" + markerID);

        //Set player face to the plane of the corresponding marker
        planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/player_" + playersID.ToString(), typeof(Texture2D)) as Texture2D;
    }

    //Each player receives ther marker and tool relation from the master
    //So now every player knows, which tool is set to which marker 
    [PunRPC]
    public void rpc_receiveMarkerToolRelation(int markerID, int toolID)
    {
        //save tool type to marker id
        if (PhotonNetwork.isMasterClient == false)
        {
            markerDistribution.setMarkerToTool(markerID, (Tool)toolID);
        }

        planeTool = GameObject.Find("tool_" + markerID);

        //Set tool texture to the plane of the corresponding marker
        if (toolID == 0)
            planeTool.GetComponent<Renderer>().material.mainTexture = toolImageHandcuffs.texture;
        else if (toolID == 1)
            planeTool.GetComponent<Renderer>().material.mainTexture = toolImageInjection.texture;
        else if (toolID == 2)
            planeTool.GetComponent<Renderer>().material.mainTexture = toolImageRope.texture;
    }

    //This call is send from master to each player respectively and contains the players targetID
    [PunRPC]
    public void rpc_receiveTarget(int target, PhotonMessageInfo info)
    {
        //player saves his own target
        targetPlayer = (Player_ID)target;

        //and sets the right face to the target image in the HUD
        targetImage.setImage("Players/player_" + target.ToString());
    }

    //This call is send from master to each player respectively and contains the players playerID
    [PunRPC]
    public void rpc_receivePlayer(int player, PhotonMessageInfo info)
    {
        //player saves his own playerID
        playerID = (Player_ID)player;
    }

    //This tool is received every time, a user picks up a tool
    //so that each player can update that particular marker with the new tool id
    [PunRPC]
    public void rpc_changeToolMarker(int toolID, int marker)
    {
        playerFunctions.changeToolMarker(toolID, marker);
    }

    //This method is called only on the master client
    //it gets the next player and target id and sends it back to the player
    [PunRPC]
    public void rpc_sendTargetAndPlayerToClient(int id, int marker)
    {
        //only if this is the master client, and we still have available target and player IDs left
        if (PhotonNetwork.isMasterClient && availableTargets.Count >= 1 && availablePlayers.Count >= 1)
        {           
            //send the target and player to the client
            int clientTarget = (int)availableTargets[0];
            int clientPlayer = (int)availablePlayers[0];            

            //safety check - target and player id must not be the same
            if (clientTarget == clientPlayer)
            {
                //if they are, just use the next playerID instead of the current
                if (availablePlayers.Count >= 2)
                {
                    clientPlayer = (int)availablePlayers[1];

                    //remove used ids from the lists
                    availableTargets.RemoveAt(0);
                    availablePlayers.RemoveAt(1);
                }
            }
            else
            {
                //remove used ids from the lists
                availableTargets.RemoveAt(0);
                availablePlayers.RemoveAt(0);
            }

            //Save marker, player, photon ID relations on master side
            //other client saves them in rpc_receiveMarkerPlayerRelation method
            markerDistribution.setMarkerToPlayer(marker, clientPlayer);
            markerDistribution.setMarkerToPhotonID(marker, id);

            //add the requesting player to the active players of the game
            activePlayers.Add((Player_ID)clientPlayer);
            activePlayersTargets.Add((Player_ID)clientTarget);

            //send the player and target ID to the requesting player, so that he can save them
            player.GetComponent<PhotonView>().RPC("rpc_receiveTarget", PhotonPlayer.Find(id), clientTarget);
            player.GetComponent<PhotonView>().RPC("rpc_receivePlayer", PhotonPlayer.Find(id), clientPlayer);

            //increment marker player counter - used to check if all players have their player and target ID
            markerPlayerCounter++;
        }

        // if all players have connected and got their player and target ID
        if (markerPlayerCounter >= NetworkManager.expectedNumberOfPlayers)
        {
            //Send all player and marker relations to all players
            for (int i = 0; i < markerDistribution.getMarkerToPlayerCount(); i++)
            {
                player.GetComponent<PhotonView>().RPC("rpc_receiveMarkerPlayerRelation", PhotonTargets.All, i, markerDistribution.getMarkerToPlayer(i), markerDistribution.getMarkerToPhotonID(i));
            }

            //Send all tool and marker relations to all players
            for (int i = 0; i < markerDistribution.getMarkerToToolCount(); i++)
            {
                player.GetComponent<PhotonView>().RPC("rpc_receiveMarkerToolRelation", PhotonTargets.All, i, (int)markerDistribution.getMarkerToTool(i));
            }
        }
    }

    //This RPC is send by a player, when he tries to arrest the player
    [PunRPC]
    public void rpc_continueArresting(int arresterID, float amount)
    {
        //only if player is not arrested yet
        if (!playerArrested)
        {
            //Set hit HUD visible
            onHitColor.a = 1.0f;
            onHitHUD.color = onHitColor;

            //Reduce arrested bar
            jailSliderValue -= amount;
            GUIJailSlider.updateValue(jailSliderValue);

            //If arrested bar is smaller than zero, call player arrested method
            if (jailSliderValue <= 0)
            {
                playerFunctions.playerBeingArrested(arresterID);
            }
        }
    }

    //This RPC is send to all players informing them, that a player has been arrested
    [PunRPC]
    public void rpc_playerWasArrested(int markerID, int arresterID, int hisTargetID)
    {
        //Get plane on the marker of the newly arrested player
        planePlayer = GameObject.Find("player_" + markerID);

        //save player id of the newly arrested player
        int hisPlayerID = markerDistribution.getMarkerToPlayer(markerID);

        //temporary saving of old score
        int oldScore = score;

        //If arrested player was our target
        if (markerDistribution.getMarkerToPlayer(markerID) == (int)targetPlayer)
        {
            //If arrested player was our own target and arrested by us
            if ((int)playerID == arresterID)
            {
                //Exchange face texture with the same face in jail with success sign
                targetImage.setImage("Players/success_arrested_player_" + hisPlayerID.ToString());

                //increment score by one since the arrested player was our target
                score++;
                
                //update score value in GUI
                GUIScoreText.updateScoreValue(score);

                //update target HUD in the top right hand corner
                planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/success_arrested_player_" + hisPlayerID.ToString(), typeof(Texture2D)) as Texture2D;

                //Create info text at the top of the screen
                infoTextHUD.text = "You arrested your target!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }
            //If arrested player was our own target and was not arrested by us
            else
            {
                //Exchange face texture with the same face in jail with success sign
                targetImage.setImage("Players/failed_arrested_player_" + hisPlayerID.ToString());

                //Exchange face texture with the same face in jail with failed sign
                planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/failed_arrested_player_" + hisPlayerID.ToString(), typeof(Texture2D)) as Texture2D;

                //Create info text at the top of the screen
                infoTextHUD.text = "Someone else arrested your target!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }
        }

        //If we were the arrester
        else if ((int)playerID == arresterID)
        {
            //If arrested player was our pursuer
            if (hisTargetID == (int)playerID)
            {
                //increment score by two since we arrested the player that had us as a target
                score += 2;

                //Exchange face texture with the same face in jail with success sign
                planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/success_arrested_player_" + hisPlayerID.ToString(), typeof(Texture2D)) as Texture2D;

                //Create info text at the top of the screen
                infoTextHUD.text = "You arrested your pursuer!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }
            //If the arrested player was either our target nor our pursuer
            else
            {
                //Otherwise arrested the wrong person
                //minimzie score
                score--;
                
                //Exchange face texture with the same face in jail with failed sign
                planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/failed_arrested_player_" + hisPlayerID.ToString(), typeof(Texture2D)) as Texture2D;

                //Create info text at the top of the screen
                infoTextHUD.text = "You arrested the wrong target!";
                infoTextColor.a = 1.0f;
                infoTextHUD.color = infoTextColor;
            }        
        }
        //If we were not the arrester
        else
        {
            //just use neutral texture on the players marker
            planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/arrested_player_" + hisPlayerID.ToString(), typeof(Texture2D)) as Texture2D;
            
            //Create info text at the top of the screen
            infoTextHUD.text = "Someone in the game was arrested!";
            infoTextColor.a = 1.0f;
            infoTextHUD.color = infoTextColor;
        }

        //only if out score was changed, update score in the GUI
        if (oldScore != score)
        {
            GUIScoreText.updateScoreValue(score);           
        }

        //If this is the master client, he will check if the game is finished
        if (PhotonNetwork.isMasterClient == true)
        {
            checkIfGameFinished((Player_ID)hisPlayerID);
        }
    }
}

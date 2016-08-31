using UnityEngine;
using System.Collections;

//This script contains different functions, that each player can execute during the game

public class PlayerFunctionsScript : MonoBehaviour
{

    public PlayerScript playerScript;

    public PlayerFunctionsScript(PlayerScript player)
    {
        playerScript = player;
    }

    //this method is called, whenever the interaction button is pressed
    public void interact()
    {
        //only do something, if the player is not arrested
        if (!playerScript.playerArrested)
        {
            int cast_trackedTarget = (int)playerScript.trackedTarget;

            //If tracked target is a valid player marker, which also belongs to a active player AND the current user has a tool equipped
            if (cast_trackedTarget != playerScript.getMarkerID() && (cast_trackedTarget != (int)Player_ID.UNKNOWN) && (playerScript.playerTool.getToolType() != Tool.NONE))
            {
                int photonID = playerScript.markerDistribution.getMarkerToPhotonID(cast_trackedTarget);

                //Send an rpc call to that player, that he is about to get arrested
                playerScript.player.GetComponent<PhotonView>().RPC("rpc_continueArresting", PhotonPlayer.Find(photonID), (int)playerScript.playerID, playerScript.playerTool.getToolEffectiveness());
            }

            //otherwise check if the current tracked marker is a tool marker and also check if it contains a tool and is not already empty
            else if (playerScript.trackedToolMarker != -1 && playerScript.markerDistribution.getMarkerToTool(playerScript.trackedToolMarker) != Tool.NONE)
            {
                //send rpc call to all other players, that the tool on that marker was taken, and that the marker is now empty or contains the players previous tool
                playerScript.player.GetComponent<PhotonView>().RPC("rpc_changeToolMarker", PhotonTargets.Others, (int)playerScript.playerTool.getToolType(), playerScript.trackedToolMarker);

                //exchange the tool on the marker with the players current tool
                PlayerToolScript tmp = new PlayerToolScript(playerScript.markerDistribution.getMarkerToTool(playerScript.trackedToolMarker));
                changeToolMarker((int)playerScript.playerTool.getToolType(), playerScript.trackedToolMarker);
                playerScript.playerTool = tmp;
                playerScript.toolImage.setImage((int)playerScript.playerTool.getToolType());
            }
        }
    }

    public void playerBeingArrested(int attackerID)
    {
        //player was arrested
        playerScript.playerArrested = true;

        //inform all other players, that the current player was arrested
        playerScript.player.GetComponent<PhotonView>().RPC("rpc_playerWasArrested", PhotonTargets.Others, playerScript.getMarkerID(), attackerID, (int)playerScript.targetPlayer);

        //change the HUD of the player to the jail hud
        GameObject hudCanvasGUI_gameObject = GameObject.Find("HUDCanvasGUI");
        hudCanvasGUI_gameObject.SetActive(false);
        playerScript.defeatedHUD.SetActive(true);
        playerScript.planePlayer = GameObject.Find("player_" + playerScript.getMarkerID().ToString());
        int cast_playerID = (int)playerScript.playerID;
        playerScript.planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/arrested_player_" + cast_playerID.ToString(), typeof(Texture2D)) as Texture2D;

        //if this is executed by the master client, check if the game is finished
        if (PhotonNetwork.isMasterClient == true)
            playerScript.checkIfGameFinished(playerScript.playerID);
    }


    public void generatePlayerAndTargetList()
    {
        //Add all available players to a list for available players and also available targets
        for (int i = 0; i < NetworkManager.expectedNumberOfPlayers; i++)
        {
            playerScript.getAvailableTargets().Add((Player_ID)i);
            playerScript.getAvailablePlayers().Add((Player_ID)i);
        }

        //the following code is some logic, to prevent the players from having themselves as a target
        //and also to give the targets some random order, that not every round the same target distribution exists
        bool invalidList = true;
        while (invalidList)
        {
            invalidList = false;

            //Shuffle list randomly
            for (int i = 0; i < playerScript.getAvailableTargets().Count; i++)
            {
                //swap target list
                Player_ID temp_target = playerScript.getAvailableTargets()[i];
                int randomIndex_target = Random.Range(0, playerScript.getAvailableTargets().Count);
                playerScript.getAvailableTargets()[i] = playerScript.getAvailableTargets()[randomIndex_target];
                playerScript.getAvailableTargets()[randomIndex_target] = temp_target;

                //swap player list
                Player_ID temp_player = playerScript.getAvailablePlayers()[i];
                int randomIndex_player = Random.Range(0, playerScript.getAvailablePlayers().Count);
                playerScript.getAvailablePlayers()[i] = playerScript.getAvailablePlayers()[randomIndex_player];
                playerScript.getAvailablePlayers()[randomIndex_player] = temp_player;
            }

            //if some player has himself as target, shuffle again
            for (int i = 0; i < playerScript.getAvailablePlayers().Count; i++)
            {
                if (playerScript.getAvailablePlayers()[i] == playerScript.getAvailableTargets()[i])
                    invalidList = true;
            }
        }
    }

    //this function sends request to the master, to receive a target and player id. This function is called by a new player on joining the game
    public void requestTargetAndPlayer()
    {
        playerScript.player.GetComponent<PhotonView>().RPC("rpc_sendTargetAndPlayerToClient", PhotonTargets.MasterClient, PhotonNetwork.player.ID, playerScript.getMarkerID());
    }

    //change the texture of a marker (called if a tool is picked up)
    public void changeToolMarker(int toolID, int marker)
    {
        playerScript.planeTool = GameObject.Find("tool_" + marker);

        playerScript.markerDistribution.setMarkerToTool(marker, (Tool)toolID);
        if (toolID == (int)Tool.HANDCUFFS)
            playerScript.planeTool.GetComponent<Renderer>().material.mainTexture = playerScript.toolImageHandcuffs.texture;
        else if (toolID == (int)Tool.INJECTION)
            playerScript.planeTool.GetComponent<Renderer>().material.mainTexture = playerScript.toolImageInjection.texture;
        else if (toolID == (int)Tool.ROPE)
            playerScript.planeTool.GetComponent<Renderer>().material.mainTexture = playerScript.toolImageRope.texture;
        else if (toolID == (int)Tool.NONE)
            playerScript.planeTool.GetComponent<Renderer>().material.mainTexture = playerScript.toolImageLooted.texture;
    }
}

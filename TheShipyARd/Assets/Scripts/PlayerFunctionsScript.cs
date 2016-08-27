using UnityEngine;
using System.Collections;

public class PlayerFunctionsScript : MonoBehaviour {

    public PlayerScript playerScript;

	public PlayerFunctionsScript(PlayerScript player)
    {
        playerScript = player;
    }

    public void attack()
    {
        if (!playerScript.playerDead)
        {
            int cast_trackedTarget = (int)playerScript.trackedTarget;

            if (cast_trackedTarget != playerScript.getMarkerID() && (cast_trackedTarget != (int)Target.UNKNOWN) && (playerScript.playerTool.getToolType() != Tool.NONE))
            {
                int photonID = playerScript.markerDistribution.getMarkerToPhotonID(cast_trackedTarget);

                playerScript.player.GetComponent<PhotonView>().RPC("rpc_takeDamage", PhotonPlayer.Find(photonID), (int)playerScript.playerID, playerScript.playerTool.getToolDamage());
            }
            else if (playerScript.trackedToolMarker != -1 && playerScript.markerDistribution.getMarkerToTool(playerScript.trackedToolMarker) != Tool.NONE)
            {
                playerScript.player.GetComponent<PhotonView>().RPC("rpc_changeToolMarker", PhotonTargets.Others, (int)playerScript.playerTool.getToolType(), playerScript.trackedToolMarker);
                PlayerToolScript tmp = new PlayerToolScript(playerScript.markerDistribution.getMarkerToTool(playerScript.trackedToolMarker));
                changeToolMarker((int)playerScript.playerTool.getToolType(), playerScript.trackedToolMarker);
                playerScript.playerTool = tmp;
                playerScript.toolImage.setImage((int)playerScript.playerTool.getToolType());
            }
        }
    }

    public void playerDying(int attackerID)
    {
        playerScript.playerDead = true;
        playerScript.player.GetComponent<PhotonView>().RPC("rpc_playerDied", PhotonTargets.Others, playerScript.getMarkerID(), attackerID, (int)playerScript.targetPlayer);
        GameObject go0 = GameObject.Find("HUDCanvasGUI");
        go0.SetActive(false);
        playerScript.defeatedHUD.SetActive(true);
        playerScript.planePlayer = GameObject.Find("player_" + playerScript.getMarkerID());
        playerScript.planePlayer.GetComponent<Renderer>().material.mainTexture = Resources.Load("Players/arrested_player_" + playerScript.playerID.ToString(), typeof(Texture2D)) as Texture2D;
    }


    public void generatePlayerAndTargetList()
    {
        //Add all target to a list
        for (int i = 0; i < NetworkManager.expectedNumberOfPlayers; i++)
        {
            playerScript.getAvailableTargets().Add((Target)i);
            playerScript.getAvailablePlayers().Add((Target)i);
        }

        //Shuffle lists
        for (int i = 0; i < playerScript.getAvailableTargets().Count; i++)
        {
            Target temp = playerScript.getAvailableTargets()[i];
            int randomIndex = Random.Range(i, playerScript.getAvailableTargets().Count - 1);
            playerScript.getAvailableTargets()[i] = playerScript.getAvailableTargets()[randomIndex];
            playerScript.getAvailableTargets()[randomIndex] = temp;
        }

        //copy list
        for (int i = 0; i < playerScript.getAvailableTargets().Count; i++)
        {
            playerScript.getAvailablePlayers()[i] = playerScript.getAvailableTargets()[i];
        }

        playerScript.getAvailablePlayers().Reverse();

    }

    public void requestTargetAndPlayer()
    {
        playerScript.player.GetComponent<PhotonView>().RPC("rpc_sendTargetAndPlayerToClient", PhotonTargets.MasterClient, PhotonNetwork.player.ID, playerScript.getMarkerID());

        //player.GetComponent<PhotonView>().RPC("rpc_sendTargetAndPlayerToClient", PhotonTargets.MasterClient);
    }

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

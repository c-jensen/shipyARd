using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Photon;

public class NetworkManager : Photon.PunBehaviour
{
    public static int expectedNumberOfPlayers = 2;
    public static int currentNumberOfPlayers = 0;
    public static bool isConnected = false;

    //List containing all unused targets
    private List<Target> availableTargets = new List<Target>();
    //List containing all unused player IDs
    private List<Target> availablePlayers = new List<Target>();

    public static PhotonView photonView;

    void Start()
    {
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
        PhotonNetwork.ConnectUsingSettings("0.1");

        photonView = PhotonView.Get(this);
    }

    void OnGUI()
    {
        //GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        isConnected = true;

        //the first client becomes the master
        if (PhotonNetwork.masterClient == null)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.player);
        }

        if (PhotonNetwork.isMasterClient)
        {
            //Add all target to a list
            for(int i = 0; i < (int) Target.MAX_NUM_OF_TARGETS; i++)
            {
                availableTargets[i] = (Target)i;
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
            for(int i = 0; i < availableTargets.Count - expectedNumberOfPlayers; i++)
            {
                availableTargets.RemoveAt(i);
            }

            availablePlayers = availableTargets;
            availablePlayers.Reverse();
            //In case of an odd number of players the middle element gets switched
            if(availablePlayers.Count % 2 == 1)
            {
                int switchIndex = (int)Mathf.Floor(availablePlayers.Count / 2.0f);
                Target tmp = availablePlayers[0];
                availablePlayers[0] = availablePlayers[switchIndex];
                availablePlayers[switchIndex] = tmp;
            }
        }
        
        
    }

    void Update()
    {
        currentNumberOfPlayers = PhotonNetwork.room.playerCount;

        if (currentNumberOfPlayers == expectedNumberOfPlayers)
        {
            Application.LoadLevel(2);
        }
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null); //null could be any room name id
    }

    [PunRPC]
    public void rpc_sendTargetAndPlayerToClient(PhotonPlayer sender)
    {
        if (PhotonNetwork.isMasterClient && availableTargets.Count >= 1 && availablePlayers.Count >= 1)
        {
            //send the target to the client
            Target clientTarget = availableTargets[0];
            availableTargets.RemoveAt(0);
            NetworkManager.photonView.RPC("rpc_receiveTarget", sender, clientTarget);

            //send the player ID to the client
            Target clientPlayer = availablePlayers[0];
            availablePlayers.RemoveAt(0);
            NetworkManager.photonView.RPC("rpc_reveivePlayer", sender, clientPlayer);
        }
    }
}

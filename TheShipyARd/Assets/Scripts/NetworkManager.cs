using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Photon;

//this class is responsible for all the network management done when starting the game

public class NetworkManager : Photon.PunBehaviour
{
    //expected number of players is the number which is set during the start menu via the slider
    public static int expectedNumberOfPlayers = 2;

    //current number of players is the count of players that have joined the room
    public static int currentNumberOfPlayers = 0;

    public static bool isConnected = false;

    IEnumerator waitRoutine()
    {
        yield return new WaitForSeconds(15.0f);
    }

    void Start()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            StartCoroutine(waitRoutine());
        }

        PhotonNetwork.logLevel = PhotonLogLevel.Full;
        PhotonNetwork.ConnectUsingSettings("0.1");
    }
    void Update()
    {
        //update: get current Number of players from photon network
        if (isConnected == true)
            currentNumberOfPlayers = PhotonNetwork.room.playerCount;

        //of currentNumberOfPlayers is reached, close the room for other players and load the next level
        if (currentNumberOfPlayers == expectedNumberOfPlayers)
        {
            Application.LoadLevel(2);
            PhotonNetwork.room.open = false;
        }
    }

    public override void OnJoinedLobby()
    {
        //attempt to join a random room
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        //joining of room was successful
        isConnected = true;

        //the first client becomes the master client
        if (PhotonNetwork.masterClient == null)
            PhotonNetwork.SetMasterClient(PhotonNetwork.player);
    }

    void OnPhotonRandomJoinFailed()
    {
        //if random join failed, create a new room
        PhotonNetwork.CreateRoom(null);
    }
}

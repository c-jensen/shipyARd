using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Photon;

public class NetworkManager : Photon.PunBehaviour
{
    public static int expectedNumberOfPlayers = 2;
    public static int currentNumberOfPlayers = 0;
    public static bool isConnected = false;

    IEnumerator waitRoutine()
    {
        yield return new WaitForSeconds(5.0f);
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
        if(isConnected == true)
            currentNumberOfPlayers = PhotonNetwork.room.playerCount;

        if (currentNumberOfPlayers == expectedNumberOfPlayers)
        {
            Application.LoadLevel(2);
            PhotonNetwork.room.open = false;
        }
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
        Debug.LogError("Room joined");

        isConnected = true;

        //the first client becomes the master
        if (PhotonNetwork.masterClient == null)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.player);
            Debug.LogError("master is set");
        }

        if(PhotonNetwork.isMasterClient == true)
            Debug.LogError("i am da fuckin mastaaaaa");


    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null); //null could be any room name id
    }   
}

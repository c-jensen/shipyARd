using UnityEngine;
using System.Collections;

using Photon;

public class RandomMatchMaker : Photon.PunBehaviour
{
    public static int expectedNumberOfPlayers = 2;
    public static int currentNumberOfPlayers = 0;
    public static bool isConnected = false;

    void Start()
    {
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
        PhotonNetwork.ConnectUsingSettings("0.1");
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
    }

    void Update()
    {
        currentNumberOfPlayers = PhotonNetwork.room.playerCount;

        if (currentNumberOfPlayers == expectedNumberOfPlayers)
        {
            Application.LoadLevel(2);
            //TODO: DELETE THIS LATER
            /*
            GameObject playerObject = PhotonNetwork.Instantiate("PlayerObject", Vector3.zero, Quaternion.identity, 0);

            // Der Parent des sharedCubes wird auf den Marker gesetzt damit der Cube gerendert wird
            GameObject sharedCube = PhotonNetwork.Instantiate("SharedCube", Vector3.zero, Quaternion.identity, 0);
            sharedCube.transform.parent = GameObject.Find("p1_target").transform;
            */
        }
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null); //null could be any room name id
    }
}

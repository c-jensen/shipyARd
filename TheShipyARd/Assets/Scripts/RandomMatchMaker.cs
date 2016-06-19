using UnityEngine;
using System.Collections;

using Photon;

public class RandomMatchMaker : Photon.PunBehaviour
{

    void Start()
    {
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        GameObject sharedCube = PhotonNetwork.Instantiate("SharedCube", Vector3.zero, Quaternion.identity, 0);
        sharedCube.transform.parent = GameObject.Find("p1_target").transform;
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null); //null could be any room name id
    }
}
